using IMX.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IMX.ATS.DIOS.Resource.Converter
{
    public class ResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResultState state = (ResultState)value;
            switch (state)
            {
                case ResultState.UNACCOMPLISHED:
                    return "进行中";
                case ResultState.SUCCESS:
                    return "OK";
                case ResultState.FAIL:
                    return "NG";
                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
