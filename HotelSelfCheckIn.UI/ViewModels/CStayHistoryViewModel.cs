using System;
using System.Collections.ObjectModel;
using System.Linq;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class CStayHistoryViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Client _client;

    public ObservableCollection<HistoryItem> PastStays { get; set; } = new();

    private bool _hasNoHistory;
    public bool HasNoHistory
    {
        get => _hasNoHistory;
        set { _hasNoHistory = value; OnPropertyChanged(); }
    }

    public CStayHistoryViewModel(Manager manager, Client client)
    {
        _manager = manager;
        _client = client;
        LoadHistory();
    }

    private void LoadHistory()
    {
        PastStays.Clear();
        
        // Luam toate rezervarile si le filtram pe cele care NU sunt Active
        var history = _manager.GetMyReservations(_client)
            .Where(r => r.Status == ReservationStatus.Completed || 
                        r.Status == ReservationStatus.Cancelled);

        foreach (var res in history)
        {
            // Calculam pretul informativ (ar putea fi stocat In Reservation pe viitor)
            // Cautam camera sa vedem pretul ei actual sau folosim o valoare default
            decimal pricePerNight = 100; // Valoare fallback

            int nights = (res.EndDate.Date - res.StartDate.Date).Days;
            decimal total = pricePerNight * (nights > 0 ? nights : 1);

            PastStays.Add(new HistoryItem
            {
                Date = res.StartDate,
                RoomName = $"Room {res.RoomNumber}",
                TotalPaid = total,
                Status = res.Status.ToString()
            });
        }

        HasNoHistory = PastStays.Count == 0;
    }
}

public class HistoryItem
{
    public DateTime Date { get; set; }
    public string RoomName { get; set; }
    public decimal TotalPaid { get; set; }
    public string Status { get; set; }
}