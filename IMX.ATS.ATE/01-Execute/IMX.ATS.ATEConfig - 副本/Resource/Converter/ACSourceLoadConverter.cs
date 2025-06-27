#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Resource.Converter
 * 唯一标识：995a411c-54f5-4723-988a-ce74a0480ad5
 * 文件名：ACSourceLoadConverter
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/7/24 15:02:17
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>
using IMX.Device.Common.Enumerations;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IMX.ATS.ATEConfig.Resource.Converter
{
    public class ACSourceLoadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString().ToUpper())
            {
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
