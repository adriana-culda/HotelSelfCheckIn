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
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public string AdminName => _currentAdmin.Username;
    public string CurrentDateTime => System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

    public AdminShellViewModel(Manager manager, Admin admin,FileService fileService)
    {
        _manager = manager;
        _currentAdmin = admin;
        _fileService = fileService;

        // Pornim implicit cu Dashboard-ul (AdminViewModel-ul vechi)
        CurrentView = new AdminViewModel(_manager, _currentAdmin);
    }

    // Metodă apelată de butoane pentru a schimba pagina
    public void Navigate(string destination)
    {
        switch (destination)
        {
            case "Dashboard":
                // AdminViewModel cerea deja admin, e ok
                CurrentView = new AdminViewModel(_manager, _currentAdmin);
                break;
                
            case "Client":
                // Aici nu am modificat constructorul, rămâne simplu (sau îl actualizezi și pe el)
                CurrentView = new ClientManagementViewModel(_manager);
                break;
                
            case "Room":
                // MODIFICARE: Acum pasăm și _currentAdmin!
                CurrentView = new RoomManagementViewModel(_manager, _currentAdmin);
                break;
                
            case "Reservation":
                // Aici trimitem manager, admin ȘI fileService
                CurrentView = new ReservationManagementViewModel(_manager, _currentAdmin, _fileService);
                break;
                
            case "Setting":
                CurrentView = new SettingViewModel(_manager);
                break;
        }
    }
}