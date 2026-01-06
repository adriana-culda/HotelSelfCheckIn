namespace HotelSelfCheckIn.UI.Models;

//record clasa cu date imutabile, init este un fel de private set.
public abstract record Room
{
    public int Number { get; init; }
    public RoomStatus Status { get; init; }
    public IReadOnlyList<string> Facilities { get; init; }
    
    public abstract string Type { get; }
    public abstract double PricePerNight { get; }
    
    protected Room(int number, RoomStatus status, IReadOnlyList<string> facilities= null)
    {
        Number = number;
        Status = status;
        if (facilities != null)
        {
            Facilities = facilities;
        }
        else
        {
            Facilities = new List<string>().AsReadOnly();
        }
    }
    
}