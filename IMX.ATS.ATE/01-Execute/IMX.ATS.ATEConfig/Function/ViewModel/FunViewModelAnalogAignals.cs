#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：5042635c-30d2-4db7-b416-00814234c775
 * 文件名：FunViewModelAnalogAignals
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/5/6 16:14:02
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

using H.WPF.Framework;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// CC/CP模拟信号界面配置类
    /// </summary>
    public class FunViewModelAnalogAignals : FunViewModel
    {
        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.AnalogAignals);

        public override FuncitonType SupportFuncitonType =>  FuncitonType.AnalogAignals;

        public override string SupportFuncitonString => "AnalogAignals";

        #region 公共属性

        #region 界面绑定属性
        #endregion

        #region 界面绑定指令
        private CCResMode ccres = CCResMode.Res_100;
        /// <summary>
        /// 当前选择CC模式
        /// </summary>
        public CCResMode CCRes
        {
            get => ccres = (Func.Config as FunConfig_AnalogAignals).ResMode;
            set
            {
                if (Set(nameof(CCRes), ref ccres, value))
                {
                    (Func.Config as FunConfig_AnalogAignals).ResMode = value;
                }
            }
        }

        private double dutycycle = 0;
        /// <summary>
        /// 设置CP占空比
        /// </summary>
        public double Dutycycle
        {
            get => dutycycle = (Func.Config as FunConfig_AnalogAignals).Dutycycle;
            set
            {
                if (Set(nameof(Dutycycle), ref dutycycle, value))
                {
                    (Func.Config as FunConfig_AnalogAignals).Dutycycle = value;
                }
            }
        }

        private double frequency = 0;
        /// <summary>
        /// 设置CP频率
        /// </summary>
        public double Frequency
        {
            get => frequency = (Func.Config as FunConfig_AnalogAignals).Frequency;
            set
            {
                if (Set(nameof(Frequency), ref frequency, value))
                {
                    (Func.Config as FunConfig_AnalogAignals).Frequency = value;
                }
            }
        }

        #endregion

        #endregion

        #region 私有变量
        #endregion

        #region 私有方法
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public FunViewModelAnalogAignals() { }
        #endregion

    }
}
