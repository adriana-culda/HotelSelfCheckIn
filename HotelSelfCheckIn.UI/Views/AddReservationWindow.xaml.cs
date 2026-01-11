using System;
using System.Windows;

namespace HotelSelfCheckIn.UI.Views; // Namespace-ul trebuie să fie corect

public partial class AddReservationWindow : Window
{
    // Proprietăți publice pe care le citim în ViewModel
    public string Username { get; private set; }
    public int RoomNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public AddReservationWindow()
    {
        InitializeComponent();
        
        // Setăm valori implicite (azi și mâine)
        dpStart.SelectedDate = DateTime.Now;
        dpEnd.SelectedDate = DateTime.Now.AddDays(1);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // 1. Validare simplă
        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtRoomNumber.Text))
        {
            MessageBox.Show("Completează toate câmpurile!", "Eroare");
            return;
        }

        if (!int.TryParse(txtRoomNumber.Text, out int room))
        {
            MessageBox.Show("Numărul camerei trebuie să fie un număr valid!", "Eroare");
            return;
        }

        if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
        {
            MessageBox.Show("Selectează datele de început și sfârșit!", "Eroare");
            return;
        }

        if (dpStart.SelectedDate >= dpEnd.SelectedDate)
        {
            MessageBox.Show("Data de Check-Out trebuie să fie după Check-In!", "Eroare");
            return;
        }

        // 2. Salvăm datele în proprietăți publice
        Username = txtUsername.Text;
        RoomNumber = room;
        StartDate = dpStart.SelectedDate.Value;
        EndDate = dpEnd.SelectedDate.Value;

        // 3. Închidem fereastra cu succes (return true)
        DialogResult = true;
    }
}