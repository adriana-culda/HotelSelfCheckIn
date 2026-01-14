using System.Windows;
using System.Windows.Controls;
using HotelSelfCheckIn.UI.ViewModels;

namespace HotelSelfCheckIn.UI.Views;

public partial class ClientShellView : UserControl
{
    public ClientShellView()
    {
        InitializeComponent();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string destination)
        {
            /* Comentăm până apare fișierul în ViewModels
               if (DataContext is ClientShellViewModel vm)
               {
                   vm.Navigate(destination);
               }
            */
        
            // Temporar, poți pune un MessageBox ca să vezi că butonul funcționează
            // MessageBox.Show("Navigăm către: " + destination);
        }
    }
}