using System.Windows;

namespace HotelSelfCheckIn.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // AM ȘTERS: ViewContainer.Content = new LoginView(); 
        // Motiv: DataBinding-ul din XAML se ocupă acum de asta.
    }
}