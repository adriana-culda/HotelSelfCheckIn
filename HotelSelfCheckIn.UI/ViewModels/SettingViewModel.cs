using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class SettingViewModel : ViewModelBase
{
    private readonly Manager _manager;
    
    // Proprietăți legate de TextBox-uri
    public string CheckInTime { get; set; } = "14:00";
    public string CheckOutTime { get; set; } = "11:00";
    public int MinDays { get; set; } = 1;
    public string GeneralRules { get; set; } = "Fumatul interzis.";

    public SettingViewModel(Manager manager)
    {
        _manager = manager;
    }
    
    // Aici vei adăuga ICommand pentru butonul "Save"
}