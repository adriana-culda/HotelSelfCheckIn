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
        // 1. Identificăm butonul apăsat
        if (sender is Button btn && btn.Tag is string tag)
        {
            // 2. Obținem ViewModel-ul din spatele ferestrei
            if (this.DataContext is ClientShellViewModel vm)
            {
                // 3. Apelăm metoda de navigare
                vm.Navigate(tag);
            }
        }
    }
}