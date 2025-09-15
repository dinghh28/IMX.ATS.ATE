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

using H.Maths.Encryption.AES;
using H.WPF.Framework;
using IMX.Common;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function.ViewModel.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 源载一体机
    /// </summary>
    public class FunViewModelACSourceLoad : SteppingFunViewModel
    {
        private TestFunction func = TestFunction.Create(FuncitonType.ACSourceLoad);
        public override TestFunction Func
        {
            get => func;
            set
            {
                func = value;
                FunConfig_ACSourceLoad config = value.Config as FunConfig_ACSourceLoad;

                StepValues.Clear();

                ObservableCollection<string> CondNames = new ObservableCollection<string>();
                ObservableCollection<ModDeviceReadData> CondValues = new ObservableCollection<ModDeviceReadData>();

                if (ConditionValues.Count < 1)
                {
                    if (!SupportConfig.DicProcessConfig.TryGetValue(GlobalModel.NowProcessName, out ProcessConfig_EX processconfig))
                    {
                        return;
                    }


                    for (int i = 0; i < processconfig.Test_ReadData_Euq.Count; i++)
                    {
                        var data = processconfig.Test_ReadData_Euq[i];
                        ConditionValues.Add(new ModDeviceReadData
                        {
                            DataInfo = data,
                            DeviceTypename = "Acquisition",
                        });
                    }

                    if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three
                        || GlobalModel.NowElectricity == ATE.Common.Electricity.ThreeANDInversion)
                    {
                        for (int i = 0; i < processconfig.Test_ReadData_EX.Count; i++)
                        {
                            var data = processconfig.Test_ReadData_EX[i];
                            ConditionValues.Add(new ModDeviceReadData
                            {
                                DataInfo = data,
                                DeviceTypename = "Acquisition",
                            });
                        }
                    }

                    for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCReceiveSignals.Count; i++)
                    {
                        var data = GlobalModel.TestDBCconfig.Test_DBCReceiveSignals[i];
                        ConditionValues.Add(new ModDeviceReadData
                        {
                            DataInfo = new ModTestDataInfo { Name = data.Custom_Name },
                            DeviceTypename = "Product",
                        });
                    }
                }

                for (int i = 0; i < ConditionValues.Count; i++)
                {
                    CondNames.Add(ConditionValues[i].DataInfo.Name);
                    CondValues.Add(ConditionValues[i]);
                }

                config.Values ??= [];

                for (int i = 0; i < config.Values.Count; i++)
                {
                    int findindex = CondNames.ToList().FindIndex(n => n == config.Values[i].Value.DataInfo.Name);
                    if (findindex == -1) 
                    {
                        continue;
                    }

                    var configvalue = new StepValue
                    {
                        ConditionValue = config.Values[i],
                        ConditionValues = CondValues,
                        ConditionNames = CondNames,
                        ConditionIndex = findindex,
                    };
                    
                    StepValues.Add(configvalue);
                }

                //步进状态赋值
                CanUseStep = Balance;
            }
        }

        public override FuncitonType SupportFuncitonType =>  FuncitonType.ACSourceLoad;

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
            get 
            {
                var mode = (Func.Config as FunConfig_ACSourceLoad).DeviceMode;
                VoltShow = mode == DeviceOperatMode.VOLT ? Visibility.Visible : Visibility.Collapsed;
                LoadShow = mode == DeviceOperatMode.LOAD ? Visibility.Visible : Visibility.Collapsed;
                devicemode = mode;
                return devicemode;
            }
            set
            {
                if (Set(nameof(DeviceMode), ref devicemode, value))
                {
                    VoltShow = value == DeviceOperatMode.VOLT? Visibility.Visible: Visibility.Collapsed;
                    LoadShow = value == DeviceOperatMode.LOAD? Visibility.Visible: Visibility.Collapsed;
                    (Func.Config as FunConfig_ACSourceLoad).DeviceMode = value;
                }
            }
        }

        private Phase_Mode phasemode = Phase_Mode.ONE;
        /// <summary>
        /// 相线模式
        /// </summary>
        public Phase_Mode PhaseMode
        {
            get 
            {
                Phase_Mode mode = (Func.Config as FunConfig_ACSourceLoad).PhaseMode;
                if (mode == Phase_Mode.ONE)
                {
                    Balance = true;
                }
                Thread.Sleep(10);
                EnableBalance = mode == Phase_Mode.THREE;
                phasemode = mode;
                return phasemode;
            } 
            set 
            {
                if (Set(nameof(PhaseMode), ref phasemode, value)) 
                {
                    if (value == Phase_Mode.ONE)
                    {
                        Balance = true;
                    }
                    Thread.Sleep(10);
                    EnableBalance = value == Phase_Mode.THREE;

                    (Func.Config as FunConfig_ACSourceLoad).PhaseMode = value;
                }
            }
        }

        private bool balance = true;
        /// <summary>
        /// 平衡模式
        /// </summary>
        public bool Balance
        {
            get 
            {
                balance = (Func.Config as FunConfig_ACSourceLoad).Balance;
                if (!balance)
                {
                    Set_StepModel = false;
                }
                Thread.Sleep(5);
                CanUseStep = balance;

                BCShow = balance ? Visibility.Collapsed : Visibility.Visible;
                SetShow = balance ? Visibility.Visible : Visibility.Collapsed;
                EnableSetBlanceValue = !balance;
                return balance;
            }
            set 
            {
                if (Set(nameof(Balance), ref balance, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).Balance = value;

                    if (!value)
                    {
                        Set_StepModel = false;
                    }
                    Thread.Sleep (5);
                    CanUseStep = value;

                    BCShow = value ? Visibility.Collapsed : Visibility.Visible;
                    SetShow = value ? Visibility.Visible : Visibility.Collapsed;
                    EnableSetBlanceValue = !value;
                }
            }
        }

        private double openangle = 0;
        /// <summary>
        /// 开机相角
        /// </summary>
        public double OpenAngle
        {
            get => openangle = (Func.Config as FunConfig_ACSourceLoad).OpenAngle;
            set
            {
                if (Set(nameof(OpenAngle), ref openangle, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).OpenAngle = value;
                }
            }
        }

        private double closeangle = 0;
        /// <summary>
        /// 停机相角
        /// </summary>
        public double CloseAngle
        {
            get => closeangle = (Func.Config as FunConfig_ACSourceLoad).CloseAngle;
            set
            {
                if (Set(nameof(CloseAngle), ref closeangle, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).CloseAngle = value;
                }
            }
        }
        #endregion

        #region 电源模式参数

        private double phasecontrol_ab;
        /// <summary>
        /// AB相位角
        /// </summary>
        public double PhaseControl_AB
        {
            get => phasecontrol_ab = (Func.Config as FunConfig_ACSourceLoad).PhaseControl_AB;
            set
            {
                if (Set(nameof(PhaseControl_AB), ref phasecontrol_ab, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PhaseControl_AB = value;
                }
            }
        }

        private double phasecontrol_ac;
        /// <summary>
        /// AC相位角
        /// </summary>
        public double PhaseControl_AC
        {
            get => phasecontrol_ac = (Func.Config as FunConfig_ACSourceLoad).PhaseControl_AC;
            set
            {
                if (Set(nameof(PhaseControl_AC), ref phasecontrol_ac, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PhaseControl_AC = value;
                }
            }
        }

        private double outputfrequency = 50;
        /// <summary>
        /// 输出频率
        /// </summary>
        public double OutputFrequency
        {
            get => outputfrequency = (Func.Config as FunConfig_ACSourceLoad).OutputFrequency; set
            {
                if (Set(nameof(OutputFrequency), ref outputfrequency, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).OutputFrequency = value;
                }
            }
        }

        private double outputfrequencyslope = 5000000;
        /// <summary>
        /// 输出频率斜率
        /// </summary>
        public double OutputFrequencySlope
        {
            get => outputfrequencyslope = (Func.Config as FunConfig_ACSourceLoad).OutputFrequencySlope; set
            {
                if (Set(nameof(OutputFrequencySlope), ref outputfrequencyslope, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).OutputFrequencySlope = value;
                }
            }
        }

        #region A相
        private double outputvol_a;
        /// <summary>
        /// A相输出电压
        /// </summary>
        public double OutputVol_A
        {
            get => outputvol_a = (Func.Config as FunConfig_ACSourceLoad).SetValue_A;
            set
            {
                if (Set(nameof(OutputVol_A), ref outputvol_a, value))
                {
                    if (Balance)
                    {
                        OutputVol_B = value;
                        OutputVol_C = value;
                    }
                    (Func.Config as FunConfig_ACSourceLoad).SetValue_A = value;
                }
            }
        }

        private double volslope_a;
        /// <summary>
        /// A相电压斜率
        /// </summary>
        public double VolSlope_A
        {
            get => volslope_a = (Func.Config as FunConfig_ACSourceLoad).VolSlope_A; 
            set
            {
                if (Set(nameof(VolSlope_A), ref volslope_a, value))
                {
                    if (Balance)
                    {
                        VolSlope_B = value;
                        VolSlope_C = value;
                    } 
                    (Func.Config as FunConfig_ACSourceLoad).VolSlope_A = value;
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
            get => volslope_b = (Func.Config as FunConfig_ACSourceLoad).VolSlope_B; set
            {
                if (Set(nameof(VolSlope_B), ref volslope_b, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).VolSlope_B = value;
                }
            }
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
            get => volslope_c = (Func.Config as FunConfig_ACSourceLoad).VolSlope_C; set
            {
                if (Set(nameof(VolSlope_C), ref volslope_c, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).VolSlope_C = value;
                }
            }
        }
        #endregion

        #endregion

        #region 负载模式参数

        private Opaerate_Mode  opaeratemode = Opaerate_Mode.CC;
        /// <summary>
        /// 拉载模式
        /// </summary>
        public Opaerate_Mode OpaerateMode
        {
            get 
            {
                opaeratemode = (Func.Config as FunConfig_ACSourceLoad).OpaerateMode;
                switch (opaeratemode)
                {
                    case Opaerate_Mode.CC:
                        if (LoadUnit != "A")
                        {
                            LoadUnit = "A";
                        }
                        LoadExShow = Visibility.Visible;
                        break;
                    case Opaerate_Mode.CR:
                        if (LoadUnit != "欧")
                        {
                            LoadUnit = "欧";
                        }
                        LoadExShow = Visibility.Collapsed;
                        break;
                    case Opaerate_Mode.CP:
                        if (LoadUnit != "kW")
                        {
                            LoadUnit = "kW";
                        }
                        LoadExShow = Visibility.Visible;
                        break;
                    case Opaerate_Mode.CV:
                    case Opaerate_Mode.NULL:
                    default:
                        if (LoadUnit != "A")
                        {
                            LoadUnit = "A";
                        }
                        LoadExShow = Visibility.Collapsed;
                        break;
                }
                return opaeratemode;
            } 
            set
            {
                if (Set(nameof(OpaerateMode), ref opaeratemode, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).OpaerateMode = value;

                    switch (value)
                    {
                        case Opaerate_Mode.CC:
                            if (LoadUnit != "A")
                            {
                                LoadUnit = "A";
                            }
                            LoadExShow = Visibility.Visible;
                            break;
                        case Opaerate_Mode.CR:
                            if (LoadUnit != "欧")
                            {
                                LoadUnit = "欧";
                            }
                            LoadExShow = Visibility.Collapsed;
                            break;
                        case Opaerate_Mode.CP:
                            if (LoadUnit != "kW")
                            {
                                LoadUnit = "kW";
                            }
                            LoadExShow = Visibility.Visible;
                            break;
                        case Opaerate_Mode.CV:
                        case Opaerate_Mode.NULL:
                        default:
                            if (LoadUnit != "A")
                            {
                                LoadUnit = "A";
                            }
                            LoadExShow = Visibility.Collapsed;
                            break;
                    }
                }
            }
        }

        private bool phaseloss_b;
        /// <summary>
        /// B相缺相
        /// </summary>
        public bool PhaseLoss_B
        {
            get => phaseloss_b = (Func.Config as FunConfig_ACSourceLoad).PhaseLoss_B; 
            set
            {
                if (Set(nameof(PhaseLoss_B), ref phaseloss_b, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PhaseLoss_B = value;
                }
            }
        }

        private bool phaseloss_c;
        /// <summary>
        /// C相缺相
        /// </summary>
        public bool PhaseLoss_C
        {
            get => phaseloss_c = (Func.Config as FunConfig_ACSourceLoad).PhaseLoss_C; 
            set
            {
                if (Set(nameof(PhaseLoss_C), ref phaseloss_c, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PhaseLoss_C = value;
                }
            }
        }

        #region 拉载值
        #region A相
        private double setvalue_a;
        /// <summary>
        /// 拉载值
        /// </summary>
        public double SetValue_A
        {
            get => setvalue_a = (Func.Config as FunConfig_ACSourceLoad).SetValue_A; 
            set
            {
                if (Set(nameof(SetValue_A), ref setvalue_a, value))
                {
                    if (Balance)
                    {
                        SetValue_B = value;
                        SetValue_C = value;
                    } 
                    (Func.Config as FunConfig_ACSourceLoad).SetValue_A = value;
                }
            }
        }

        private double curslope_a = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_A
        {
            get => curslope_a = (Func.Config as FunConfig_ACSourceLoad).CurSlope_A;
            set
            {
                if (Set(nameof(CurSlope_A), ref curslope_a, value))
                {
                    if (Balance)
                    {
                        CurSlope_B = value;
                        CurSlope_C = value;
                    }

                    (Func.Config as FunConfig_ACSourceLoad).CurSlope_A = value;
                }
            }
        }

        private double powerfactor_a;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_A
        {
            get => powerfactor_a = (Func.Config as FunConfig_ACSourceLoad).PowerFactor_A;
            set
            {
                if (Set(nameof(PowerFactor_A), ref powerfactor_a, value))
                {
                    if (Balance)
                    {
                        PowerFactor_B = value;
                        PowerFactor_C = value;
                    }

                    (Func.Config as FunConfig_ACSourceLoad).PowerFactor_A = value;
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
            get => setvalue_b = (Func.Config as FunConfig_ACSourceLoad).SetValue_B; 
            set
            {
                if (Set(nameof(SetValue_B), ref setvalue_b, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).SetValue_B = value;
                }
            }
        }

        private double curslope_b = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_B
        {
            get => curslope_b = (Func.Config as FunConfig_ACSourceLoad).CurSlope_B; 
            set
            {
                if (Set(nameof(CurSlope_B), ref curslope_b, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).CurSlope_B = value;
                }
            }
        }

        private double powerfactor_b;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_B
        {
            get => powerfactor_b = (Func.Config as FunConfig_ACSourceLoad).PowerFactor_B; 
            set
            {
                if (Set(nameof(PowerFactor_B), ref powerfactor_b, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PowerFactor_B = value;
                }
            }
        }
        #endregion

        #region C相
        private double setvalue_c;
        /// <summary>
        /// 拉载值
        /// </summary>
        public double SetValue_C
        {
            get => setvalue_c = (Func.Config as FunConfig_ACSourceLoad).SetValue_C; 
            set
            {
                if (Set(nameof(SetValue_C), ref setvalue_c, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).SetValue_C = value;
                }
            }
        }

        private double curslope_c = 250;
        /// <summary>
        /// 电流斜率
        /// </summary>
        public double CurSlope_C
        {
            get => curslope_c = (Func.Config as FunConfig_ACSourceLoad).CurSlope_C; 
            set
            {
                if (Set(nameof(CurSlope_C), ref curslope_c, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).CurSlope_C = value;
                }
            }
        }

        private double powerfactor_c;
        /// <summary>
        /// 功率因素
        /// </summary>
        public double PowerFactor_C
        {
            get => powerfactor_c = (Func.Config as FunConfig_ACSourceLoad).PowerFactor_C; 
            set
            {
                if (Set(nameof(PowerFactor_C), ref powerfactor_c, value))
                {
                    (Func.Config as FunConfig_ACSourceLoad).PowerFactor_C = value;
                }
            }
        }
        #endregion

        private string loadunit = "A";
        /// <summary>
        /// 拉载值单位
        /// </summary>
        public string LoadUnit
        {
            get => loadunit;
            set => Set(nameof(LoadUnit), ref loadunit, value);
        }

        #endregion

        #endregion

        #region 界面呈现逻辑参数
        private bool enablebalance = false;
        /// <summary>
        /// 允许设置平衡状态
        /// </summary>
        public bool EnableBalance
        {
            get => enablebalance;
            set => Set(nameof(EnableBalance), ref enablebalance, value);
        }

        private bool canusestep = true;
        /// <summary>
        /// 是否允许使用步进模式
        /// </summary>
        public bool CanUseStep
        {
            get => canusestep;
            set => Set(nameof(CanUseStep), ref canusestep, value);
        }


        private Visibility voltshow = Visibility.Visible;
        /// <summary>
        /// 电源数据显示
        /// </summary>
        public Visibility VoltShow
        {
            get => voltshow;
            set => Set(nameof(VoltShow), ref voltshow, value);
        }

        private Visibility loadshow = Visibility.Collapsed;
        /// <summary>
        /// 负载数据显示
        /// </summary>
        public Visibility LoadShow
        {
            get => loadshow;
            set => Set(nameof(LoadShow), ref loadshow, value);
        }

        private Visibility bcshow = Visibility.Collapsed;
        /// <summary>
        /// 是否显示BC相数据
        /// </summary>
        public Visibility BCShow
        {
            get => bcshow;
            set => Set(nameof(BCShow), ref bcshow, value);
        }

        private Visibility setshow = Visibility.Visible;

        public Visibility SetShow
        {
            get => setshow;
            set => Set(nameof(SetShow), ref setshow, value);
        }

        private Visibility loadexshow = Visibility.Visible;
        /// <summary>
        /// 负载额外数据
        /// </summary>
        public Visibility LoadExShow
        {
            get => loadexshow;
            set => Set(nameof(LoadExShow), ref loadexshow, value);
        }

        private bool enablesetblancevalue = false;
        /// <summary>
        /// 是否允许设置不平衡相关参数
        /// </summary>
        public bool EnableSetBlanceValue
        {
            get => enablesetblancevalue;
            set => Set(nameof(EnableSetBlanceValue), ref enablesetblancevalue, value);
        }

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

        protected override void Add(ObservableCollection<ModDeviceReadData> obj)
        {
            if (GlobalModel.NowProcessName == "开关机流程")
            {
                MessageBox.Show($"当前【{GlobalModel.NowProcessName}】无法使用步进功能");
                return;
            }
            base.Add(obj);
        }

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
            //ConditionValues.Clear();

            //if (!SupportConfig.DicProcessConfig.TryGetValue(GlobalModel.NowProcessName, out ProcessConfig_EX value))
            //{
            //    return;
            //}


            //for (int i = 0; i < value.Test_ReadData_Euq.Count; i++)
            //{
            //    var data = value.Test_ReadData_Euq[i];
            //    ConditionValues.Add(new ModDeviceReadData
            //    {
            //        DataInfo = data,
            //    });
            //}

            //if (GlobalModel.NowElectricity == ATE.Common.Electricity.Three)
            //{
            //    for (int i = 0; i < value.Test_ReadData_EX.Count; i++)
            //    {
            //        var data = value.Test_ReadData_EX[i];
            //        ConditionValues.Add(new ModDeviceReadData
            //        {
            //            DataInfo = data,
            //        });
            //    }
            //}

            //for (int i = 0; i < GlobalModel.TestDBCconfig.Test_DBCReceiveSignals.Count; i++)
            //{
            //    var data = GlobalModel.TestDBCconfig.Test_DBCReceiveSignals[i];
            //    ConditionValues.Add(new ModDeviceReadData
            //    {
            //        DataInfo = new ModTestDataInfo { Name = data.Custom_Name},
            //    });
            //}
        }
        #endregion

    }
}
