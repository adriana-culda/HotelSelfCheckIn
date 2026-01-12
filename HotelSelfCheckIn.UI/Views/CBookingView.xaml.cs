using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class CBookingView : UserControl
{
    public CBookingView()
    {
        InitializeComponent();
    }

    // Am eliminat metoda BtnConfirm_Click pentru că logica 
    // se mută în ViewModel (prin ConfirmBookingCommand).
    // Astfel, fișierul rămâne "curat", specific arhitecturii MVVM.
}