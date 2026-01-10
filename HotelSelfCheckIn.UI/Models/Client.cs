namespace HotelSelfCheckIn.UI.Models;

public record Client: User
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public Client(string username, string password) : base(username, password,"None")
    { }
}