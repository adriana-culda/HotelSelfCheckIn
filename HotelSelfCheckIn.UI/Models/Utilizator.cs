using System;

namespace HotelSelfCheckIn.UI.Models;


public enum RolUtilizator
{
    Admin,
    Client
}

public class Utilizator
{
    public string NumeUtilizator { get; set; } = string.Empty;
    public string Parola { get; set; } = string.Empty;
    public RolUtilizator Rol { get; set; }
}