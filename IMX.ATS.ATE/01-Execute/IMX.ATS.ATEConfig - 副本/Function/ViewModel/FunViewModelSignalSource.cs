#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function.ViewModel
 * 唯一标识：f7246523-c368-49a2-a57b-034f2ae1c227
 * 文件名：FunViewSignalSource
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/5/6 15:34:17
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
    /// 信号发生器界面配置类
    /// </summary>
    public class FunViewModelSignalSource : FunViewModel
    {
        public override TestFunction Func { get ; set; }

        public override FuncitonType SupportFuncitonType => FuncitonType.SignalSource;

        public override string SupportFuncitonString => "SignalSource";


        #region 公共属性

        #region 界面绑定属性

        private double dutycycle;
        /// <summary>
        /// 设置输出占空比
        /// </summary>
        public double Dutycycle
        {
            get => dutycycle = (Func.Config as FunConfig_SignalSource).Duty;
            set
            {
                if (Set(nameof(Dutycycle), ref dutycycle, value))
                {
                    (Func.Config as FunConfig_SignalSource).Duty = value;
                }
            }
        }

        private double frequency;
        /// <summary>
        /// 设置输出频率
        /// </summary>
        public double Frequency
        {
            get => frequency = (Func.Config as FunConfig_SignalSource).Freq;
            set
            {
                if (Set(nameof(Frequency), ref frequency, value))
                {
                    (Func.Config as FunConfig_SignalSource).Freq = value;
                }
            }
        }

        private double ampl;
        /// <summary>
        /// 设置输出幅度
        /// </summary>
        public double Ampl
        {
            get => ampl = (Func.Config as FunConfig_SignalSource).Ampl;
            set
            {
                if (Set(nameof(Ampl), ref ampl, value))
                {
                    (Func.Config as FunConfig_SignalSource).Ampl = value;
                }
            }
        }
        #endregion

        #region 界面绑定指令
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
        public FunViewModelSignalSource() { }
        #endregion

    }
}
