namespace HotelSelfCheckIn.UI.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public class Manager
{
    // Referinta catre serviciul de fisiere
    private readonly FileService _fileService;

    // Listele de date
    private List<Room> _rooms = new();
    private List<Reservation> _reservations = new();
    private List<User> _users = new(); 
    private HotelSettings _settings = new();
    public HotelSettings GetSettings() => _settings;

    // ---------------------------------------------------------
    // CONSTRUCTORUL (Rezolva eroarea CS7036 daca e apelat corect)
    // ---------------------------------------------------------
    public Manager(FileService fileService)
    {
        _fileService = fileService;

        // Useri default (ca sa te poti loga)
        if (!_users.Any(u => u.Username == "admin")) 
            _users.Add(new Admin("admin", "admin"));
            
        if (!_users.Any(u => u.Username == "client"))
            _users.Add(new Client("client1", "client1"));
        if (!_users.Any(u => u.Username == "client"))
            _users.Add(new Client("client2", "client2"));
    }

    // Metoda privata pentru salvare automata
    private void SaveChanges()
    {
        _fileService.SaveRooms(_rooms);
        _fileService.SaveReservations(_reservations);
        _fileService.SaveSettings(_settings);
    }

    // =========================================================
    // METODE PENTRU AUTH & LOAD (Rezolva erorile CS1061)
    // =========================================================

    public User? Authenticate(string username, string password)
    {
        return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public void LoadData(List<Room> rooms, List<Reservation> reservations,List<User> users, HotelSettings settings)
    {
        _rooms = rooms ?? new List<Room>();
        _reservations = reservations ?? new List<Reservation>();
        _users = users ?? new List<User>();  
        if (!_users.Any())
        {
            _users.Add(new Admin("admin", "admin"));
            _users.Add(new Client("client1", "client1"));
            _users.Add(new Client("client2", "client2"));
        }
        
        // Daca settings e null (fisier lipsa), pastram default-ul, altfel luam ce am citit
        if (settings != null) 
        {
            _settings = settings;
        }
    }
    
    

    public void RegisterUser(User newUser) 
    {
        if (!_users.Any(u => u.Username == newUser.Username)) 
            _users.Add(newUser);
    }
    
    public List<Client> GetAllClients()
    {
        // OfType<Client>() face doua lucruri:
        // 1. Filtreaza doar obiectele care sunt de tip Client (ignora Adminii)
        // 2. Schimba tipul rezultatului In IEnumerable<Client>
        return _users.OfType<Client>().ToList(); 
    }
    

    // =========================================================
    // LOGICA DE BUSINESS (Camere & Rezervari)
    // =========================================================

    public void AddRoom(Admin admin, Room newRoom)
    {
        _rooms.Add(newRoom);
        SaveChanges(); 
    }
    
    public void UpdateRoom(Admin admin, int roomNumber, Room newRoom)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (existingRoom is null) throw new KeyNotFoundException($"Camera {roomNumber} nu a fost gasita.");
        
        int index = _rooms.IndexOf(existingRoom);
        _rooms[index] = newRoom;
        SaveChanges(); 
    }

    public void DeleteRoom(Admin admin, int roomNumber)
    {
        var existingRoom = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (existingRoom is null) throw new KeyNotFoundException($"Camera {roomNumber} nu a fost gasita.");
        
        bool hasActiveBookings = _reservations.Any(res => res.RoomNumber == roomNumber && res.Status == ReservationStatus.Active);
        if (hasActiveBookings) throw new InvalidOperationException($"Camera {roomNumber} are rezervari active.");
        
        _rooms.Remove(existingRoom);
        SaveChanges(); 
    }

    public IEnumerable<Room> GetAllRooms(Admin admin) => _rooms.AsReadOnly();

