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
    private HotelSettings _settings;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Inițializare Backend
        _fileService = new FileService();
        _manager = new Manager(_fileService);

        try 
        {
            try 
            {
                var (camere, rezervari) = _fileService.Load();
                var setari = _fileService.LoadSettings();
                _manager.LoadData(camere, rezervari, setari);
            }
            catch (Exception ex)
            { 
                _manager.LoadData(new List<Room>(), new List<Reservation>(), new HotelSettings());
                MessageBox.Show("Sistemul a pornit cu o bază de date goală. Eroare: " + ex.Message);
            }
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