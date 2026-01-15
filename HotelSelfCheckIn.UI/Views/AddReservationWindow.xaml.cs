using System.Windows;

namespace HotelSelfCheckIn.UI.Views
{
    public partial class AddReservationWindow : Window
    {
        public AddReservationWindow()
        {
            InitializeComponent();
           
        }

        
        private void CloseWindow()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}