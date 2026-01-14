namespace HotelSelfCheckIn.UI.Models;

public record SingleRoom : Room
{
    public override string Type => "Single";
    public override double PricePerNight => 150.0;

    public SingleRoom(int number, List<string> facilities = null) 
        : base(number, RoomStatus.Free, facilities) {}
    public SingleRoom() : base(0, RoomStatus.Free) { }
}

public record DoubleRoom : Room
{
    public override string Type => "Double";
    public override double PricePerNight => 250.0;

    public DoubleRoom(int number, List<string> facilities = null) 
        : base(number, RoomStatus.Free, facilities) {}
    public DoubleRoom() : base(0, RoomStatus.Free) { }
}
public record TripleRoom : Room
{
    public override string Type => "Triple";
    public override double PricePerNight => 350.0;

    public TripleRoom(int number, List<string> facilities = null) 
        : base(number, RoomStatus.Free, facilities) {}
    public TripleRoom() : base(0, RoomStatus.Free) { }
}
public record FamilyRoom : Room
{
    public override string Type => "Family";
    public override double PricePerNight => 450.0;

    public FamilyRoom(int number, List<string> facilities = null) 
        : base(number, RoomStatus.Free, facilities) {}
    public FamilyRoom() : base(0, RoomStatus.Free) { }
}