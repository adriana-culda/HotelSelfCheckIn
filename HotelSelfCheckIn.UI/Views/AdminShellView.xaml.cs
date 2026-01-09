using System.Windows;
using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class AdminShellView : UserControl
{
    public AdminShellView()
    {
        InitializeComponent();
        AdminContentDisplay.Content = new AdminView();
    }
    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn == null || btn.Tag == null) return;

        switch (btn.Tag.ToString())
        {
            case "Dashboard":
                AdminContentDisplay.Content = new AdminView();
                break;
            case "Client":
                AdminContentDisplay.Content = new ClientManagementView();
                break;
            case "Room":
                AdminContentDisplay.Content = new RoomManagementView();
                break;
            case "Reservation":
                AdminContentDisplay.Content = new ReservationManagementView();
                break;
            case "Setting":
                AdminContentDisplay.Content = new SettingView();
                break;
        }
    }
}