namespace HotelSelfCheckIn.UI.Models;

// Camera Single
public record CameraSingle : Camera
{
    public override string Tip => "Single";
    public override double PretPeNoapte => 150.0;

    public CameraSingle(int numar, List<string> facilitati = null) 
        : base(numar, StatusCamera.Libera, facilitati) 
    { 
    }
}

// Camera Dubla
public record CameraDubla : Camera
{
    public override string Tip => "Dubla";
    public override double PretPeNoapte => 250.0;

    public CameraDubla(int numar, List<string> facilitati = null) 
        : base(numar, StatusCamera.Libera, facilitati) 
    { 
    }
}

// Camera Tripla
public record CameraTripla : Camera
{
    public override string Tip => "Tripla";
    public override double PretPeNoapte => 350.0;

    public CameraTripla(int numar, List<string> facilitati = null) 
        : base(numar, StatusCamera.Libera, facilitati) 
    { 
    }
}

// Camera Familiala
public record CameraFamiliala : Camera
{
    public override string Tip => "Familiala";
    public override double PretPeNoapte => 450.0;

    public CameraFamiliala(int numar, List<string> facilitati = null) 
        : base(numar, StatusCamera.Libera, facilitati) 
    { 
    }
}