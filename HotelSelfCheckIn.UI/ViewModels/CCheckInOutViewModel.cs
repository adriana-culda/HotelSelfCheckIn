using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class CCheckInOutViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _client;

    private string _bookingCode;
    public string BookingCode
    {
        get => _bookingCode;
        set { _bookingCode = value; OnPropertyChanged(); }
    }

    public ICommand CheckInCommand { get; }
    public ICommand CheckOutCommand { get; }

    public CCheckInOutViewModel(Manager manager, Client client)
    {
        _manager = manager;
        _client = client;

        CheckInCommand = new RelayCommand(_ => ExecuteCheckIn());
        CheckOutCommand = new RelayCommand(_ => ExecuteCheckOut());
    }

    private void ExecuteCheckIn()
    {
        if (string.IsNullOrWhiteSpace(BookingCode)) return;

        // Cautam rezervarea care apartine clientului si are codul introdus (primele 8 caractere ale GUID-ului)
        var reservation = _manager.GetMyReservations(_client)
            .FirstOrDefault(r => r.ReservationID.ToString().ToUpper().StartsWith(BookingCode.ToUpper()));

        if (reservation == null)
        {
            MessageBox.Show("Invalid booking code or reservation not found.");
            return;
        }

        if (DateTime.Today < reservation.StartDate.Date)
        {
            MessageBox.Show($"Too early! Your check-in is scheduled for {reservation.StartDate:dd MMM yyyy}.");
            return;
        }

        // Actualizam statusul camerei In "Occupied" prin Manager
        
        _manager.SetRoomStatus(null, reservation.RoomNumber, RoomStatus.Occupied);
        
        MessageBox.Show($"Check-in successful! Welcome to room {reservation.RoomNumber}.");
    }

    private void ExecuteCheckOut()
    {
        if (string.IsNullOrWhiteSpace(BookingCode)) return;

        var reservation = _manager.GetMyReservations(_client)
            .FirstOrDefault(r => r.ReservationID.ToString().ToUpper().StartsWith(BookingCode.ToUpper()));

        if (reservation != null && reservation.Status == ReservationStatus.Active)
        {
            // 1. Eliberam camera (o punem In Cleaning sau Free)
            _manager.SetRoomStatus(null, reservation.RoomNumber, RoomStatus.Cleaning);

            // 2. Marcam rezervarea ca finalizata
            _manager.ForceChangeStatus(null, reservation.ReservationID, ReservationStatus.Completed);

            MessageBox.Show("Check-out successful! We hope you enjoyed your stay.");
            BookingCode = string.Empty;
        }
        else
        {
            MessageBox.Show("No active reservation found for this code.");
        }
    }
}