using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class EditRoomViewModel : ViewModelBase
{
    // Proprietatea legată în XAML: {Binding SelectedRoom...}
    private RoomInputData _selectedRoom;
    public RoomInputData SelectedRoom
    {
        get => _selectedRoom;
        set { _selectedRoom = value; OnPropertyChanged(); }
    }

    // Lista pentru ComboBox-ul de status
    public IEnumerable<RoomStatus> StatusList { get; }

    public ICommand SaveEditRoomCommand { get; }
    public Action<bool> CloseAction { get; set; }

    // Rezultatul final care va fi trimis către Manager
    public Room ResultRoom { get; private set; }

    public EditRoomViewModel(Room roomToEdit)
    {
        // 1. Populăm lista de statusuri pentru ComboBox
        StatusList = Enum.GetValues(typeof(RoomStatus)).Cast<RoomStatus>();

        // 2. Copiem datele camerei reale în obiectul nostru de editare (DTO)
        // Folosim RoomInputData pentru a evita problemele cu clasa abstractă Room
        SelectedRoom = new RoomInputData
        {
            RoomNumber = roomToEdit.Number,
            Type = roomToEdit.Type,
            Status = roomToEdit.Status
            // Prețul nu îl mai punem, se ia automat din tipul camerei
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

            // Aplicăm statusul selectat în ComboBox
            // Folosim 'with' dacă Room e record, sau setare directă dacă e clasă
            if (ResultRoom is Room r) 
            {
                // Deoarece RoomInputData e doar date, trebuie sa aplicam statusul pe obiectul creat
                // Soluția rapidă pentru record-uri:
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