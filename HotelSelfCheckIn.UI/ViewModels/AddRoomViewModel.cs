using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AddRoomViewModel : ViewModelBase
{
    // 1. Folosim clasa intermediară (DTO) pentru Binding-ul din XAML.
    // Asta rezolvă problema "Cannot create instance of abstract class"
    // și problema "Cannot resolve RoomNumber" (pentru că definim noi proprietățile aici).
    private RoomInputData _newRoom;
    public RoomInputData NewRoom
    {
        get => _newRoom;
        set { _newRoom = value; OnPropertyChanged(); }
    }

    public ICommand SaveRoomCommand { get; }
    public Action<bool> CloseAction { get; set; }

    // Aici vom stoca rezultatul final (obiectul concret: SingleRoom, DoubleRoom etc.)
    public Room ResultRoom { get; private set; }

    public AddRoomViewModel(Room? roomToEdit = null)
    {
        _newRoom = new RoomInputData();

        if (roomToEdit != null)
        {
            // --- MOD EDITARE ---
            // Copiem datele din Modelul real în Formularul nostru
            _newRoom.RoomNumber = roomToEdit.Number; // Mapare Number -> RoomNumber
            _newRoom.Type = roomToEdit.Type;
            _newRoom.Status = roomToEdit.Status;
        }
        else
        {
            // --- MOD ADĂUGARE ---
            // Valori default
            _newRoom.RoomNumber = 0;
            _newRoom.Type = "Single"; // Default type
            _newRoom.Status = RoomStatus.Free;
        }

        SaveRoomCommand = new RelayCommand(ExecuteSave);
    }

    private void ExecuteSave(object parameter)
    {
        // 1. Validare
        if (NewRoom.RoomNumber <= 0)
        {
            MessageBox.Show("Numărul camerei trebuie să fie valid!");
            return;
        }
        if (string.IsNullOrWhiteSpace(NewRoom.Type))
        {
            MessageBox.Show("Introduceți tipul camerei!");
            return;
        }

        // 2. CREAREA OBIECTULUI CONCRET
        // Aici transformăm inputul userului (String) în Clasa C# (SingleRoom/DoubleRoom)
        // Prețul se va seta automat în constructorul claselor respective!
        try 
        {
            string tip = NewRoom.Type.Trim().ToLower();

            // Verificăm ce a scris userul și instanțiem clasa potrivită
            if (tip.Contains("single"))
            {
                ResultRoom = new SingleRoom(NewRoom.RoomNumber);
            }
            else if (tip.Contains("double"))
            {
                ResultRoom = new DoubleRoom(NewRoom.RoomNumber);
            }
            else if (tip.Contains("family"))
            {
                ResultRoom = new FamilyRoom(NewRoom.RoomNumber);
            }
            else if (tip.Contains("triple")) // Dacă ai clasa asta
            {
                ResultRoom = new TripleRoom(NewRoom.RoomNumber);
            }
            else 
            {
                // Fallback: Dacă userul scrie ceva necunoscut (ex: "Penthouse"),
                // îl tratăm ca pe un SingleRoom sau afișăm eroare.
                // Aici aleg să creez un SingleRoom ca să nu crape aplicația.
                ResultRoom = new SingleRoom(NewRoom.RoomNumber);
            }

            // Setăm statusul ales de user
            // (Folosim "with" pentru că Room e probabil record sau imuabil, sau setare directă)
            // Dacă Room e clasă normală: ResultRoom.Status = NewRoom.Status;
            // Dacă e record:
            ResultRoom = ResultRoom with { Status = NewRoom.Status };

            // Semnalăm succesul
            CloseAction?.Invoke(true);
        }
        catch(Exception ex)
        {
            MessageBox.Show("Eroare la crearea camerei: " + ex.Message);
        }
    }
}

// --- CLASA AJUTĂTOARE PENTRU XAML ---
// Aceasta trebuie să fie în același fișier sau accesibilă
public class RoomInputData
{
    // Aceste nume trebuie să fie EXACT ca în Binding-ul din XAML
    public int RoomNumber { get; set; } 
    public string Type { get; set; }
    public RoomStatus Status { get; set; }
    
    // AM SCOS PREȚUL DE AICI
}