using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class EditRoomViewModel : ViewModelBase
{
    // Proprietatea legata In XAML: {Binding SelectedRoom...}
    private RoomInputData _selectedRoom;
    public RoomInputData SelectedRoom
    {
        get => _selectedRoom;
        set { _selectedRoom = value; OnPropertyChanged(); }
    }

    // Lista pentru status
    public IEnumerable<RoomStatus> StatusList { get; }

    public ICommand SaveEditRoomCommand { get; }
    public Action<bool> CloseAction { get; set; }

    // Rezultatul final care va fi trimis catre Manager
    public Room ResultRoom { get; private set; }

    public EditRoomViewModel(Room roomToEdit)
    {
        // 1. Populam lista de statusurix
        StatusList = Enum.GetValues(typeof(RoomStatus)).Cast<RoomStatus>();

        // 2. Copiem datele camerei reale In obiectul nostru de editare (DTO)
        // Folosim RoomInputData pentru a evita problemele cu clasa abstracta Room
        SelectedRoom = new RoomInputData
        {
            RoomNumber = roomToEdit.Number,
            Type = roomToEdit.Type,
            Status = roomToEdit.Status
            // Pretul nu Il mai punem, se ia automat din tipul camerei
        };

        SaveEditRoomCommand = new RelayCommand(ExecuteSave);
    }

    private void ExecuteSave(object parameter)
    {
        // Validare
        if (string.IsNullOrWhiteSpace(SelectedRoom.Type))
        {
            MessageBox.Show("Tipul camerei este obligatoriu!");
            return;
        }

        try
        {
            // Reconstituim obiectul Room corect (Single/Double/etc)
            string tip = SelectedRoom.Type.Trim().ToLower();

            if (tip.Contains("single")) ResultRoom = new SingleRoom(SelectedRoom.RoomNumber);
            else if (tip.Contains("double")) ResultRoom = new DoubleRoom(SelectedRoom.RoomNumber);
            else if (tip.Contains("family")) ResultRoom = new FamilyRoom(SelectedRoom.RoomNumber);
            else if (tip.Contains("triple")) ResultRoom = new TripleRoom(SelectedRoom.RoomNumber);
            else ResultRoom = new SingleRoom(SelectedRoom.RoomNumber); // Fallback

            // Aplicam statusul selectat
            // Folosim 'with' daca Room e record, sau setare directa daca e clasa
            if (ResultRoom is Room r) 
            {
                // Deoarece RoomInputData e doar date, trebuie sa aplicam statusul pe obiectul creat
                // Solutia rapida pentru record-uri:
                if(ResultRoom is SingleRoom s) ResultRoom = s with { Status = SelectedRoom.Status };
                else if(ResultRoom is DoubleRoom d) ResultRoom = d with { Status = SelectedRoom.Status };
                else if(ResultRoom is FamilyRoom f) ResultRoom = f with { Status = SelectedRoom.Status };
                else if(ResultRoom is TripleRoom t) ResultRoom = t with { Status = SelectedRoom.Status };
            }

            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la salvare: {ex.Message}");
        }
    }
}