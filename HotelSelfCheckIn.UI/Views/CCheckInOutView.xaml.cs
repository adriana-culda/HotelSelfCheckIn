using System.Windows;
using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class CCheckInOutView : UserControl
{
    public CCheckInOutView()
    {
        InitializeComponent();
    }

    private void BtnCheckIn_Click(object sender, RoutedEventArgs e)
    {
        //luam codul din DataContext prin Binding
        dynamic data = this.DataContext;
        
        if (data != null && !string.IsNullOrEmpty(data.BookingCode))
        {
            //aici va veni logica de verificare 
            MessageBox.Show($"Processing Check-in for code: {data.BookingCode}");
        }
        else
        {
            MessageBox.Show("Please enter a valid booking code.");
        }
    }

    private void BtnCheckOut_Click(object sender, RoutedEventArgs e)
    {
        dynamic data = this.DataContext;

        if (data != null && !string.IsNullOrEmpty(data.BookingCode))
        {
            //logica pentru plecare
            MessageBox.Show($"Processing Check-out for code: {data.BookingCode}");
        }
        else
        {
            MessageBox.Show("Please enter a valid booking code.");
        }
    }
}