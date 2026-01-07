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
    //PS: DOAR ADMINUL ARE VOIE SA MODIFICE URMATOARELE!!!!
    
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
    //A.4. Afisare camere?
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
    //Adminul este finalizat. Daca sunt probleme, sa-mi scrieti!!
    
    public bool IsRoomAvailable(int roomNumber, DateTime start, DateTime end)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        //verificare stare + disponibilitate
        if (room == null || room.Status == RoomStatus.Unavailable) 
        {
            return false; 
        }
        //verificam daca exista alta rezervare in aceeasi perioada
        bool hasOverlap = _reservations.Any(res => 
            res.RoomNumber == roomNumber && 
            res.Status == ReservationStatus.Active && 
            start < res.EndDate && 
            end > res.StartDate);

        return !hasOverlap;
    }
    //METODE CLIENTI (am inteles ca trebuie sa fie in acelasi fisier </3)
    public IEnumerable<Room> SearchRooms(DateTime startDate, DateTime endDate, string? type=null, List<string>? requiredFeatures=null)
    {
        return _rooms.Where(room =>
        {
            bool isAvailable = IsRoomAvailable(room.Number, startDate, endDate);
            bool matchType = type == null ||room.Type.Equals(type, StringComparison.OrdinalIgnoreCase);
            bool matchFacilities = requiredFeatures == null || requiredFeatures.All(f => room.Facilities.Contains(f));
            return isAvailable && matchType && matchFacilities;
        }).ToList();
    }
    /*public void PerformCheckIn(Client client, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId && r.Username == client.Username);
    
        if (res == null) throw new Exception("Rezervarea nu a fost gasita.");
        if (DateTime.Now.Date != res.StartDate.Date) 
            throw new InvalidOperationException("Check in-ul se poate face doar în ziua inceperii rezervarii.");

        // Actualizarea statusului camerei + rezervarii
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with { Status = ReservationStatus.Active };
    
        // Setăm camera ca "Ocupată"
        //SetRoomStatus(Admin admin,res.RoomNumber, RoomStatus.Occupied);
    
        _logger.LogInformation("Clientul {User} a efectuat check-in pentru camera {Num}", client.Username, res.RoomNumber);
    }*/
    public void ClientRequestReservation(Client client,int roomNumber,DateTime startDate, DateTime endDate)
    {
        if(!IsRoomAvailable(roomNumber, startDate, endDate))
            throw new InvalidOperationException("Camera nu e valabila");
        var request= new Reservation(Guid.NewGuid(), client.Username, roomNumber, startDate, endDate,ReservationStatus.Pending);
        _reservations.Add(request);
        _logger.LogInformation("Clientul {User} a trimis o cerere de rezervare pentru Camera nr. {Num}",client.Username,roomNumber);
    }

    public void AdminConfirmReservation(Admin admin, Guid reservationId)
    {
        var pendingReservation = _reservations.FirstOrDefault(r => r.ReservationID == reservationId);
        if (pendingReservation == null || pendingReservation.Status != ReservationStatus.Pending)
        {
            _logger.LogWarning("Confirmare esuata. Rezervarea {Id} nu exista sau nu e in asteptare",reservationId);
            return;
        }
        int index = _reservations.IndexOf(pendingReservation);
        _reservations[index] = pendingReservation with {Status = ReservationStatus.Active};
        
        SetRoomStatus(admin, pendingReservation.RoomNumber, RoomStatus.Occupied);
        _logger.LogInformation("Adminul {Admin} a confirmat rezervarea pentru {User}. Camera {Num} este ocupata.", admin.Username, pendingReservation.Username, pendingReservation.RoomNumber);
    }
    
}

