namespace HotelSelfCheckIn.UI.Models;

public record HotelSettings
{
    //definire intervale check in/out
    public TimeSpan CheckInStart { get; init; } = new TimeSpan(14, 0, 0); //ex 14:00
    public TimeSpan CheckOutEnd { get; init; } = new TimeSpan(11, 0, 0);  //ex 11:00
}