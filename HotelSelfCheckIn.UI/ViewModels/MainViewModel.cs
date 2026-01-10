namespace HotelSelfCheckIn.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    // Aici ținem minte pe ce ecran suntem
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    // Constructorul primește primul ecran (Login)
    public MainViewModel(ViewModelBase initialViewModel)
    {
        CurrentViewModel = initialViewModel;
    }
    
    // Metodă ca să schimbăm ecranul din exterior
    public void NavigateTo(ViewModelBase newViewModel)
    {
        CurrentViewModel = newViewModel;
    }
}