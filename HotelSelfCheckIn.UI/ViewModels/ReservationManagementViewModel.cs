using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.Views;
using System.Linq;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ReservationManagementViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin;
    private readonly FileService _fileService;

    private ObservableCollection<Reservation> _reservations;
    public ObservableCollection<Reservation> Reservations
    {
        get => _reservations;
        set
        {
            _reservations = value;
            OnPropertyChanged();
        }
    }

    // Comenzi
    public ICommand AddReservationCommand { get; }
    public ICommand ShowActiveCommand { get; }  // Pt RadioButton Active
    public ICommand ShowHistoryCommand { get; } // Pt RadioButton History
    public ICommand CancelReservationCommand { get; } // Pt butonul X
    public ICommand CompleteReservationCommand { get; } // Pt butonul ✅

    public ReservationManagementViewModel(Manager manager, Admin currentAdmin, FileService fileService)
    {
        _manager = manager;
        _currentAdmin = currentAdmin;
        _fileService = fileService;

        // Inițializăm Comenzile
        AddReservationCommand = new RelayCommand(ExecuteAddReservation);
        ShowActiveCommand = new RelayCommand(_ => LoadReservations(true));
        ShowHistoryCommand = new RelayCommand(_ => LoadReservations(false));
        
        CancelReservationCommand = new RelayCommand(ExecuteCancel);
        CompleteReservationCommand = new RelayCommand(ExecuteComplete);

        // Încărcăm default cele active
        LoadReservations(true);
    }

    // --- LOGICA DE FILTRARE ---
    private void LoadReservations(bool activeOnly)
    {
        IEnumerable<Reservation> list;
        if (activeOnly)
        {
            list = _manager.GetActiveReservations(_currentAdmin);
        }
        else
        {
            // Combinăm Completed + Cancelled pentru istoric
            var history = _manager.GetHistoryReservations(_currentAdmin); // Completed
            // Dacă ai o metodă pt Cancelled în Manager, o adaugi aici. Momentan luăm Completed.
            list = history;
        }
        Reservations = new ObservableCollection<Reservation>(list);
    }

    // --- LOGICA ADAUGARE ---
    private void ExecuteAddReservation(object parameter)
    {
        var window = new AddReservationWindow();
        if (window.ShowDialog() == true)
        {
            try
            {
                _manager.CreateReservation(_currentAdmin, window.Username, window.RoomNumber, window.StartDate, window.EndDate);
                SaveAndRefresh();
                MessageBox.Show("Rezervare creată!", "Succes");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare: {ex.Message}", "Eroare");
            }
        }
    }

    // --- LOGICA ANULARE (Butonul X) ---
    private void ExecuteCancel(object parameter)
    {
        if (parameter is Guid resId) // Primim ID-ul din XAML
        {
            if (MessageBox.Show("Sigur anulezi rezervarea?", "Confirmare", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _manager.ForceChangeStatus(_currentAdmin, resId, ReservationStatus.Cancelled);
                SaveAndRefresh();
            }
        }
    }

    // --- LOGICA COMPLETARE (Butonul ✅) ---
    private void ExecuteComplete(object parameter)
    {
        if (parameter is Guid resId)
        {
            _manager.ForceChangeStatus(_currentAdmin, resId, ReservationStatus.Completed);
            SaveAndRefresh();
        }
    }

    private void SaveAndRefresh()
    {
        var all = _manager.GetAllReservations(_currentAdmin);
        _fileService.SaveReservations(all);
        LoadReservations(true); // Reîncărcăm lista activă
    }
}