using System.Windows;
using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        
    }
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;
    
        if (mainWindow != null)
        {
            mainWindow.ViewContainer.Content = new AdminShellView();
        }
    }
}