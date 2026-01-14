using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class CRoomSearchViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _client;
    
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set { _currentViewModel = value; OnPropertyChanged(); }
    }
    // --- INPUT BINDINGS (Ce ai în XAML) ---
    
    private DateTime _searchCheckIn = DateTime.Today;
    public DateTime SearchCheckIn
    {
        get => _searchCheckIn;
        set { _searchCheckIn = value; OnPropertyChanged(); }
    }

    private DateTime _searchCheckOut = DateTime.Today.AddDays(1);
    public DateTime SearchCheckOut
    {
        get => _searchCheckOut;
        set { _searchCheckOut = value; OnPropertyChanged(); }
    }

    private string _selectedRoomType;
    public string SelectedRoomType
    {
        get => _selectedRoomType;
        set { _selectedRoomType = value; OnPropertyChanged(); }
    }

    private bool _hasNoResults;
    public bool HasNoResults
    {
        get => _hasNoResults;
        set { _hasNoResults = value; OnPropertyChanged(); }
    }

    // Lista de camere
    public ObservableCollection<Room> AvailableRooms { get; set; } = new();

    // --- COMMANDS ---
    public ICommand SearchRoomsCommand { get; }
    public ICommand SelectRoomCommand { get; }

    // --- CONSTRUCTOR ---
    public CRoomSearchViewModel(Manager manager, Client client)
    {
        _manager = manager;
        _client = client;

        SearchRoomsCommand = new RelayCommand(ExecuteSearch);
        SelectRoomCommand = new RelayCommand(ExecuteBook);
        
        // Ascundem textul "Nu am găsit" la început
        HasNoResults = false;
    }

    private void ExecuteSearch(object parameter)
    {
        if (SearchCheckIn >= SearchCheckOut)
        {
            MessageBox.Show("Data de Check-Out trebuie să fie după Check-In!");
            return;
        }

        // 1. Golim lista veche
        AvailableRooms.Clear();

        // 2. Căutăm în Manager
        var rooms = _manager.FindAvailableRooms(SearchCheckIn, SearchCheckOut);

        // 3. Filtrăm după Tipul Camerei (dacă a fost selectat ceva în ComboBox)
        if (!string.IsNullOrEmpty(SelectedRoomType))
        {
            // Convertim string-ul din ComboBox ("Single Room") în Enum-ul RoomType
            // Sau facem o comparare simplă de string-uri
            rooms = rooms.Where(r => r.Type.ToString() + " Room" == SelectedRoomType || 
                                     r.Type.ToString() == SelectedRoomType).ToList();
        }

        // 4. Adăugăm în listă
        foreach (var room in rooms)
        {
            AvailableRooms.Add(room);
        }

        // 5. Actualizăm vizibilitatea mesajului de eroare
        HasNoResults = AvailableRooms.Count == 0;
    }

    private void ExecuteBook(object parameter)
    {
        if (parameter is Room selectedRoom)
        {
            bool success = _manager.CreateClientReservation(_client.Username, selectedRoom.Number, SearchCheckIn, SearchCheckOut);
            
            if (success)
            {
                MessageBox.Show($"Rezervare reușită pentru camera {selectedRoom.Number}!");
                AvailableRooms.Remove(selectedRoom); // O scoatem din listă
                
                // Opțional: Navigăm către "My Reservations"
            }
            else
            {
                MessageBox.Show("Eroare: Camera a fost rezervată de altcineva între timp.");
            }
        }
    }
}