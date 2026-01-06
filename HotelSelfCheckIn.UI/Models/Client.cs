namespace HotelSelfCheckIn.UI.Models;

public record Client: User
{
    public Client(string username, string password) : base(username, password,"None")
    { }
}