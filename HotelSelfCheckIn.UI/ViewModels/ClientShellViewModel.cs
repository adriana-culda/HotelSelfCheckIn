using System;
using System.Windows.Input;
using System.Windows.Threading;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ClientShellViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _currentClient;
    private readonly Action _onLogout;
    private readonly DispatcherTimer _timer;

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }
    public string ClientName => _currentClient.Username;
    private string _currentDateTime;
    public string CurrentDateTime
    {
        get => _currentDateTime;
        set { _currentDateTime = value; OnPropertyChanged(); }
    }

    
    public string GuestName => _currentClient.Name;

    public ICommand NavigateReserveCommand { get; }
    public ICommand NavigateMyBookingsCommand { get; }
    public ICommand LogoutCommand { get; }

    // ADĂUGĂM Action onLogout ca al treilea parametru
    public ClientShellViewModel(Manager manager, Client client, Action onLogout)
    {
        _manager = manager;
        _currentClient = client;
        _onLogout = onLogout;

        NavigateReserveCommand = new RelayCommand(_ => Navigate("Reserve"));
        NavigateMyBookingsCommand = new RelayCommand(_ => Navigate("MyBookings"));
        
        LogoutCommand = new RelayCommand(_ => Logout());
        
        
        // Pagina de start implicită (asigură-te că proprietatea se numește CurrentView, nu CurrentViewModel)
        Navigate("Reserve");
        
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateTime();
        _timer.Start();
        
        UpdateTime();
    }
    private void UpdateTime()
    {
        // Format: Luni, 15 Ianuarie 2026 - 14:30:05
        CurrentDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy - HH:mm:ss");
    }

    public void Navigate(string destination)
    {
        switch (destination)
        {
            case "Search": // Pentru butoanele care au Tag="Search"
            case "Reserve":
                CurrentView = new CRoomSearchViewModel(_manager, _currentClient,this);
                OnPropertyChanged(nameof(CurrentView));
                break;
            case "Manage":
                CurrentView = new CManageBookingsViewModel(_manager, _currentClient,this);
                break;
            case "History":
                 CurrentView = new CStayHistoryViewModel(_manager, _currentClient);
                break;
            case "CheckInOut": 
                CurrentView = new CCheckInOutViewModel(_manager, _currentClient);
                break;
        }
    }

    private void Logout()
    {
         
        // Oprim timer-ul la logout
        _timer.Stop();        
        
        // Invocam actiunea primita din MainViewModel
        _onLogout?.Invoke();
    }
}