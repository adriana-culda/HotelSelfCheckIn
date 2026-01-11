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

    // --- LISTA ---
    private ObservableCollection<Reservation> _reservations;
    public ObservableCollection<Reservation> Reservations
    {
        get => _reservations;
        set { _reservations = value; OnPropertyChanged(); }
    }

    // --- SELECȚIA ---
    private Reservation _selectedReservation;
    public Reservation SelectedReservation
    {
        get => _selectedReservation;
        set { _selectedReservation = value; OnPropertyChanged(); }
    }

    // --- COMENZI (Le păstrăm pe toate) ---
    public ICommand AddReservationCommand { get; }  // O păstrăm în cod!
    public ICommand EditReservationCommand { get; }
    public ICommand CancelSelectedCommand { get; } 
    
    public ICommand ShowActiveCommand { get; }
    public ICommand ShowHistoryCommand { get; }
    public ICommand CancelReservationCommand { get; } 
    public ICommand CompleteReservationCommand { get; } 

    public ReservationManagementViewModel(Manager manager, Admin currentAdmin, FileService fileService)
    {
        _manager = manager;
        _currentAdmin = currentAdmin;
        _fileService = fileService;

        // Inițializăm tot
        AddReservationCommand = new RelayCommand(ExecuteAddReservation);
        EditReservationCommand = new RelayCommand(ExecuteEditReservation);
        CancelSelectedCommand = new RelayCommand(ExecuteCancelSelected);

        ShowActiveCommand = new RelayCommand(_ => LoadReservations(true));
        ShowHistoryCommand = new RelayCommand(_ => LoadReservations(false));
        
        CancelReservationCommand = new RelayCommand(ExecuteCancelInline);
        CompleteReservationCommand = new RelayCommand(ExecuteCompleteInline);

        LoadReservations(true);
    }

    // --- LOGICA ADAUGARE (O păstrăm pentru viitor/client) ---
    private void ExecuteAddReservation(object parameter)
    {
        var window = new AddReservationWindow(); // Asigură-te că ai fișierul AddReservationWindow
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

    // --- LOGICA EDITARE ---
    private void ExecuteEditReservation(object parameter)
    {
        if (SelectedReservation == null)
        {
            MessageBox.Show("Selectează o rezervare din tabel mai întâi!", "Atenție");
            return;
        }

        var window = new EditReservationWindow(SelectedReservation);
        if (window.ShowDialog() == true)
        {
            try
            {
                _manager.AdminUpdateReservation(
                    _currentAdmin, 
                    SelectedReservation.ReservationID, 
                    window.NewRoomNumber, 
                    window.NewStartDate, 
                    window.NewEndDate
                );
                SaveAndRefresh();
                MessageBox.Show("Rezervare modificată!", "Succes");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare: {ex.Message}", "Eroare");
            }
        }
    }

    // --- LOGICA ANULARE ---
    private void ExecuteCancelSelected(object parameter)
    {
        if (SelectedReservation == null)
        {
            MessageBox.Show("Selectează o rezervare din tabel!", "Atenție");
            return;
        }
        ProcessCancel(SelectedReservation.ReservationID);
    }

    private void ExecuteCancelInline(object parameter)
    {
        if (parameter is Guid resId) ProcessCancel(resId);
    }

    private void ExecuteCompleteInline(object parameter)
    {
        if (parameter is Guid resId)
        {
            _manager.ForceChangeStatus(_currentAdmin, id: resId, ReservationStatus.Completed);
            SaveAndRefresh();
        }
    }

    private void ProcessCancel(Guid id)
    {
        if (MessageBox.Show("Sigur anulezi rezervarea?", "Confirmare", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            _manager.ForceChangeStatus(_currentAdmin, id, ReservationStatus.Cancelled);
            SaveAndRefresh();
        }
    }
    
    private void SaveAndRefresh()
    {
        var all = _manager.GetAllReservations(_currentAdmin);
        _fileService.SaveReservations(all);
        LoadReservations(true); 
    }
    
    private void LoadReservations(bool activeOnly)
    {
        if (activeOnly) Reservations = new ObservableCollection<Reservation>(_manager.GetActiveReservations(_currentAdmin));
        else Reservations = new ObservableCollection<Reservation>(_manager.GetHistoryReservations(_currentAdmin));
    }
}