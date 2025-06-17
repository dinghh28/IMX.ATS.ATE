using IMX.Device.Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace IMX.ATS.Manual.Convter
{
    public class ACSourceLoadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString().ToUpper())
            {
                //单三相判断
                case "PHASEMODE":
                    return (Phase_Mode)value == Phase_Mode.THREE;
                //平衡状态判断
                case "BALANCE":
                    Type type = typeof(Visibility);
                    if (targetType == type)
                    {
                        return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                    }
                    return !(bool)value;
                #region 源载模式判断
                case "LOAD":
                    return (DeviceOperatMode)value == DeviceOperatMode.VOLT ? Visibility.Collapsed : Visibility.Visible;
                case "VOLT":
                    return (DeviceOperatMode)value == DeviceOperatMode.LOAD ? Visibility.Collapsed : Visibility.Visible;
                #endregion
                #region 负载模式设置
                case "OPAERATEMODE":
                    return (Opaerate_Mode)value == Opaerate_Mode.CR ? "欧" : (Opaerate_Mode)value == Opaerate_Mode.CC ? "A" : "kW";
                case "CR":
                    return (Opaerate_Mode)value == Opaerate_Mode.CR ? Visibility.Collapsed : Visibility.Visible;
                #endregion
                #region 步进模式设置
                case "STEP":
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                #endregion
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
