using System;
using System.Windows;
using System.Windows.Input;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class EditReservationViewModel : ViewModelBase
{

    public string Username { get; set; }
    
    private string _roomNumber;
    public string RoomNumber
    {
        get => _roomNumber;
        set { _roomNumber = value; OnPropertyChanged(); }
    }

    private DateTime _startDate;
    public DateTime StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
        get => _endDate;
        set { _endDate = value; OnPropertyChanged(); }
    }


    public ICommand SaveCommand { get; }

    //---CONSTRUCTOR
    public EditReservationViewModel(Reservation res)
    {
        
        Username = res.Username;
        RoomNumber = res.RoomNumber.ToString();
        StartDate = res.StartDate;
        EndDate = res.EndDate;

        SaveCommand = new RelayCommand(ExecuteSave);
    }

    private void ExecuteSave(object parameter)
    {
       
        if (!int.TryParse(RoomNumber, out int r))
        {
            MessageBox.Show("Numarul camerei este invalid!");
            return;
        }
        if (StartDate >= EndDate)
        {
            MessageBox.Show("Data de Check-Out trebuie sa fie dupa Check-In!");
            return;
        }

        
        if (parameter is Window window)
        {
            window.DialogResult = true; 
            window.Close();
        }
    }
    
   
    public int GetParsedRoomNumber() => int.Parse(RoomNumber);
    public DateTime GetStartDate() => StartDate;
    public DateTime GetEndDate() => EndDate;
}