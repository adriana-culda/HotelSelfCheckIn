using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.Views;

namespace HotelSelfCheckIn.UI.ViewModels;

public class RoomManagementViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin;

    // --- LISTA DE CAMERE ---
    private ObservableCollection<Room> _rooms;
    public ObservableCollection<Room> Rooms
    {
        get => _rooms;
        set { _rooms = value; OnPropertyChanged(); }
    }

    // --- SELECȚIA ---
    private Room _selectedRoom;
    public Room SelectedRoom
    {
        get => _selectedRoom;
        set { _selectedRoom = value; OnPropertyChanged(); }
    }

    // --- COMENZI ---
    public ICommand AddRoomCommand { get; }
    public ICommand EditRoomCommand { get; }
    public ICommand RemoveRoomCommand { get; }
    
    // Comenzi rapide (tabel)
    public ICommand SetCleaningCommand { get; }
    public ICommand SetMaintenanceCommand { get; }

    public RoomManagementViewModel(Manager manager, Admin currentAdmin)
    {
        _manager = manager;
        _currentAdmin = currentAdmin;

        AddRoomCommand = new RelayCommand(ExecuteAdd);
        EditRoomCommand = new RelayCommand(ExecuteEdit);
        RemoveRoomCommand = new RelayCommand(ExecuteRemove);
        
        SetCleaningCommand = new RelayCommand(param => ExecuteChangeStatus(param, RoomStatus.Cleaning));
        SetMaintenanceCommand = new RelayCommand(param => ExecuteChangeStatus(param, RoomStatus.Unavailable));

        LoadRooms();
    }
    
    private void ExecuteChangeStatus(object parameter, RoomStatus newStatus)
    {
        
        if (parameter is Room room)
        {
            try
            {
                
                _manager.SetRoomStatus(_currentAdmin, room.Number, newStatus);
            
               
                LoadRooms(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare: {ex.Message}");
            }
        }
    }

    private void LoadRooms()
    {
        IEnumerable<Room> listaCamere = _manager.GetAllRooms(_currentAdmin);
        Rooms = new ObservableCollection<Room>(listaCamere);
    }

    // --- 1. ADAUGARE
    private void ExecuteAdd(object parameter)
    {
        var vm = new AddRoomViewModel(null);
        
        bool? result = ShowRoomWindow(vm);

        if (result == true)
        {
            try
            {
                _manager.AddRoom(_currentAdmin, vm.ResultRoom);
                LoadRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la adăugare: {ex.Message}");
            }
        }
    }

    // --- 2. EDITARE
    private void ExecuteEdit(object parameter)
    {
        if (SelectedRoom == null)
        {
            MessageBox.Show("Selectează o cameră pentru modificare.");
            return;
        }

        
        var vm = new EditRoomViewModel(SelectedRoom);

       
        var window = new EditRoomWindow(); 
        window.DataContext = vm;

        vm.CloseAction = (success) =>
        {
            window.DialogResult = success;
            window.Close();
        };

       
        if (window.ShowDialog() == true)
        {
            try
            {
                
                _manager.UpdateRoom(_currentAdmin, SelectedRoom.Number, vm.ResultRoom);
                LoadRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la modificare: {ex.Message}");
            }
        }
    }

    // --- 3. ȘTERGERE
    private void ExecuteRemove(object parameter)
    {
        var roomToDelete = parameter as Room ?? SelectedRoom;

        if (roomToDelete == null)
        {
            MessageBox.Show("Selectează o cameră.");
            return;
        }

        
        if (MessageBox.Show($"Sigur ștergi camera {roomToDelete.Number}?", "Confirmare", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
               
                _manager.DeleteRoom(_currentAdmin, roomToDelete.Number);
                LoadRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nu s-a putut șterge: {ex.Message}");
            }
        }
    }

    // --- HELPER STATUS ---
    private void ChangeStatus(object parameter, RoomStatus newStatus)
    {
        if (parameter is Room room)
        {
            
            _manager.SetRoomStatus(_currentAdmin, room.Number, newStatus);
            LoadRooms();
        }
    }

    // --- HELPER DESCHIDERE FEREASTRA
    private bool? ShowRoomWindow(AddRoomViewModel vm)
    {
        var window = new AddRoomWindow();
        window.DataContext = vm;

        vm.CloseAction = (success) =>
        {
            window.DialogResult = success;
            window.Close();
        };

        return window.ShowDialog();
    }
}