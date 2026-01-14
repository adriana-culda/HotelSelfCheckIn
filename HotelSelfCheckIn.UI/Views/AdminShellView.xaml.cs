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
       
        if (sender is Button clickedButton)
        {
          
            string destination = clickedButton.Tag?.ToString();

            
            if (DataContext is AdminShellViewModel vm && !string.IsNullOrEmpty(destination))
            {
                
                vm.Navigate(destination);
            }
        }
    }
}