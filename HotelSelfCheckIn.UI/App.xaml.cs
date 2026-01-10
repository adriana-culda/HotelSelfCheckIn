using System.Collections.Generic; // <--- Asigură-te că ai acest using pentru List<Room>
using System.Windows;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.ViewModels;
using HotelSelfCheckIn.UI.Views;

namespace HotelSelfCheckIn.UI;

public partial class App : Application
{
    private MainWindow _mainWindow;
    private Manager _manager;
    private FileService _fileService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _fileService = new FileService();
        _manager = new Manager(); // Constructorul Managerului creează deja Admin-ul default

        try 
        {
            // 1. Încărcăm ce găsim pe disc
            var (camere, rezervari) = _fileService.Load(); 

            // --- SEED CAMERE (Dacă nu există) ---
            if (camere.Count == 0)
            {
                camere = new List<Room>
                {
                    new SingleRoom(101),
                    new DoubleRoom(102),
                    new SingleRoom(103) with { Status = RoomStatus.Occupied }, // Aceasta va avea o rezervare activă!
                    new FamilyRoom(104) with { Status = RoomStatus.Cleaning },
                    new TripleRoom(105),
                    new SingleRoom(106),
                    new DoubleRoom(107) with { Status = RoomStatus.Unavailable },
                    new FamilyRoom(108),
                    new SingleRoom(109),
                    new DoubleRoom(110)
                };
                _fileService.SaveRooms(camere);
            }

            // --- SEED REZERVĂRI (Dacă nu există) ---
            if (rezervari.Count == 0)
            {
                // Atenție: Trebuie să folosim numere de cameră care există deja (ex: 103)
                // și useri care există (ex: "client1" sau "admin")
                
                rezervari = new List<Reservation>
                {
                    // O rezervare ACTIVĂ pentru camera 103 (care e Occupied mai sus)
                    new Reservation(
                        Guid.NewGuid(), 
                        "client1", 
                        103, 
                        DateTime.Now.AddDays(-1), // A început ieri
                        DateTime.Now.AddDays(2),  // Se termină peste 2 zile
                        ReservationStatus.Active
                    ),

                    // O rezervare FINALIZATĂ (Istoric)
                    new Reservation(
                        Guid.NewGuid(), 
                        "client2", 
                        101, 
                        DateTime.Now.AddDays(-10), 
                        DateTime.Now.AddDays(-5), 
                        ReservationStatus.Completed
                    ),

                    // O rezervare VIITOARE (Pending)
                    new Reservation(
                        Guid.NewGuid(), 
                        "client1", 
                        105, 
                        DateTime.Now.AddDays(5), 
                        DateTime.Now.AddDays(7), 
                        ReservationStatus.Pending
                    )
                };

                // Salvăm pe disc -> se creează "rezervari.json"
                _fileService.SaveReservations(rezervari);
            }

            // 2. Încărcăm totul în Manager
            _manager.LoadData(camere, rezervari);
        }
        catch (Exception ex)
        { 
            // E bine să vezi eroarea dacă apare
            MessageBox.Show("Eroare la încărcarea datelor: " + ex.Message);
        }
        catch 
        { 
            // Dacă ceva crapă rău, aplicația continuă dar fără date
        }

        _mainWindow = new MainWindow();

        var loginVm = new LoginViewModel(_manager, OnUserLoggedIn);
        
        _mainWindow.Content = new LoginView { DataContext = loginVm };
        _mainWindow.Show();
    }

    private void OnUserLoggedIn(User user)
    {
        if (user is Admin adminLogat)
        {
            var shellVm = new AdminShellViewModel(_manager, adminLogat);
            _mainWindow.Content = new AdminShellView { DataContext = shellVm };
        }
        else if (user is Client clientLogat)
        {
            MessageBox.Show($"Bine ai venit, client {clientLogat.Username}!");
        }
    }
}