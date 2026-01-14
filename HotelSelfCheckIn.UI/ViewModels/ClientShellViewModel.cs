using System;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ClientShellViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _currentClient;
    private readonly Action _onLogout; // Aceasta este "telecomanda" către MainViewModel

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }

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
    }

    public void Navigate(string destination)
    {
        switch (destination)
        {
            case "Search": // Pentru butoanele tale care au Tag="Search"
            case "Reserve":
                CurrentView = new CRoomSearchViewModel(_manager, _currentClient);
                OnPropertyChanged(nameof(CurrentView));
                break;
            case "MyBookings":
                // CurrentView = new ClientBookingsViewModel(_manager, _currentClient);
                break;
        }
    }

    private void Logout()
    {
        // Invocăm acțiunea primită din MainViewModel
        _onLogout?.Invoke();
    }
}