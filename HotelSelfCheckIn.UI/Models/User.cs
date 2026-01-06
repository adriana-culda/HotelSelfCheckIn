namespace HotelSelfCheckIn.UI.Models;

public class User
{
    public string Username { get; }
    public string Password { get; }

    protected User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}