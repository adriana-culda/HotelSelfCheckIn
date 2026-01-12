namespace HotelSelfCheckIn.UI.Models;

public record Reservation
{
    public Guid ReservationID { get; init; }
    public string Username { get; init; }
    public int RoomNumber { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public ReservationStatus Status { get; init; }

    public Reservation(Guid reservationID, string username, int roomNumber, DateTime startDate, DateTime endDate, ReservationStatus status= ReservationStatus.Active)
    {
        ReservationID = reservationID;
        Username = username;
        RoomNumber = roomNumber;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }

    public Reservation Cancel()
    {
        if (Status != ReservationStatus.Active)
             throw new InvalidOperationException("Rezervarea nu poate fi anulata.");
        return this with { Status = ReservationStatus.Cancelled };
    }
    public Reservation Complete()
    {
        return this with { Status = ReservationStatus.Completed };
    }
}