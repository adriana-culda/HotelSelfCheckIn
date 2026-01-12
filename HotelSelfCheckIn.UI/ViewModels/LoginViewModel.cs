using System;
using System.Windows.Controls; // Necesar pentru PasswordBox
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models; // Aici este clasa ta Manager

namespace HotelSelfCheckIn.UI.ViewModels;

public class LoginViewModel : ViewModelBase
{
    // FIX 1: Folosim clasa ta reală 'Manager', nu 'HotelManager'
    private readonly Manager _manager;
    private readonly Action<User> _loginSuccessCallback; // Funcția care schimbă pagina spre Admin
    
    private string _username;
    private string _errorMessage;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
            ErrorMessage = string.Empty; // Ștergem eroarea când userul scrie iar
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

    // Constructorul primește Managerul tău real
    public LoginViewModel(Manager manager, Action<User> onLoginSuccess)
    {
        _manager = manager;
        _loginSuccessCallback = onLoginSuccess;
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void ExecuteLogin(object parameter)
    {
        var passwordBox = parameter as PasswordBox;
        var password = passwordBox?.Password;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Te rog introdu username și parola.";
            return;
        }

        // FIX 2 (NOTA 10): Folosim logica ta reală din Manager!
        // Nu mai verificăm manual "admin/admin", ci apelăm funcția ta.
        var user = _manager.Authenticate(Username, password);

        if (user != null)
        {
            // Verificăm ce fel de utilizator este
            if (user is Admin)
            {
                // Este Admin -> Schimbăm pagina spre Tabel
                _loginSuccessCallback?.Invoke(user);
            }
            else if (user is Client)
            {
                // Este Client -> Deocamdată doar afișăm un mesaj (sau facem navigare spre ClientView mai târziu)
                ErrorMessage = "Logare Client reușită! (Interfața Client urmează)";
            }
        }
        else
        {
            ErrorMessage = "Username sau parolă incorectă!";
        }
    }
}