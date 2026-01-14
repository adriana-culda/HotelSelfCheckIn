using System;
using System.Collections.Generic;
using System.Windows;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.ViewModels;
using HotelSelfCheckIn.UI.Views;

namespace HotelSelfCheckIn.UI;

public partial class App : Application
{
    private FileService _fileService;
    private Manager _manager;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Inițializare Backend
        _fileService = new FileService();
        _manager = new Manager(_fileService);

        try 
        {
            // 2. Încărcare Date + Seed Data (Codul tău bun)
            var (camere, rezervari) = _fileService.Load();
            bool fisiereExistaC = System.IO.File.Exists("SavedData/camere.json");
            bool fisiereExistaR = System.IO.File.Exists("SavedData/rezervari.json");
            var setari = _fileService.LoadSettings();

            if (!fisiereExistaC && camere.Count == 0)
            {
                camere = new List<Room>
                {
                    new SingleRoom(101), new DoubleRoom(102), new SingleRoom(103) with { Status = RoomStatus.Occupied },
                    new FamilyRoom(104) with { Status = RoomStatus.Cleaning }, new TripleRoom(105), new SingleRoom(106),
                    new DoubleRoom(107) with { Status = RoomStatus.Unavailable }, new FamilyRoom(108), new SingleRoom(109), new DoubleRoom(110)
                };
                _fileService.SaveRooms(camere);
            }

            if (!fisiereExistaR && rezervari.Count == 0)
            {
                rezervari = new List<Reservation>
                {
                    new Reservation(Guid.NewGuid(), "client1", 103, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(2), ReservationStatus.Active),
                    new Reservation(Guid.NewGuid(), "client2", 101, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5), ReservationStatus.Completed),
                    new Reservation(Guid.NewGuid(), "client1", 105, DateTime.Now.AddDays(5), DateTime.Now.AddDays(7), ReservationStatus.Pending)
                };
                _fileService.SaveReservations(rezervari);
            }

            _manager.LoadData(camere, rezervari, setari);
        }
        catch (Exception ex)
        { 
            MessageBox.Show("Eroare la încărcarea datelor: " + ex.Message);
        }

        // 3. PORNIRE APLICAȚIE (MVVM PUR)
        // Nu mai facem nicio logică manuală aici. MainViewModel se ocupă de tot.
        
        var mainWindow = new MainWindow();
        var mainViewModel = new MainViewModel(_manager,_fileService); // MainViewModel va porni automat cu Login
        
        mainWindow.DataContext = mainViewModel; // Legătura magică
        mainWindow.Show();
    }
}