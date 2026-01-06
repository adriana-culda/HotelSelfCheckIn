namespace HotelSelfCheckIn.UI.Models;

public abstract record User
{
    public string Username { get; init; }
    public string Password { get; init; }

    protected User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}