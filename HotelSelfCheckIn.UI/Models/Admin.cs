namespace HotelSelfCheckIn.UI.Models;

public record Admin: User
{
    //"accesul la funcționalități trebuie realizat doar după autentificare"
    public Admin(string username, string password) : base(username, password,"Full")
    { }
}