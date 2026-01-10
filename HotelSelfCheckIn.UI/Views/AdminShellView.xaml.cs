using System.Windows;
using System.Windows.Controls;
using HotelSelfCheckIn.UI.ViewModels;

namespace HotelSelfCheckIn.UI.Views;

public partial class AdminShellView : UserControl
{
    public AdminShellView()
    {
        InitializeComponent();
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        // 1. Aflăm ce buton s-a apăsat
        if (sender is Button btn && btn.Tag is string destination)
        {
            // 2. Accesăm ViewModel-ul din spate
            if (DataContext is AdminShellViewModel vm)
            {
                // 3. Îi spunem să schimbe pagina
                vm.Navigate(destination);
            }
        }
    }
}