using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AddRoomViewModel : ViewModelBase
{
    // 1. Folosim clasa intermediara (DTO) pentru Binding-ul din XAML.
    // Asta rezolva problema "Cannot create instance of abstract class"
    // si problema "Cannot resolve RoomNumber" (pentru ca definim proprietatile aici).
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
            // Copiem datele din Modelul real In Formularul nostru
            _newRoom.RoomNumber = roomToEdit.Number; // Mapare Number -> RoomNumber
            _newRoom.Type = roomToEdit.Type;
            _newRoom.Status = roomToEdit.Status;
        }
        else
        {
            // --- MOD ADaUGARE ---
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
            MessageBox.Show("Numarul camerei trebuie sa fie valid!");
            return;
        }
        if (string.IsNullOrWhiteSpace(NewRoom.Type))
        {
            MessageBox.Show("Introduceti tipul camerei!");
            return;
        }

        // 2. CREAREA OBIECTULUI CONCRET
        // Aici transform inputul userului (String) In Clasa C# (SingleRoom/DoubleRoom)
        // Pretul se va seta automat In constructorul claselor respective!
        try 
        {
            string tip = NewRoom.Type.Trim().ToLower();

            // Verificam ce a scris userul si instantiem clasa potrivita
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
            else if (tip.Contains("triple")) // Daca ai clasa asta
            {
                ResultRoom = new TripleRoom(NewRoom.RoomNumber);
            }
            else 
            {
                // Fallback: Daca userul scrie ceva necunoscut (ex: "Penthouse"),
                // Il tratam ca pe un SingleRoom sau afisam eroare.
                // Aici aleg sa creez un SingleRoom ca sa nu crape aplicatia.
                ResultRoom = new SingleRoom(NewRoom.RoomNumber);
            }

            // Setam statusul ales de user
            // (Folosim "with" pentru ca Room e probabil record sau imuabil, sau setare directa)
            // Daca Room e clasa normala: ResultRoom.Status = NewRoom.Status;
            // Daca e record:
            ResultRoom = ResultRoom with { Status = NewRoom.Status };

            // Semnalam succesul
            CloseAction?.Invoke(true);
        }
        catch(Exception ex)
        {
            MessageBox.Show("Eroare la crearea camerei: " + ex.Message);
        }
    }
}

// --- CLASA AJUTATOARE PENTRU XAML ---
// Aceasta trebuie sa fie In acelasi fisier sau accesibila
public class RoomInputData
{
    // Aceste nume trebuie sa fie EXACT ca In Binding-ul din XAML
    public int RoomNumber { get; set; } 
    public string Type { get; set; }
    public RoomStatus Status { get; set; }
    
 
}