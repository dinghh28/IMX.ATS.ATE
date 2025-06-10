using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class ManualViewModelACSourceLoad : WindowViewModelBaseEx
    {
        #region 私有变量

        #endregion

        #region 界面绑定

        #region 界面绑定属性

        //设备开关操作
        public List<SetOutPutState> OperateTypeList { get; set; } = new List<SetOutPutState>
        {
            SetOutPutState.ON,
            SetOutPutState.OFF,
            SetOutPutState.Null
        };

        private SetOutPutState operateType;
        /// <summary>
        /// 当前动作
        /// </summary>
        public SetOutPutState OperateType
        {
            get => operateType;
            set => Set(nameof(OperateType), ref operateType, value);
        }

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
            set => Set(nameof(Balance), ref balance, value);
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

        private Opaerate_Mode opaeratemode = Opaerate_Mode.CC;
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

        public RelayCommand SetValues => new RelayCommand(SetedValues);

        #endregion

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法
        private void SetedValues()
        {
            try
            {
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.ACSourceLoad.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IACSourceLoad operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }


                string InfoString = string.Empty;
                OperateResult result = operate.SetOperatMode(DeviceMode)
                    .And(operate.SetPhaseMode(PhaseMode))
                    .And(operate.SetGeneralValue(Balance, OpenAngle, CloseAngle));

                InfoString = $"设置\n[源载模式]{DeviceMode}\n" +
                    $"[相线模式]{PhaseMode}\n" +
                    $"[平衡模式]{Balance}\n" +
                    $"[开机相角]{OpenAngle}\n" +
                    $"[停机相角]{CloseAngle}";

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }

                InfoString = $"{InfoString}成功";

                if (DeviceMode == DeviceOperatMode.VOLT)
                {
                    result = operate.SetVoltValue(
                        PhaseControl_AB, PhaseControl_AC, OutputFrequency,OutputFrequencySlope,
                        new List<double> { SetValue_A, SetValue_B, SetValue_C },
                        new List<double> { VolSlope_A, VolSlope_B, VolSlope_C});

                    InfoString = $"设置\n[AB相位角]{PhaseControl_AB}\n" +
                        $"[AC相位角]{PhaseControl_AC}\n" +
                        $"[输出频率]{OutputFrequency}\n" +
                        $"[输出频率斜率]{OutputFrequencySlope}\n" +
                        $"[A相拉载值]{SetValue_A}\n" +
                        $"[B相拉载值]{SetValue_B}\n" +
                        $"[C相拉载值]{SetValue_C}\n" +
                        $"[A相电压斜率]{VolSlope_A}\n" +
                        $"[B相电压斜率]{VolSlope_B}\n" +
                        $"[C相电压斜率]{VolSlope_C}\n" ;
                }
                else
                {
                    result = operate.SetLoadValue(
                        PhaseLoss_B, PhaseLoss_C, OpaerateMode,
                        new List<double> { SetValue_A, SetValue_B, SetValue_C },
                        new List<double> { VolSlope_A, VolSlope_B, VolSlope_C },
                        new List<double> { PowerFactor_A, PowerFactor_B, PowerFactor_C });

                    InfoString = $"设置\n[B相缺相]{PhaseLoss_B}\n" +
                       $"[C相缺相]{PhaseLoss_C}\n" +
                       $"[拉载模式]{OpaerateMode}\n" +
                       $"[A相拉载值]{SetValue_A}\n" +
                       $"[B相拉载值]{SetValue_B}\n" +
                       $"[C相拉载值]{SetValue_C}\n" +
                       $"[A相电压斜率]{VolSlope_A}\n" +
                       $"[B相电压斜率]{VolSlope_B}\n" +
                       $"[C相电压斜率]{VolSlope_C}\n"+
                       $"[A相功率因素]{PowerFactor_A}\n" +
                       $"[B相功率因素]{PowerFactor_B}\n" +
                       $"[C相功率因素]{PowerFactor_C}\n";
                }

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }
                InfoString = $"{InfoString}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelACSourceLoad), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelACSourceLoad), nameof(SetedValues), $"设备已{InfoString}");
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelACSourceLoad), nameof(SetedValues), ex);
                MessageBox.Show("设置失败", "参数设置", MessageBoxButton.OK);
            }
        }
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

        #region 构造函数

        #endregion
    }
}
