using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models; // Asigura-te ca namespace-ul e corect pentru Room, Admin, Manager
using System.Linq;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin; 

    // Lista vizibila In tabel
    public ObservableCollection<Room> Camere { get; set; }

    // Statistici (Dashboard)
    public int TotalRooms => Camere.Count;
    // Folosim GetActiveReservations cu adminul curent
    public int TotalReservations => _manager.GetActiveReservations(_currentAdmin).Count();
    public int TotalUsers => 0; 
    public int TotalClients => 0;

    public ICommand CheckInCommand { get; }
    public ICommand CheckOutCommand { get; }

    // Constructorul cere Managerul si Adminul logat
    public AdminViewModel(Manager manager, Admin adminLogat)
    {
        _manager = manager;
        _currentAdmin = adminLogat;

        // 1. Cerem datele folosind permisiunea adminului
        var roomsList = _manager.GetAllRooms(_currentAdmin);
        Camere = new ObservableCollection<Room>(roomsList);

        // 2. Configuram butoanele
        CheckInCommand = new RelayCommand(ExecuteCheckIn);
        CheckOutCommand = new RelayCommand(ExecuteCheckOut);
    }

    private void ExecuteCheckIn(object parameter)
    {
        if (parameter is int roomNumber)
        {
            // Cautam camera In lista locala ca sa vedem statusul curent
            var room = Camere.FirstOrDefault(r => r.Number == roomNumber);
            
            if (room != null && room.Status == RoomStatus.Free) // Sau RoomStatus.Available
            {
                // Apelam metoda din Manager: SetRoomStatus
                _manager.SetRoomStatus(_currentAdmin, roomNumber, RoomStatus.Occupied);
                
                RefreshLista();
                MessageBox.Show($"Check-In efectuat pentru camera {roomNumber}!", "Succes");
            }
            else
            {
                MessageBox.Show("Camera nu este libera!", "Eroare");
            }
        }
    }

    private void ExecuteCheckOut(object parameter)
    {
        if (parameter is int roomNumber)
        {
            // Apelam metoda  din Manager pentru a elibera camera
            _manager.SetRoomStatus(_currentAdmin, roomNumber, RoomStatus.Cleaning); // Sau Free direct
            
            RefreshLista();
            MessageBox.Show($"Check-Out efectuat. Camera {roomNumber} e la curatenie.", "Info");
        }
    }

    // Functie critica: ReImprospatam tabelul dupa modificari
    private void RefreshLista()
    {
        Camere.Clear();
        // Cerem din nou lista actualizata din Manager
        foreach (var r in _manager.GetAllRooms(_currentAdmin))
        {
            Camere.Add(r);
        }
        // Actualizam si cifrele de sus
        OnPropertyChanged(nameof(TotalRooms));
        OnPropertyChanged(nameof(TotalReservations));
    }
}