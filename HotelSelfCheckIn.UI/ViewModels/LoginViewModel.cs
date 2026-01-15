using System;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models; 

namespace HotelSelfCheckIn.UI.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Action<User> _loginSuccessCallback; 
    
    private string _username;
    private string _errorMessage;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
            ErrorMessage = string.Empty; 
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(Manager manager, Action<User> onLoginSuccess)
    {
        _manager = manager;
        _loginSuccessCallback = onLoginSuccess;
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void ExecuteLogin(object parameter)
    {
        // 1. EXTRAGEM PAROLA DIN PARAMETRU 
        // Parameter este controlul PasswordBox trimis din XAML
        var passwordBox = parameter as PasswordBox;
        var password = passwordBox?.Password; // Extragem string-ul

        // Validare simpla
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Te rog introdu username si parola.";
            return;
        }

        // 2. Incercam autentificarea
        var user = _manager.Authenticate(Username, password);

        if (user == null)
        {
            ErrorMessage = "Username sau parola incorecta!";
            return;
        }

        // 3. SUCCES!
        // Doar apelam callback-ul. MainViewModel va decide ce fereastra sa deschida.
        _loginSuccessCallback?.Invoke(user);
    }
}