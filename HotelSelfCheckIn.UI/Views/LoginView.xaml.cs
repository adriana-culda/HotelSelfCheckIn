using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    // AM ȘTERS BtnLogin_Click. 
    // Butonul din XAML trebuie să folosească Command="{Binding LoginCommand}"
}