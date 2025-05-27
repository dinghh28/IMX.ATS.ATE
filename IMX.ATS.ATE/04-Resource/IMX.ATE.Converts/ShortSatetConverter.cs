using IMX.Device.Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IMX.ATE.Converts
{
    public class ShortSatetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DeviceOutPutState state = (DeviceOutPutState)value;

            return state == DeviceOutPutState.ON;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool)value;

            return result ? DeviceOutPutState.ON : DeviceOutPutState.OFF;

        }
    }
}
