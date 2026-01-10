using System.Collections.ObjectModel;
using System.Collections.Generic;
using HotelSelfCheckIn.UI.Models;
using System.Linq; // Pentru .ToList() dacă e nevoie

namespace HotelSelfCheckIn.UI.ViewModels;

public class ReservationManagementViewModel : ViewModelBase
{
    // Aici lipsea definirea câmpului
    private readonly Manager _manager;
    
    public ObservableCollection<Reservation> Reservations { get; set; }

    public ReservationManagementViewModel(Manager manager, Admin currentAdmin)
    {
        _manager = manager;

        // Luăm rezervările active (sau istoricul, cum preferi)
        // Rezolvăm ambiguitatea explicit
        IEnumerable<Reservation> listaRezervari = _manager.GetActiveReservations(currentAdmin);
        
        Reservations = new ObservableCollection<Reservation>(listaRezervari);
    }
}