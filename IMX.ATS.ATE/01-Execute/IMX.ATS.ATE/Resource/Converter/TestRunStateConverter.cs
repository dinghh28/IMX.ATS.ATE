﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.BIS.Resource.Converter
 * 唯一标识：995a411c-54f5-4723-988a-ce74a0480ad5
 * 文件名：ProtectConfigConverter
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IMX.ATS.ATE.Resource.Converter
{
    public class TestRunStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isuse = (bool)value;

            switch (parameter.ToString())
            {
                case "String":
                    return isuse ? "停止试验" : "开始试验";
                case "Brush":
                    return isuse ? Brushes.Red : Brushes.Green;
                case "Bool":
                    return !isuse;
                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "Int")
            {
                return Int32.Parse(value.ToString()) == 0;
            }
            return value;
        }
    }
}
