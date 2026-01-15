using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.ViewModels;

namespace HotelSelfCheckIn.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly FileService _fileService; 
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }

    // Constructorul primeste acum si FileService
    public MainViewModel(Manager manager, FileService fileService)
    {
        _manager = manager;
        _fileService = fileService;

        CurrentView = new LoginViewModel(_manager, OnLoginSuccess);
    }

    private void OnLoginSuccess(User user)
    {
        if (user is Admin adminUser)
        {
            
            // Trimitem 4 parametri: Manager, Admin, FileService (pt codul tau vechi) si Action (pt logout)
            CurrentView = new AdminShellViewModel(_manager, adminUser, _fileService);
        }
        else if (user is Client clientUser)
        {
            // La client nu e nevoie de FileService
            CurrentView = new ClientShellViewModel(_manager, clientUser,OnLogout);
        }
    }

    private void OnLogout()
    {
        CurrentView = new LoginViewModel(_manager, OnLoginSuccess);
    }
}