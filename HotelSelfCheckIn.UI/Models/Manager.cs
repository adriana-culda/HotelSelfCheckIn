namespace HotelSelfCheckIn.UI.Models;

using Microsoft.Extensions.Logging;

//practic e motorul de cautare or sum like that (mai multi admini pot folosi acest hotel manager + ii bun pt "clean code"
public class Manager
{
    //  atribute
    private List<Room> _rooms = new();
    private List<Reservation> _reservations = new();
    private HotelSettings _settings = new(); // Pentru regulile generale
    
    private readonly ILogger<Manager> _logger;
    //  constructor
    public Manager(ILogger<Manager> logger)
    {
        _logger = logger;
    }
    //PS: DOAR ADMINUL ARE VOIE SA MODIFICE!!!!
    
    // I. Administrare camere =======================================================================
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
        //Linia de mai sus cauta in lista _rooms camera/camerele care au numarul "roomNumber". Altfel returneaza NULL.
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
        //de verficat daca este ocupata camera (in caz de orice): adaugat!
        bool hasActiveBookings = _reservations.Any(res => res.RoomNumber == roomNumber && res.Status == ReservationStatus.Active);
        if (hasActiveBookings)
        {
            _logger.LogWarning("Adminul {Admin} a incercat sa stearga camera {Num} care este ocupata.", admin.Username, roomNumber);
            throw new InvalidOperationException($"Nu se poate sterge camera {roomNumber}. Aceasta are rezervari active.");
        }
        _rooms.Remove(existingRoom);
        _logger.LogInformation("Adminul {admin.Username} a sters camera nr. {Num}",admin.Username,roomNumber);
    }
    
    //B.Status
    public void SetRoomStatus(Admin admin,int roomNumber, RoomStatus newStatus)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room == null)
        {
            _logger.LogWarning("Camera nr. {Num} nu a fost gasita.", roomNumber);
            return;
        }
        var updatedRoom = room with { Status = newStatus };
        _rooms[_rooms.IndexOf(room)] = updatedRoom;
        _logger.LogInformation("Adminul {admin.Username} a modificat statusul camerei nr. {Num} in {newStatus}",admin.Username,roomNumber,newStatus);
    }
    //==================================================================================================================
    // II. Gestionare rezervari
    public IEnumerable<Reservation> GetActiveReservations(Admin admin) 
        => _reservations.Where(r => r.Status == ReservationStatus.Active).ToList();
    //Alternativa 
    /*
     public IEnumerable<Reservation> GetActiveReservations(Admin admin)
    {
        List<Reservation> rezults = new List<Reservation>();
        foreach (var r in _reservations)
        {
            if (r.Status == ReservationStatus.Active)
                {
                  rezults.Add(r);
                }
        }
    return rezults;
    }
     */
    
    //Istoric toate rezervarile care NU sunt ACTIVE
    // public IEnumerable<Reservation> GetHistoryReservations0(Admin admin) 
    //     => _reservations.Where(r => r.Status != ReservationStatus.Active).ToList();
    
    //Istoric toate rezervarile care sunt FINALIZATE
    public IEnumerable<Reservation> GetHistoryReservations(Admin admin) 
        => _reservations.Where(r => r.Status == ReservationStatus.Completed).ToList();
    
    //Alternativa 
    /*
     public IEnumerable<Reservation> GetHistoryReservations(Admin admin)
    {
        List<Reservation> filtered = new List<Reservation>();
        foreach (var r in _reservations)
        {
            //if (r.Status != ReservationStatus.Active)  //<- aici e modificarea, dar ne arata cele care doar nu sunt active
            if(r.Status == ReservationStatus.Completed) //daca ne cere doar cele complete
                {
                  filtered.Add(r);
                }
        }
    return filtered;
    }
     */

    // Modificare status
    public void ForceChangeStatus(Admin admin, Guid id, ReservationStatus newStatus)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == id);
        if (res != null)
        {
            var newRes = res with { Status = newStatus };
            _reservations[_reservations.IndexOf(res)] = newRes;
            _logger.LogInformation("Status schimbat manual de admin: {User}", admin.Username);
        }
    }
    //=======================================================================================================
    // III. Configurare reguli de check in/out, se pot modifica din HotelSettings!!!!
    public void UpdateCheckInRules(Admin admin,TimeSpan checkInStart, TimeSpan checkOutEnd)
    {
        _settings = _settings with { 
            CheckInStart = checkInStart, 
            CheckOutEnd = checkOutEnd 
        };
        _logger.LogInformation("Adminul {Admin} a actualizat regulile de check in/out.",admin.Username);
    }
    //=========================================================================================================
    //posibil de adaugat
    //a.verificare camera pt rezervare (deoarece camera poate fi libera acum si ocupata mai tarziu si nu stiu cat de bine merge cu unavailable)
    //b. validare rezervare
}

//Adminul este finalizat. Daca sunt probleme, sa-mi scrieti!!