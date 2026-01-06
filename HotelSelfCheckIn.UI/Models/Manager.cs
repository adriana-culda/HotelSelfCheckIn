namespace HotelSelfCheckIn.UI.Models;

using Microsoft.Extensions.Logging;

//practic e motorul de cautare or sum like that (mai multi admini pot folosi acest hotel manager + ii bun pt "clean code"
public class Manager
{
    private List<Room> _rooms = new();
    private List<Reservation> _reservations = new();
    private HotelSettings _settings = new(); // Pentru regulile generale
    
    private readonly ILogger<Manager> _logger;
    public Manager(ILogger<Manager> logger)
    {
        _logger = logger;
    }
    //DOAR ADMINUL ARE VOIE SA MODIFICE!!!!
    
    //Administrare camere =======================================================================
    //A.1. creare
    public void AddRoom(Admin admin, Room newRoom)
    {
        _rooms.Add(newRoom);
        _logger.LogInformation($"Adminul {admin.Username} a adaugat camera {newRoom.Number}");
    }
    
    //A.2. modificare
    public void UpdateRoom(Admin admin,int roomNumber, Room newRoom)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (existingRoom is null)
        {
            _logger.LogWarning("Camera nr. {Num} nu exista.", roomNumber);
            throw new KeyNotFoundException($"Camera nr. {roomNumber} nu a fost gasita.");
        }
        int index = _rooms.IndexOf(existingRoom);
        _rooms[index] = newRoom;
        _logger.LogInformation("Adminul {admin.Username} a actualizat camera nr. {Num}",admin.Username,roomNumber);
    }
    //A.3. stergere 
    public void DeleteRoom(Admin admin, int roomNumber)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (existingRoom is null)
        {
            _logger.LogWarning("Camera nr. {Num} nu exista.", roomNumber);
            throw new KeyNotFoundException($"Camera nr. {roomNumber} nu a fost gasita.");
        }
        //de verficat daca este ocupata camera (in caz de orice)
        _rooms.Remove(existingRoom);
        _logger.LogInformation("Adminul {admin.Username} a sters camera nr. {Num}",admin.Username,roomNumber);
    }
    
    //B.Status
    public void SetRoomStatus(int roomNumber, RoomStatus newStatus)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room != null)
        {
            var updatedRoom = room with { Status = newStatus };
            _rooms[_rooms.IndexOf(room)] = updatedRoom;
        }
    }
    //==================================================================================================================
    // Gestionare rezervari
    public IEnumerable<Reservation> GetAllReservations() => _reservations.AsReadOnly();

    //Configurare reguli, se pot modifica din HotelSettings!!!
    public void UpdateCheckInRules(TimeSpan checkInStart, TimeSpan checkOutEnd)
    {
        _settings = _settings with { 
            CheckInStart = checkInStart, 
            CheckOutEnd = checkOutEnd 
        };
        _logger.LogInformation("Regulile de check-in au fost actualizate.");
    }
}