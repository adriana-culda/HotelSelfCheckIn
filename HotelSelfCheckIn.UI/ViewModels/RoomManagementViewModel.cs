using System.Collections.ObjectModel;
using System.Collections.Generic; // Important
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class RoomManagementViewModel : ViewModelBase
{
    private readonly Manager _manager;
    public ObservableCollection<Room> Rooms { get; set; }

    // Constructorul trebuie să primească și Admin-ul (pentru permisiuni)
    public RoomManagementViewModel(Manager manager, Admin currentAdmin)
    {
        _manager = manager;

        // 1. Apelăm metoda corectă din Manager: GetAllRooms
        // 2. Rezolvăm ambiguitatea convertind explicit la IEnumerable<Room>
        IEnumerable<Room> listaCamere = _manager.GetAllRooms(currentAdmin);
        
        Rooms = new ObservableCollection<Room>(listaCamere);
    }
}