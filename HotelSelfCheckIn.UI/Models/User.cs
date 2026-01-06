namespace HotelSelfCheckIn.UI.Models;

public abstract record User
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string AccessLevel { get; init; }

    protected User(string username, string password, string accessLevel)
    {
        Username = username;
        Password = password;
        AccessLevel = accessLevel;
    }
}