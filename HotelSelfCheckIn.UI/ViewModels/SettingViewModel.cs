using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class SettingViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin; 

    // --- PROPRIETATI CU NOTIFICARE (OnPropertyChanged) ---
    
    // Daca nu pui OnPropertyChanged, UI-ul nu se actualizeaza corect bidirectional.

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

    // Comanda pentru butonul "Salveaza"
    public ICommand SaveSettingsCommand { get; }

    // --- CONSTRUCTOR ---
    public SettingViewModel(Manager manager, Admin admin)
    {
        _manager = manager;
        _currentAdmin = admin;

        // 1. Incarcam datele curente din Manager
        var settings = _manager.GetSettings();

        // 2. Le punem In proprietatile vizibile (convertim TimeSpan la String "HH:mm")
        CheckInTime = settings.CheckInStart.ToString(@"hh\:mm");
        CheckOutTime = settings.CheckOutEnd.ToString(@"hh\:mm");
        MinDays = settings.MinReservationDays;
        

        // 3. Initializam comanda
        SaveSettingsCommand = new RelayCommand(ExecuteSave);
    }

    private void ExecuteSave(object parameter)
    {
        try
        {
            // Validare si Conversie (String -> TimeSpan)
            if (!TimeSpan.TryParse(CheckInTime, out TimeSpan inTime))
            {
                MessageBox.Show("Formatul orei Check-in invalid! Foloseste HH:MM (ex: 14:00).");
                return;
            }

            if (!TimeSpan.TryParse(CheckOutTime, out TimeSpan outTime))
            {
                MessageBox.Show("Formatul orei Check-out invalid! Foloseste HH:MM (ex: 11:00).");
                return;
            }

            if (MinDays < 1)
            {
                MessageBox.Show("Durata minima trebuie sa fie de cel putin 1 zi.");
                return;
            }

            // Apelam Managerul sa salveze
            _manager.UpdateSettings(_currentAdmin, inTime, outTime, MinDays, GeneralRules);

            MessageBox.Show("Setarile au fost salvate cu succes!");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Eroare la salvare: " + ex.Message);
        }
    }
}