using IMX.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IMX.ATE.Converts
{
    public class InitStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResultState state = (ResultState)value;

            string icon = string.Empty;
            SolidColorBrush brush = Brushes.Black;

            switch (state)
            {
                case ResultState.UNACCOMPLISHED:
                    icon = "\ue610";
                    brush = Brushes.Black;
                    break;
                case ResultState.SUCCESS:
                    icon = "\ue945";
                    brush = Brushes.Green;
                    break;
                case ResultState.FAIL:
                    icon = "\ue93b";
                    brush = Brushes.Red;
                    break;
                default:
                    return value;
            }

            if (parameter?.ToString() == "Foreground")
            {
                return brush;
            }
            else
            {
                return icon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
