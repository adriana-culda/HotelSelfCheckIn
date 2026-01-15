using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class CManageBookingsViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _client;
    private readonly ClientShellViewModel _shell;

    public ObservableCollection<ReservationDisplayItem> ActiveBookings { get; set; } = new();

    private bool _hasNoActiveBookings;
    public bool HasNoActiveBookings
    {
        get => _hasNoActiveBookings;
        set { _hasNoActiveBookings = value; OnPropertyChanged(); }
    }

    public ICommand ModifyBookingCommand { get; }
    public ICommand CancelBookingCommand { get; }

    public CManageBookingsViewModel(Manager manager, Client client, ClientShellViewModel shell)
    {
        _manager = manager;
        _client = client;
        _shell = shell;

        ModifyBookingCommand = new RelayCommand(ExecuteModify);
        CancelBookingCommand = new RelayCommand(ExecuteCancel);

        LoadBookings();
    }

    private void LoadBookings()
    {
        ActiveBookings.Clear();
        
        // Luăm rezervările de la manager pentru acest client
        var myReservations = _manager.GetMyReservations(_client)
                                     .Where(r => r.Status == ReservationStatus.Active);

        foreach (var res in myReservations)
        {
            ActiveBookings.Add(new ReservationDisplayItem
            {
                BookingId = res.ReservationID.ToString().Substring(0, 8).ToUpper(),
                FullId = res.ReservationID,
                RoomNumber = res.RoomNumber,
                StayInterval = $"{res.StartDate:dd MMM} - {res.EndDate:dd MMM yyyy}",
                // Tipul camerei îl luăm din lista de camere a managerului
                RoomType = "Room " + res.RoomNumber // Poți extinde logica să caute tipul exact (Single/Double)
            });
        }

        HasNoActiveBookings = ActiveBookings.Count == 0;
    }

    private void ExecuteCancel(object parameter)
    {
        if (parameter is ReservationDisplayItem item)
        {
            var result = MessageBox.Show($"Are you sure you want to cancel reservation for Room {item.RoomNumber}?", 
                                        "Cancel Booking", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try 
                {
                    _manager.CancelReservation(_client, item.FullId);
                    LoadBookings(); // Refresh listă
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }

    private void ExecuteModify(object parameter)
    {
        if (parameter is ReservationDisplayItem item)
        {
            //Trimite la Search, dar cu datele pre-completate
            _shell.CurrentView = new CRoomSearchViewModel(_manager, _client, _shell, item);
        }
    }
}

// Clasa helper pentru a potrivi Binding-ul din XAML-ul
public class ReservationDisplayItem
{
    public string BookingId { get; set; }
    public Guid FullId { get; set; }
    public int RoomNumber { get; set; }
    public string RoomType { get; set; }
    public string StayInterval { get; set; }
}