using System;
using System.Windows; 
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AdminShellViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin;
    private readonly FileService _fileService;

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set
        {
            
            if (_currentView == value) return;
            
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public string AdminName => _currentAdmin.Username;
    public string CurrentDateTime => System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

    public ICommand LogoutCommand { get; }

    public AdminShellViewModel(Manager manager, Admin admin, FileService fileService)
    {
        _manager = manager;
        _currentAdmin = admin;
        _fileService = fileService;

        LogoutCommand = new RelayCommand(ExecuteLogout);

       
        try
        {
            CurrentView = new AdminViewModel(_manager, _currentAdmin);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la pornire Dashboard: {ex.Message}");
        }
    }

    // --- FUNCTIA DE NAVIGARE
    public void Navigate(string destination)
    {
        try
        {
            switch (destination)
            {
                case "Dashboard":
                    // 1. PROTECȚIE: Esti deja pe Dashboard? Atunci nu facem nimic.
                    if (CurrentView is AdminViewModel) return;
                    
                    // 2. Navigare
                    CurrentView = new AdminViewModel(_manager, _currentAdmin);
                    break;
                    
                case "Client":
                    if (CurrentView is ClientManagementViewModel) return;
                    CurrentView = new ClientManagementViewModel(_manager);
                    break;
                    
                case "Room":
                    if (CurrentView is RoomManagementViewModel) return;
                    CurrentView = new RoomManagementViewModel(_manager, _currentAdmin);
                    break;
                    
                case "Reservation":
                    if (CurrentView is ReservationManagementViewModel) return;
                    CurrentView = new ReservationManagementViewModel(_manager, _currentAdmin, _fileService);
                    break;
                    
                case "Setting":
                    if (CurrentView is SettingViewModel) return;
                    CurrentView = new SettingViewModel(_manager,_currentAdmin);
                    break;
                case "Clients":
                    CurrentView = new AClientListViewModel(_manager);
                    break;
            }
        }
        catch (Exception ex)
        {
            // 3. PROTECTIE CRASH:
            MessageBox.Show($"Nu s-a putut deschide pagina '{destination}'.\nEroare: {ex.Message}", "Eroare Navigare");
        }
    }

    private void ExecuteLogout(object parameter)
    {
        try 
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
        catch(Exception ex)
        {
             MessageBox.Show("Eroare la logout: " + ex.Message);
        }
    }
}