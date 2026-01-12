using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HotelSelfCheckIn.UI.Models; // Asigură-te că namespace-ul RoomStatus e corect

namespace HotelSelfCheckIn.UI.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verificăm ce status are camera
            if (value is RoomStatus status)
            {
                return status switch
                {
                    RoomStatus.Free => Brushes.Green,       // Verde pentru liber
                    RoomStatus.Occupied => Brushes.Red,     // Roșu pentru ocupat
                    RoomStatus.Cleaning => Brushes.Orange,  // Portocaliu pentru curățenie
                    RoomStatus.Unavailable => Brushes.Gray, // Gri pentru indisponibil
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}