namespace HotelSelfCheckIn.UI.Models;

public abstract record Camera
{
    public int Numar { get; init; }
    public StatusCamera Status { get; init; }
    public IReadOnlyList<string> Facilitati { get; init; }
    
    public abstract string Tip { get; }
    public abstract double PretPeNoapte { get; }

    protected Camera(int numar, StatusCamera status, List<string> facilitati = null)
    {
        Numar = numar;
        Status = status;
        if (facilitati != null)
        {
            //transf in lista imutabila
            Facilitati = facilitati.AsReadOnly();
        }
        else
        {
            //cream o lista goala imutabila
            Facilitati = new List<string>().AsReadOnly();
        }
    }
}