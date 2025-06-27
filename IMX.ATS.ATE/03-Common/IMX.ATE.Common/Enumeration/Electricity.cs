using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATE.Common
{
    /// <summary>
    /// 电能
    /// </summary>
    public enum Electricity
    {
        /// <summary>
        /// 单相交流电
        /// </summary>
        [Description("单相")]
        Single,
        /// <summary>
        /// 单相交流电带逆变
        /// </summary>
        [Description("单相带逆变")]
        SingleANDInversion,
        /// <summary>
        /// 三相交流电
        /// </summary>
        [Description("三相")]
        Three,
        /// <summary>
        /// 三相交流电带逆变
        /// </summary>
        [Description("三相带逆变")]
        ThreeANDInversion,
    }
}
