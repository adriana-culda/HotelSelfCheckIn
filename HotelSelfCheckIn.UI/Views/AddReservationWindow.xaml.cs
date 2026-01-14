using System.Windows;

namespace HotelSelfCheckIn.UI.Views
{
    public partial class AddReservationWindow : Window
    {
        public AddReservationWindow()
        {
            InitializeComponent();
            // WindowStartupLocation-ul stabilit în XAML (CenterScreen) 
            // se va ocupa de poziționare.
        }

        // Metodă pentru a închide fereastra din cod dacă este nevoie
        private void CloseWindow()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}