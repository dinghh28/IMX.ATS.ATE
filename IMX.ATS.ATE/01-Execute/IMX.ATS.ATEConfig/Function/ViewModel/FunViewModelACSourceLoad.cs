#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ATEConfig.Function
 * 唯一标识：d3370c56-e018-4a69-ae7c-30e47ed984ed
 * 文件名：FunViewModelACSourceLoad
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/5/6 14:42:53
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
using IMX.Common;
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
    /// 源载一体机
    /// </summary>
    public class FunViewModelACSourceLoad : SteppingFunViewModel
    {
        public override TestFunction Func { get; set; } = TestFunction.Create(FuncitonType.ACSource);

        public override FuncitonType SupportFuncitonType =>  FuncitonType.ACSource;

        public override string SupportFuncitonString => "ACSourceLoad";

        #region 公共属性

        #region 界面绑定属性

        #region 系统/公共参数
        private DeviceOperatMode devicemode = DeviceOperatMode.VOLT;
        /// <summary>
        /// 源载模式
        /// </summary>
        public DeviceOperatMode DeviceMode
        {
            get => devicemode;
            set => Set(nameof(DeviceMode), ref devicemode, value);
        }

        private Phase_Mode phasemode = Phase_Mode.ONE;
        /// <summary>
        /// 相线模式
        /// </summary>
        public Phase_Mode PhaseMode
        {
            get => phasemode;
            set 
            {
                if (Set(nameof(PhaseMode), ref phasemode, value)) 
                {
                    if (value == Phase_Mode.ONE)
                    {
                        Balance = true;
                    }
                }
            }
        }

        private bool balance = true;
        /// <summary>
        /// 平衡模式
        /// </summary>
        public bool Balance
        {
            get => balance;
            set 
            {
                if (Set(nameof(Balance), ref balance, value))
                {
                    if (!value)
                    {
                        Set_StepModel = false;
                    }
                }
            }
        }

        private double openangle = 0;
        /// <summary>
        /// 开机相角
        /// </summary>
        public double OpenAngle
        {
            get => openangle;
            set => Set(nameof(OpenAngle), ref openangle, value);
        }

        private double closeangle = 0;
        /// <summary>
        /// 停机相角
        /// </summary>
        public double CloseAngle
        {
            get => closeangle;
            set => Set(nameof(CloseAngle), ref closeangle, value);
        }
        #endregion

        #region 电源模式参数

        private double phasecontrol_ab;
        /// <summary>
        /// AB相位角
        /// </summary>
        public double PhaseControl_AB
        {
            get => phasecontrol_ab;
            set => Set(nameof(PhaseControl_AB), ref phasecontrol_ab, value);
        }

        private double phasecontrol_ac;
        /// <summary>
        /// AC相位角
        /// </summary>
        public double PhaseControl_AC
        {
            get => phasecontrol_ac;
            set => Set(nameof(PhaseControl_AC), ref phasecontrol_ac, value);
        }

        private double outputfrequency = 50;
        /// <summary>
        /// 输出频率
        /// </summary>
        public double OutputFrequency
        {
            get => outputfrequency;
            set => Set(nameof(OutputFrequency), ref outputfrequency, value);
        }

        private double outputfrequencyslope = 5000000;
        /// <summary>
        /// 输出频率斜率
        /// </summary>
        public double OutputFrequencySlope
        {
            get => outputfrequencyslope;
            set => Set(nameof(OutputFrequencySlope), ref outputfrequencyslope, value);
        }

        #region A相
        private double outputvol_a;
        /// <summary>
        /// A相输出电压
        /// </summary>
        public double OutputVol_A
        {
            get => outputvol_a;
            set => Set(nameof(OutputVol_A), ref outputvol_a, value);
        }

        private double volslope_a;
        /// <summary>
        /// A相电压斜率
        /// </summary>
        public double VolSlope_A
        {
            get => volslope_a;
            set 
            {
                if (Set(nameof(VolSlope_A), ref volslope_a, value)) 
                {
                    if (Balance)
                    {
                        VolSlope_B = value;
                        VolSlope_C = value;
                    }
                }
            }
        }
        #endregion

        #region B相
        private double outputvol_b;
        /// <summary>
        /// B相输出电压
        /// </summary>
        public double OutputVol_B
        {
            get => outputvol_b;
            set => Set(nameof(OutputVol_B), ref outputvol_b, value);
        }

        private double volslope_b;
        /// <summary>
        /// B相电压斜率
        /// </summary>
        public double VolSlope_B
        {
            get => volslope_b;
            set => Set(nameof(VolSlope_B), ref volslope_b, value);
        }
        #endregion

        #region C相
        private double outputvol_c;
        /// <summary>
        /// C相输出电压
        /// </summary>
        public double OutputVol_C
        {
            get => outputvol_c;
            set => Set(nameof(OutputVol_C), ref outputvol_c, value);
        }

        private double volslope_c;
        /// <summary>
        /// C相电压斜率
        /// </summary>
        public double VolSlope_C
        {
            get => volslope_c;
            set => Set(nameof(VolSlope_C), ref volslope_c, value);
        }
        #endregion

        #endregion

        #region 负载模式参数

        private Opaerate_Mode  opaeratemode;
        /// <summary>
        /// 拉载模式
        /// </summary>
        public Opaerate_Mode OpaerateMode
        {
            get => opaeratemode;
            set => Set(nameof(OpaerateMode), ref opaeratemode, value);
        }

        private bool phaseloss_b;
        /// <summary>
        /// B相缺相
        /// </summary>
        public bool PhaseLoss_B
        {
            get => phaseloss_b;
            set => Set(nameof(PhaseLoss_B), ref phaseloss_b, value);
        }

        private bool phaseloss_c;
        /// <summary>
        /// C相缺相
        /// </summary>
        public bool PhaseLoss_C
        {
            get => phaseloss_c;
            set => Set(nameof(PhaseLoss_C), ref phaseloss_c, value);
        }

        #region 拉载值
        #region A相
        private double setvalue_a;
        /// <summary>
        /// 拉载值
        /// </summary>
        public double SetValue_A
        {
            get => setvalue_a;
            set
            {
                if (Set(nameof(SetValue_A), ref setvalue_a, value))
                {
                    if (Balance)
                    {
                        SetValue_B = value;
                        SetValue_C = value;
                    }
                }
            }
        }

        private double curslope_a = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_A
        {
            get => curslope_a;
            set
            {
                if (Set(nameof(CurSlope_A), ref curslope_a, value))
                {
                    if (Balance)
                    {
                        CurSlope_B = value;
                        CurSlope_C = value;
                    }
                }
            }
        }

        private double powerfactor_a;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_A
        {
            get => powerfactor_a;
            set
            {
                if (Set(nameof(PowerFactor_A), ref powerfactor_a, value))
                {
                    if (Balance)
                    {
                        PowerFactor_B = value;
                        PowerFactor_C = value;
                    }
                }
            }
        }
        #endregion

        #region B相
        private double setvalue_b;
        /// <summary>
        /// 拉载值
        /// </summary>
        public double SetValue_B
        {
            get => setvalue_b;
            set => Set(nameof(SetValue_B), ref setvalue_b, value);
        }

        private double curslope_b = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_B
        {
            get => curslope_b;
            set => Set(nameof(CurSlope_B), ref curslope_b, value);
        }

        private double powerfactor_b;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_B
        {
            get => powerfactor_b;
            set => Set(nameof(PowerFactor_B), ref powerfactor_b, value);
        }
        #endregion

        #region C相
        private double setvalue_c;
        /// <summary>
        /// 拉载值
        /// </summary>
        public double SetValue_C
        {
            get => setvalue_c;
            set => Set(nameof(SetValue_C), ref setvalue_c, value);
        }

        private double curslope_c = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_C
        {
            get => curslope_c;
            set => Set(nameof(CurSlope_C), ref curslope_c, value);
        }

        private double powerfactor_c;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_C
        {
            get => powerfactor_c;
            set => Set(nameof(PowerFactor_C), ref powerfactor_c, value);
        }
        #endregion
        #endregion

        #endregion

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
        public FunViewModelACSourceLoad() 
        {
            ConditionValues.Clear();

            if (!SupportConfig.DicProcessConfig.TryGetValue(GlobalModel.NowProcessName, out ProcessConfig_EX value))
            {
                return;
            }


            for (int i = 0; i < value.Test_ReadData_Euq.Count; i++)
            {
                var data = value.Test_ReadData_Euq[i];
                ConditionValues.Add(new ModDeviceReadData
                {
                    DataInfo = data,
                });
            }

            if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three)
            {
                for (int i = 0; i < value.Test_ReadData_EX.Count; i++)
                {
                    var data = value.Test_ReadData_EX[i];
                    ConditionValues.Add(new ModDeviceReadData
                    {
                        DataInfo = data,
                    });
                }
            }

            for (int i = 0; i < value.Test_ReadData_Pro.Count; i++)
            {
                var data = value.Test_ReadData_Pro[i];
                ConditionValues.Add(new ModDeviceReadData
                {
                    DataInfo = data,
                });
            }
        }
        #endregion

    }
}
