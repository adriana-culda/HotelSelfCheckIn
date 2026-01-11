using System.Windows.Controls;
using System.Windows;

namespace HotelSelfCheckIn.UI.Views;

public partial class CBookingView : UserControl
{
    public CBookingView()
    {
        InitializeComponent();
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        dynamic data = this.DataContext;

        if (data != null)
        {
            //Luam numele direct din proprietate
            string name = data.GuestName; 
            MessageBox.Show($"Reservation confirmed for {name}!");
        }
        else
        {
            //Mesaj de siguranta
            MessageBox.Show("Reservation confirmed! (Data binding pending)");
        }
    }
}