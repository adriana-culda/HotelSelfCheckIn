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
    //==================================================================================================================
    //PS: DOAR ADMINUL ARE VOIE SA MODIFICE URMATOARELE!!!!
    
    // I. Administrare camere 
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
    //+ vizualizare camere
    public IEnumerable<Room> GetAllRooms(Admin admin) => _rooms.AsReadOnly();
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
    //__________________________________________________________________________________________________________________
    // II. Gestionare rezervari
    // A. Vizualizare rezervari active si istorice
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

    // B. Modificare status
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
    //__________________________________________________________________________________________________________________
    // III. Configurare reguli de check in/out, se pot modifica din HotelSettings!!!!
    public void UpdateCheckInRules(Admin admin,TimeSpan checkInStart, TimeSpan checkOutEnd)
    {
        _settings = _settings with { 
            CheckInStart = checkInStart, 
            CheckOutEnd = checkOutEnd 
        };
        _logger.LogInformation("Adminul {Admin} a actualizat regulile de check in/out.",admin.Username);
    }
    //==================================================================================================================
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
    //METODE CLIENTI====================================================================================================
    
    // I. Cautarea camerelor disponibile
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
    //+  Afisare camere disponibile (cu status free) 
    public IEnumerable<Room> GetRoomsForClient() 
        => _rooms.Where(r => r.Status == RoomStatus.Free).ToList();
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
    // II. Crearea unei rezervari
    // Model cerere aprobare client/admin pentru rezervari _____________________________________________________________
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
    //ADMIN- FINALIZARE REZERVARE (o baga in sistem ca si completed)
    public void AdminFinalizeReservation(Admin admin, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId);
        if (res != null)
        {
            _reservations[_reservations.IndexOf(res)] = res with {Status = ReservationStatus.Completed};
            SetRoomStatus(admin, res.RoomNumber, RoomStatus.Cleaning); //am pus sa fie imd valabila, pt proiect nu prea conteaza
            SetRoomStatus(admin, res.RoomNumber, RoomStatus.Free);
            _logger.LogInformation("Adminul a finalizat sejurul pentru rezervare {Id}",reservationId);
        }
    }
    //__________________________________________________________________________________________________________________
    // III. Check in/out -> marcare
    public void PerformCheckin(Client client, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId  && r.Username == client.Username);
        if (res == null) throw new KeyNotFoundException("Rezervare inexistenta");
        
        bool isCorrectDay = DateTime.Now.Date == res.StartDate.Date;
        bool isAfterCheckInHour = DateTime.Now.TimeOfDay >= _settings.CheckInStart;
        
        if(!isCorrectDay || !isAfterCheckInHour)
            throw new InvalidOperationException($"Check in permis doar in ziua de {res.StartDate.ToShortDateString()} dupa ora {_settings.CheckInStart}.");
        
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with {Status = ReservationStatus.Active};
        _logger.LogInformation("Clientul {User} a efectuat check in digital pentru camera nr.{Num}.",client.Username,reservationId);
    }

    public void PerformCheckout(Client client, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId && r.Username == client.Username);
        if (res == null) throw new KeyNotFoundException("Rezervare inexistenta");
        if (res.Status != ReservationStatus.Active) throw new InvalidOperationException("Nu se poate face checkout pentru o rezervare inactiva");
        
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with {Status = ReservationStatus.Completed};
        
        var room= _rooms.FirstOrDefault(r => r.Number == res.RoomNumber);
        if (room != null)
        {
            //_rooms[_rooms.IndexOf(room)] = room with {Status = RoomStatus.Cleaning};
            _rooms[_rooms.IndexOf(room)] = room with {Status = RoomStatus.Free};
        }
        _logger.LogInformation("Clientul {User} a efectuat check out digital", client.Username);
    }
    //__________________________________________________________________________________________________________________
    // IV. Gestionare rezervari personale
    
    // 1. Vizualizare rezervari
    public IEnumerable<Reservation> GetMyReservations(Client client) =>
        _reservations.Where(r => r.Username ==client.Username).ToList();
    // 2. Anulare rezervare
    public void CancelReservation(Client client, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId && r.Username == client.Username);
        if (res == null)
        {
            _logger.LogWarning("Clientul {User} a incercat sa anuleze o rezervare inexistenta", client.Username);
            throw new KeyNotFoundException("Rezervare inexistenta");
        }
        if(res.Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Nu ai voie sa anulezi o rezervare finalizata");
        
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with {Status = ReservationStatus.Cancelled};
        _logger.LogInformation("Clientul {User} si-a anulat rezervarea {Id}",client.Username,reservationId);
    }
    //3. Modificare rezervare
    public void UpdateReservation(Client client, Guid reservationId,DateTime newStart, DateTime newEnd)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId && r.Username == client.Username);
        if (res == null)
            throw new KeyNotFoundException("Rezervare inexistenta");
        
        bool isAvailable = !_reservations.Any(other => 
            other.ReservationID != reservationId && 
            other.RoomNumber == res.RoomNumber &&
            other.Status == ReservationStatus.Active &&
            newStart < other.EndDate &&
            newEnd > other.StartDate);
        
        if (!isAvailable)
            throw new KeyNotFoundException("Camera nu e disponibila");
        
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with {StartDate =  newStart, EndDate = newEnd};
        
        _logger.LogInformation("Clientul {User} si-a modificat perioada rezervarii {Id}",client.Username,reservationId);
    }
    
    // V. Istoric Sejururi ____________________________________________________________________________________________
    public IEnumerable<Reservation> GetMyHistory(Client client) =>
        _reservations.Where(r => r.Username == client.Username && r.Status == ReservationStatus.Completed).ToList();
}