    public void SetRoomStatus(Admin admin, int roomNumber, RoomStatus newStatus)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room == null) return;

        Room updatedRoom = null;
        if(room is SingleRoom s) updatedRoom = s with { Status = newStatus };
        else if(room is DoubleRoom d) updatedRoom = d with { Status = newStatus };
        else if(room is FamilyRoom f) updatedRoom = f with { Status = newStatus };
        else if(room is TripleRoom t) updatedRoom = t with { Status = newStatus };
        
        if (updatedRoom != null) 
        {
            _rooms[_rooms.IndexOf(room)] = updatedRoom;
            SaveChanges(); 
        }
    }

    // --- REZERVaRI

    public void CreateReservation(Admin admin, string username, int roomNumber, DateTime start, DateTime end)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room == null) throw new KeyNotFoundException("Camera nu exista.");

        if (!IsRoomAvailable(roomNumber, start, end)) throw new InvalidOperationException("Camera nu este disponibila.");

        var newRes = new Reservation(Guid.NewGuid(), username, roomNumber, start, end, ReservationStatus.Active);
        _reservations.Add(newRes);
        
        if (start.Date <= DateTime.Now.Date && end.Date > DateTime.Now.Date)
        {
            SetRoomStatus(admin, roomNumber, RoomStatus.Occupied); 
        }
        else 
        {
            SaveChanges(); 
        }
    }
    
    public List<Reservation> GetAllReservations(Admin admin) => _reservations;

    public IEnumerable<Reservation> GetActiveReservations(Admin admin) 
        => _reservations.Where(r => r.Status == ReservationStatus.Active).ToList();
    
    public IEnumerable<Reservation> GetHistoryReservations(Admin admin) 
        => _reservations.Where(r => r.Status == ReservationStatus.Completed).ToList();

    public void AdminUpdateReservation(Admin admin, Guid reservationId, int newRoomNumber, DateTime newStart, DateTime newEnd)
    {
        var oldRes = _reservations.FirstOrDefault(r => r.ReservationID == reservationId);
        if (oldRes == null) throw new KeyNotFoundException("Rezervarea nu exista.");

        bool isAvailable = !_reservations.Any(other => 
            other.ReservationID != reservationId && 
            other.RoomNumber == newRoomNumber &&
            other.Status == ReservationStatus.Active &&
            newStart < other.EndDate &&
            newEnd > other.StartDate);

        if (!isAvailable) throw new InvalidOperationException("Camera nu este disponibila.");

        var newRes = oldRes with { RoomNumber = newRoomNumber, StartDate = newStart, EndDate = newEnd };
        _reservations[_reservations.IndexOf(oldRes)] = newRes;
        SaveChanges(); 
    }
    
    public void ForceChangeStatus(Admin admin, Guid id, ReservationStatus newStatus)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == id);
        if (res != null)
        {
            var newRes = res with { Status = newStatus };
            _reservations[_reservations.IndexOf(res)] = newRes;
            SaveChanges();
        }
    }

    public bool IsRoomAvailable(int roomNumber, DateTime start, DateTime end)
    {
        var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
        if (room == null || room.Status == RoomStatus.Unavailable) return false; 
        
        bool hasOverlap = _reservations.Any(res => 
            res.RoomNumber == roomNumber && 
            res.Status == ReservationStatus.Active && 
            start < res.EndDate && 
            end > res.StartDate);

        return !hasOverlap;
    }
    
    
    
    //---SETTINGS
    public void UpdateSettings(Admin admin, TimeSpan checkIn, TimeSpan checkOut, int minDays, string rules)
    {
        // Cream o instanta noua (pentru ca record-urile sunt imutabile sau pentru a Inlocui tot obiectul)
        _settings = new HotelSettings(checkIn, checkOut, minDays);
        
        // Salvam fizic In settings.json
        _fileService.SaveSettings(_settings); 
    }

    // --- CLIENT
    public IEnumerable<Room> GetRoomsForClient() => _rooms.Where(r => r.Status == RoomStatus.Free).ToList();
    
    public IEnumerable<Reservation> GetMyReservations(Client client) => _reservations.Where(r => r.Username == client.Username).ToList();
    
    

    public void CancelReservation(Client client, Guid reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.ReservationID == reservationId && r.Username == client.Username);
        if (res == null) throw new KeyNotFoundException("Rezervare inexistenta.");
        if(res.Status == ReservationStatus.Completed) throw new InvalidOperationException("Nu poti anula o rezervare finalizata.");
        
        int index = _reservations.IndexOf(res);
        _reservations[index] = res with {Status = ReservationStatus.Cancelled};
        SaveChanges(); 
    }
    //---UI CLIENT
    public List<Room> FindAvailableRooms(DateTime start, DateTime end)
    {
        // 1. Luam toate camerele care nu sunt "Unavailable" (scoase din uz)
        var candidateRooms = _rooms.Where(r => r.Status != RoomStatus.Unavailable).ToList();

        // 2. Filtram camerele care au deja rezervari active ce se suprapun cu perioada ceruta
        var availableRooms = candidateRooms.Where(room => 
        {
            bool hasOverlap = _reservations.Any(res => 
                res.RoomNumber == room.Number && 
                (res.Status == ReservationStatus.Active || res.Status == ReservationStatus.Pending) && 
                start < res.EndDate && 
                end > res.StartDate);
            
            return !hasOverlap;
        }).ToList();

        return availableRooms;
    }
    
    public bool CreateClientReservation(string username, int roomNumber, DateTime start, DateTime end)
    {
        try 
        {
            if (!IsRoomAvailable(roomNumber, start, end)) return false;

            var newRes = new Reservation(
                Guid.NewGuid(), 
                username, 
                roomNumber, 
                start, 
                end, 
                ReservationStatus.Active // Sau Pending, depinde de logica ta
            );

            _reservations.Add(newRes);
            SaveChanges();
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public void SaveUsers()
    {
        _fileService.SaveUsers(_users);
    }
}