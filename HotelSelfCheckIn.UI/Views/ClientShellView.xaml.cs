using System.Windows;
using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class ClientShellView : UserControl
{
    public ClientShellView()
    {
        InitializeComponent();
        ClientContentDisplay.Content = new CRoomSearchView();
    }
    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn == null || btn.Tag == null) return;

        switch (btn.Tag.ToString())
        {
            case "Search":
                ClientContentDisplay.Content = new CRoomSearchView();
                break;
            case "Book":
                ClientContentDisplay.Content = new CBookingView();
                break;
            case "CheckInOut":
                ClientContentDisplay.Content = new CCheckInOutView();
                break;
            case "Manage":
                ClientContentDisplay.Content = new CManageBookingsView();
                break;
            case "History":
                ClientContentDisplay.Content = new CStayHistoryView();
                break;
        }
    }
}