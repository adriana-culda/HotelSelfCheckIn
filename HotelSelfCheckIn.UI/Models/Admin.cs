namespace HotelSelfCheckIn.UI.Models;

public record Admin: User
{
    
    public Admin(string username, string password) : base(username, password,"Full")
    { }
}