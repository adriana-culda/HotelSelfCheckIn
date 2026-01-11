using System;
using System.Windows;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.Views;

public partial class EditReservationWindow : Window
{
    public int NewRoomNumber { get; private set; }
    public DateTime NewStartDate { get; private set; }
    public DateTime NewEndDate { get; private set; }

    // Constructorul primește datele vechi ca să le afișeze
    public EditReservationWindow(Reservation existingRes)
    {
        InitializeComponent();

        txtUsername.Text = existingRes.Username;
        txtRoomNumber.Text = existingRes.RoomNumber.ToString();
        dpStart.SelectedDate = existingRes.StartDate;
        dpEnd.SelectedDate = existingRes.EndDate;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(txtRoomNumber.Text, out int room))
        {
            MessageBox.Show("Număr cameră invalid!"); return;
        }
        if (dpStart.SelectedDate >= dpEnd.SelectedDate)
        {
            MessageBox.Show("Check-Out trebuie să fie după Check-In!"); return;
        }

        NewRoomNumber = room;
        NewStartDate = dpStart.SelectedDate.Value;
        NewEndDate = dpEnd.SelectedDate.Value;

        DialogResult = true;
    }
}