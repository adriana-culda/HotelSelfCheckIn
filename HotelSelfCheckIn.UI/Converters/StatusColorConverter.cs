using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HotelSelfCheckIn.UI.Models; // Asigura-te ca ai acces la Enum-ul RoomStatus

namespace HotelSelfCheckIn.UI.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RoomStatus status)
        {
            switch (status)
            {
                case RoomStatus.Free:
                    return Brushes.Green;
                case RoomStatus.Occupied:
                    return Brushes.Red;
                case RoomStatus.Cleaning:
                    return Brushes.Orange;
                case RoomStatus.Unavailable:
                    return Brushes.Gray;
            }
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}