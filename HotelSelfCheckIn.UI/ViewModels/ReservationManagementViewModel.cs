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

    // --- COMENZI 
    public ICommand ShowActiveCommand { get; }
    public ICommand ShowHistoryCommand { get; }
    
    //--- BUTOANE
    public ICommand EditReservationCommand { get; }
    public ICommand RemoveReservationCommand { get; } 
    public ICommand CompleteReservationCommand { get; }
    public ICommand CancelReservationCommand { get; }
    
    //---CONSTRUCTOR
    public ReservationManagementViewModel(Manager manager, Admin currentAdmin, FileService fileService)
    {
        _manager = manager;
        _currentAdmin = currentAdmin;
        _fileService = fileService;
        
        ShowActiveCommand = new RelayCommand(_ => LoadReservations(true));
        ShowHistoryCommand = new RelayCommand(_ => LoadReservations(false));
        
        CompleteReservationCommand = new RelayCommand(param => ExecuteChangeStatus(param, ReservationStatus.Completed));
        CancelReservationCommand = new RelayCommand(param => ExecuteChangeStatus(param, ReservationStatus.Cancelled));
        
        EditReservationCommand = new RelayCommand(ExecuteEditReservation);
        RemoveReservationCommand = new RelayCommand(ExecuteRemoveSelected);
        
        LoadReservations(true);
        
        
        
    }
    
    //---ALGORITMI


    private void LoadReservations(bool activeOnly)
    {
       
        try
        {
            if (activeOnly)
                Reservations = new ObservableCollection<Reservation>(_manager.GetActiveReservations(_currentAdmin));
            else
                Reservations = new ObservableCollection<Reservation>(_manager.GetHistoryReservations(_currentAdmin));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la încărcarea listei: {ex.Message}");
        }
    }


    private void SaveAndRefresh()
    {
       
        try
        {
            var allData = _manager.GetAllReservations(_currentAdmin);
            _fileService.SaveReservations(allData);
           
            bool isActiveView = true; 
            if (Reservations != null && Reservations.Any())
            {
                isActiveView = Reservations.Any(r => r.Status == ReservationStatus.Active);
            }
            
            LoadReservations(isActiveView);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la salvare: {ex.Message}");
        }
    }
    
    

    private void ExecuteChangeStatus(object parameter, ReservationStatus newStatus)
    {
        
        try
        {
            if (parameter is Reservation res)
            {
                if (newStatus == ReservationStatus.Cancelled && 
                    MessageBox.Show("Sigur vrei sa anulezi?", "Confirmare", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                _manager.ForceChangeStatus(_currentAdmin, res.ReservationID, newStatus);
                SaveAndRefresh();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare status: {ex.Message}");
        }
    }

    private void ExecuteRemoveSelected(object parameter)
    {
        
        try
        {
            var resToDelete = parameter as Reservation ?? SelectedReservation;
            
            if (resToDelete == null)
            {
                MessageBox.Show("Selecteaza o rezervare din tabel!", "Atentie");
                return;
            }

            if (MessageBox.Show("Sigur anulezi rezervarea selectata?", "Confirmare", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                _manager.ForceChangeStatus(_currentAdmin, resToDelete.ReservationID, ReservationStatus.Cancelled);
                SaveAndRefresh();
            }
        }
        catch (Exception ex)
        {
           
            MessageBox.Show($"Eroare la anulare: {ex.Message}", "Eroare");
        }
    }

    private void ExecuteEditReservation(object parameter)
    {
      
        try
        {
            if (SelectedReservation == null)
            {
                MessageBox.Show("Selecteaza o rezervare din tabel!", "Atentie");
                return;
            }

            var window = new EditReservationWindow();
            var editVm = new EditReservationViewModel(SelectedReservation);
            window.DataContext = editVm;

            if (window.ShowDialog() == true)
            {
               
                _manager.AdminUpdateReservation(
                    _currentAdmin, 
                    SelectedReservation.ReservationID, 
                    editVm.GetParsedRoomNumber(), 
                    editVm.GetStartDate(), 
                    editVm.GetEndDate()
                );

                SaveAndRefresh();
                MessageBox.Show("Rezervare modificata cu succes!", "Succes");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la editare: {ex.Message}", "Eroare");
        }
    }
}
    
