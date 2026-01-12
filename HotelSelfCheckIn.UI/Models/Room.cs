using System.Text.Json.Serialization; // Adaugă acest using

namespace HotelSelfCheckIn.UI.Models;
//Pt. transf din fisier JSON in C#
//Room nu poate fi creat direct (ii abstract) => trebuie sa dam o eticheta
[JsonDerivedType(typeof(SingleRoom), typeDiscriminator: "single")]
[JsonDerivedType(typeof(DoubleRoom), typeDiscriminator: "double")]
[JsonDerivedType(typeof(TripleRoom), typeDiscriminator: "triple")]
[JsonDerivedType(typeof(FamilyRoom), typeDiscriminator: "family")]

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