namespace HotelSelfCheckIn.UI.Models;

public record Admin: User
{
    public Admin(string username, string password) : base(username, password)
    { }
    //"iar accesul la funcționalități trebuie realizat doar după autentificare"
    public string AccessLevel { get; init; } = "Full";
}