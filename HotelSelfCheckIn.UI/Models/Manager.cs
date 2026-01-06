namespace HotelSelfCheckIn.UI.Models;

using Microsoft.Extensions.Logging;

//practic e motorul de cautare or sum like that (mai multi admini pot folosi acest hotel manager + ii bun pt "clean code"
public class Manager
{
    private List<Room> _rooms=new();
    private List<Reservation> _reservations = new();
    private HotelSettings _settings = new(); // Pentru regulile generale
    
    private readonly ILogger<Manager> _logger;
    public Manager(ILogger<Manager> logger)
    {
        _logger = logger;
    }
    //DOAR ADMINUL ARE VOIE SA MODIFICE!!!!
    
    //Administrare camerelor
    public void AddRoom(Admin admin, Room newRoom)
    {
        //if(User is not Admin)
        _rooms.Add(newRoom);
        _logger.LogInformation($"Adminul {admin.Username} a adăugat camera {newRoom.Number}");
    }

    public void SetRoomStatus(int roomNumber, RoomStatus newStatus)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room != null)
        {
            var updatedRoom = room with { Status = newStatus };
            _rooms[_rooms.IndexOf(room)] = updatedRoom;
        }
    }

    // Gestionare rezervari
    public IEnumerable<Reservation> GetAllReservations() => _reservations.AsReadOnly();

    //Configurare reguli
    public void UpdateCheckInRules(TimeSpan checkInStart, TimeSpan checkOutEnd)
    {
        _settings = _settings with { 
            CheckInStart = checkInStart, 
            CheckOutEnd = checkOutEnd 
        };
        _logger.LogInformation("Regulile de check-in au fost actualizate.");
    }
}