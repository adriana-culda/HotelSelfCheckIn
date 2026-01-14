using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class SettingViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin; // Avem nevoie de admin pentru loguri/drepturi

    // --- PROPRIETĂȚI CU NOTIFICARE (OnPropertyChanged) ---
    // Dacă nu pui OnPropertyChanged, UI-ul nu se actualizează corect bidirecțional.

    private string _checkInTime;
    public string CheckInTime
    {
        get => _checkInTime;
        set { _checkInTime = value; OnPropertyChanged(); }
    }

    private string _checkOutTime;
    public string CheckOutTime
    {
        get => _checkOutTime;
        set { _checkOutTime = value; OnPropertyChanged(); }
    }

    private int _minDays;
    public int MinDays
    {
        get => _minDays;
        set { _minDays = value; OnPropertyChanged(); }
    }

    private string _generalRules;
    public string GeneralRules
    {
        get => _generalRules;
        set { _generalRules = value; OnPropertyChanged(); }
    }

    // Comanda pentru butonul "Salvează"
    public ICommand SaveSettingsCommand { get; }

    // --- CONSTRUCTOR ---
    public SettingViewModel(Manager manager, Admin admin)
    {
        _manager = manager;
        _currentAdmin = admin;

        // 1. Încărcăm datele curente din Manager
        var settings = _manager.GetSettings();

        // 2. Le punem în proprietățile vizibile (convertim TimeSpan la String "HH:mm")
        CheckInTime = settings.CheckInStart.ToString(@"hh\:mm");
        CheckOutTime = settings.CheckOutEnd.ToString(@"hh\:mm");
        MinDays = settings.MinReservationDays;
        

        // 3. Inițializăm comanda
        SaveSettingsCommand = new RelayCommand(ExecuteSave);
    }

    private void ExecuteSave(object parameter)
    {
        try
        {
            // Validare și Conversie (String -> TimeSpan)
            if (!TimeSpan.TryParse(CheckInTime, out TimeSpan inTime))
            {
                MessageBox.Show("Formatul orei Check-in invalid! Folosește HH:MM (ex: 14:00).");
                return;
            }

            if (!TimeSpan.TryParse(CheckOutTime, out TimeSpan outTime))
            {
                MessageBox.Show("Formatul orei Check-out invalid! Folosește HH:MM (ex: 11:00).");
                return;
            }

            if (MinDays < 1)
            {
                MessageBox.Show("Durata minimă trebuie să fie de cel puțin 1 zi.");
                return;
            }

            // Apelăm Managerul să salveze
            _manager.UpdateSettings(_currentAdmin, inTime, outTime, MinDays, GeneralRules);

            MessageBox.Show("Setările au fost salvate cu succes!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Eroare la salvare: " + ex.Message);
        }
    }
}