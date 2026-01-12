using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models; // Asigură-te că namespace-ul e corect pentru Room, Admin, Manager
using System.Linq;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin; // <--- Trebuie să ținem minte cine e șeful

    // Lista vizibilă în tabel
    public ObservableCollection<Room> Camere { get; set; }

    // Statistici (Dashboard)
    public int TotalRooms => Camere.Count;
    // Folosim GetActiveReservations cu adminul curent
    public int TotalReservations => _manager.GetActiveReservations(_currentAdmin).Count();
    public int TotalUsers => 0; 
    public int TotalClients => 0;

    public ICommand CheckInCommand { get; }
    public ICommand CheckOutCommand { get; }

    // Constructorul cere Managerul ȘI Adminul logat
    public AdminViewModel(Manager manager, Admin adminLogat)
    {
        _manager = manager;
        _currentAdmin = adminLogat;

        // 1. Cerem datele folosind permisiunea adminului
        var roomsList = _manager.GetAllRooms(_currentAdmin);
        Camere = new ObservableCollection<Room>(roomsList);

        // 2. Configurăm butoanele
        CheckInCommand = new RelayCommand(ExecuteCheckIn);
        CheckOutCommand = new RelayCommand(ExecuteCheckOut);
    }

    private void ExecuteCheckIn(object parameter)
    {
        if (parameter is int roomNumber)
        {
            // Căutăm camera în lista locală ca să vedem statusul curent
            var room = Camere.FirstOrDefault(r => r.Number == roomNumber);
            
            if (room != null && room.Status == RoomStatus.Free) // Sau RoomStatus.Available
            {
                // Apelăm metoda ta din Manager: SetRoomStatus
                _manager.SetRoomStatus(_currentAdmin, roomNumber, RoomStatus.Occupied);
                
                RefreshLista();
                MessageBox.Show($"Check-In efectuat pentru camera {roomNumber}!", "Succes");
            }
            else
            {
                MessageBox.Show("Camera nu este liberă!", "Eroare");
            }
        }
    }

    private void ExecuteCheckOut(object parameter)
    {
        if (parameter is int roomNumber)
        {
            // Apelăm metoda ta din Manager pentru a elibera camera
            _manager.SetRoomStatus(_currentAdmin, roomNumber, RoomStatus.Cleaning); // Sau Free direct
            
            RefreshLista();
            MessageBox.Show($"Check-Out efectuat. Camera {roomNumber} e la curățenie.", "Info");
        }
    }

    // Funcție critică: Reîmprospătăm tabelul după modificări
    private void RefreshLista()
    {
        Camere.Clear();
        // Cerem din nou lista actualizată din Manager
        foreach (var r in _manager.GetAllRooms(_currentAdmin))
        {
            Camere.Add(r);
        }
        // Actualizăm și cifrele de sus
        OnPropertyChanged(nameof(TotalRooms));
        OnPropertyChanged(nameof(TotalReservations));
    }
}