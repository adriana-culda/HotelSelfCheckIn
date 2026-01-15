namespace HotelSelfCheckIn.UI.Models;

public record HotelSettings
{
    public TimeSpan CheckInStart { get; init; }
    public TimeSpan CheckOutEnd { get; init; }
    
    // --- CaMPURI NOI PENTRU UI ---
    public int MinReservationDays { get; init; }
    

    // Constructor gol (default)
    public HotelSettings() 
    {
        CheckInStart = new TimeSpan(14, 0, 0); // 14:00
        CheckOutEnd = new TimeSpan(11, 0, 0);  // 11:00
        MinReservationDays = 1;
        
    }

    // Constructor complet (pentru salvare)
    public HotelSettings(TimeSpan checkIn, TimeSpan checkOut, int minDays)
    {
        CheckInStart = checkIn;
        CheckOutEnd = checkOut;
        MinReservationDays = minDays;
        
    }
}