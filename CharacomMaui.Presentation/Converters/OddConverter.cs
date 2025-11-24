using System.Globalization;

namespace CharacomMaui.Presentation.Converters;

public class OddConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null) return false;
    if (value is int num)
    {
      return num % 2 != 0;
    }
    return false;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}