using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;       // Pentru MessageBox
using System.Windows.Input; // Pentru ICommand
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.Views; // Aici este AddReservationWindow
using System.Linq;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ReservationManagementViewModel : ViewModelBase
{
    private readonly Manager _manager;
    private readonly Admin _currentAdmin;
    private readonly FileService _fileService; // <--- Avem nevoie de el pentru salvare

    // Folosim o proprietate cu 'backing field' și OnPropertyChanged ca să se actualizeze UI-ul corect
    private ObservableCollection<Reservation> _reservations;
    public ObservableCollection<Reservation> Reservations
    {
        get => _reservations;
        set
        {
            _reservations = value;
            OnPropertyChanged();
        }
    }
    
    // Comanda legată de butonul din XAML
    public ICommand AddReservationCommand { get; }

    // Constructorul cere acum și FileService
    public ReservationManagementViewModel(Manager manager, Admin currentAdmin, FileService fileService)
    {
        _manager = manager;
        _currentAdmin = currentAdmin;
        _fileService = fileService;

        // Încărcăm lista inițială
        RefreshList();
        
        // Configurăm butonul să apeleze funcția ExecuteAddReservation
        AddReservationCommand = new RelayCommand(ExecuteAddReservation);
    }

    private void ExecuteAddReservation(object parameter)
    {
        // 1. Deschidem fereastra de adăugare (Pop-up)
        var window = new AddReservationWindow();
        
        // 2. Așteptăm să vedem dacă userul a dat "Save" (DialogResult == true)
        if (window.ShowDialog() == true)
        {
            try
            {
                // 3. Încercăm să creăm rezervarea în Manager (folosind datele din fereastră)
                _manager.CreateReservation(
                    _currentAdmin, 
                    window.Username, 
                    window.RoomNumber, 
                    window.StartDate, 
                    window.EndDate
                );

                // 4. Dacă a mers și nu a dat eroare (ex: cameră ocupată), SALVĂM PE DISC
                // Luăm TOATE rezervările din manager și le scriem în JSON
                var listaCompleta = _manager.GetAllReservations(_currentAdmin);
                _fileService.SaveReservations(listaCompleta);

                // 5. Reîmprospătăm tabelul din interfață
                RefreshList();
                
                MessageBox.Show("Rezervare creată cu succes!", "Succes");
            }
            catch (Exception ex)
            {
                // Dacă Managerul zice că e ocupată camera, afișăm eroarea aici
                MessageBox.Show($"Nu s-a putut crea rezervarea: {ex.Message}", "Eroare");
            }
        }
    }

    private void RefreshList()
    {
        // Luăm rezervările active pentru a le afișa în tabel (sau poți lua toate)
        IEnumerable<Reservation> list = _manager.GetActiveReservations(_currentAdmin);
        
        // Actualizăm colecția vizibilă
        Reservations = new ObservableCollection<Reservation>(list);
    }
}