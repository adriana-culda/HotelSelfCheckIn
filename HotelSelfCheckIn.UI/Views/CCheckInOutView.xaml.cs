using System.Windows.Controls;

namespace HotelSelfCheckIn.UI.Views;

public partial class CCheckInOutView : UserControl
{
    public CCheckInOutView()
    {
        InitializeComponent();
    }

    // Am eliminat metodele BtnCheckIn_Click și BtnCheckOut_Click.
    // Logica de verificare a codului și afișarea mesajelor vor fi
    // gestionate de colegul tău în CheckInOutViewModel.cs.
}