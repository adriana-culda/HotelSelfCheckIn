using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class CBookingViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _client;
    private readonly ReservationDisplayItem _resToUpdate;
    private readonly ClientShellViewModel _shell;

    // Proprietatile pentru Binding-ul din XAML-ul tau
    public Room SelectedRoom { get; }
    public DateTime ArrivalDate { get; }
    public DateTime DepartureDate { get; }

    public string GuestName { get; set; }
    public string GuestEmail { get; set; }
    public string GuestPhone { get; set; }
    public string SpecialRequests { get; set; }

    public decimal TotalAmount { get; }

    public ICommand ConfirmBookingCommand { get; }

    public CBookingViewModel(Manager manager, Client client, Room room, DateTime checkIn, DateTime checkOut,ReservationDisplayItem resToUpdate = null)
    {
        _manager = manager;
        _client = client;
        _resToUpdate = resToUpdate;
        SelectedRoom = room;
        ArrivalDate = checkIn;
        DepartureDate = checkOut;
        
        if (_resToUpdate != null)
        {
            var oldRes = _manager.GetMyReservations(_client)
                .FirstOrDefault(r => r.ReservationID == _resToUpdate.FullId);
            if (oldRes != null)
            {
                // SpecialRequests = oldRes.SpecialRequests; 
            }
        } 
        
        // Calcul pret total
        int nights = (checkOut.Date - checkIn.Date).Days;
        TotalAmount = (decimal)(room.PricePerNight * (nights > 0 ? nights : 1));

        ConfirmBookingCommand = new RelayCommand(_ => ExecuteFinalize());
    }

    private void ExecuteConfirm()
    {
        // Aici apelam managerul pentru a salva rezervarea finala
        bool success = _manager.CreateClientReservation(_client.Username, SelectedRoom.Number, ArrivalDate, DepartureDate);

        if (success)
        {
            MessageBox.Show("Booking confirmed! Enjoy your stay.", "Success");
            // Optional: Navigheaza Inapoi la Search sau la My Reservations
        }
        else
        {
            MessageBox.Show("Something went wrong. Please try again.");
        }
    }

    private void ExecuteFinalize()
    {
        // 1. Transferam datele de pe ecran In obiectul Client
        _client.Name = GuestName;
        _client.Email = GuestEmail;
        _client.Phone = GuestPhone;

        // 2. Executam rezervarea
        bool success;
        if (_resToUpdate != null)
        {
            _manager.CancelReservation(_client, _resToUpdate.FullId);
            success = _manager.CreateClientReservation(_client.Username, SelectedRoom.Number, ArrivalDate, DepartureDate);
        }
        else
        {
            success = _manager.CreateClientReservation(_client.Username, SelectedRoom.Number, ArrivalDate, DepartureDate);
        }

        // 3. Salvam datele permanent
        if (success)
        {
            // Aceasta scrie In SavedData/users.json prin FileService
            _manager.SaveUsers(); 

            MessageBox.Show("Booking confirmed! Your profile has been updated.", "Success");

            // Ne Intoarcem la lista de rezervari
            _shell?.Navigate("Manage");
        }
    }
}