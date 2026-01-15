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

    // Proprietățile pentru Binding-ul din XAML-ul tău
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
                // SpecialRequests = oldRes.SpecialRequests; // Dacă ai salvat asta în Rezervare
            }
        } 
        
        // Calcul preț total
        int nights = (checkOut.Date - checkIn.Date).Days;
        TotalAmount = (decimal)(room.PricePerNight * (nights > 0 ? nights : 1));

        ConfirmBookingCommand = new RelayCommand(_ => ExecuteConfirm());
    }

    private void ExecuteConfirm()
    {
        // Aici apelăm managerul pentru a salva rezervarea finală
        bool success = _manager.CreateClientReservation(_client.Username, SelectedRoom.Number, ArrivalDate, DepartureDate);

        if (success)
        {
            MessageBox.Show("Booking confirmed! Enjoy your stay.", "Success");
            // Opțional: Navighează înapoi la Search sau la My Reservations
        }
        else
        {
            MessageBox.Show("Something went wrong. Please try again.");
        }
    }
    private void ExecuteFinalize()
    {
        // 1. Actualizăm obiectul Client cu datele introduse în UI
        // Aceste date vin din TextBox-urile legate (Binding) la GuestName, GuestEmail, etc.
        _client.Name = GuestName;
        _client.Email = GuestEmail;
        _client.Phone = GuestPhone;

        // 2. Executăm logica de rezervare (existentă)
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

        // 3. Dacă rezervarea a reușit, salvăm și profilul actualizat al utilizatorului
        if (success)
        {
            _manager.SaveUsers(); // Aici se apelează metoda nouă
            MessageBox.Show("Booking confirmed! Your contact details have been saved to your profile.");
        
            // Navigăm înapoi la My Reservations
            _shell.Navigate("Manage");
        }
    }
}