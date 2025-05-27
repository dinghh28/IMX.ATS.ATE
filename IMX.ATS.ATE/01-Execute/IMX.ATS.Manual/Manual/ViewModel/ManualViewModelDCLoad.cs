using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Device.DCLoad;
using IMX.Function;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class ManualViewModelDCLoad : WindowViewModelBaseEx
    {
        #region 私有变量

        //private DeviceInfo_ALL DeviceInfo = new DeviceInfo_ALL();

        //设备开关操作
        public List<SetOutPutState> OperateTypeList { get; set; } = new List<SetOutPutState>
        {
            SetOutPutState.ON,
            SetOutPutState.OFF,
            SetOutPutState.Null
        };

        //设备运行模式
        public List<Opaerate_Mode> RunModeList { get; set; } = new List<Opaerate_Mode>
        {
            Opaerate_Mode.CV,
            Opaerate_Mode.CC,
            Opaerate_Mode.CR,
        };

        /// <summary>
        /// 界面操作状态
        /// </summary>
        private bool OperationEnable { get; set; } = true;



        #endregion

        #region 界面绑定


        #region 属性

        private SetOutPutState operateType;
        /// <summary>
        /// 当前动作
        /// </summary>
        public SetOutPutState OperateType
        {
            get => operateType;
            set => Set(nameof(OperateType), ref operateType, value);
        }

        private DeviceOutPutState set_shortstate = DeviceOutPutState.OFF;
        /// <summary>
        /// 短路模式设置
        /// </summary>
        public DeviceOutPutState Set_ShortState
        {
            get => set_shortstate;
            set
            {
                if (Set(nameof(Set_ShortState), ref set_shortstate, value))
                {

                    EnableSetLoadValue = value == DeviceOutPutState.ON ? Visibility.Collapsed : Visibility.Visible;
                    //if (value == DeviceOutPutState.ON)
                    //{
                    //    EnableSetLoadValue = false;
                    //}
                    //else
                    //{
                    //    EnableSetLoadValue = true;
                    //}
                }
            }
        }

        private Opaerate_Mode runModeType;
        /// <summary>
        /// 当前运行模式
        /// </summary>
        public Opaerate_Mode RunModeType
        {
            get => runModeType;
            set
            {
                if (Set(nameof(RunModeType), ref runModeType, value))
                {
                    EnableSlope = value == Opaerate_Mode.CC ? Visibility.Visible : Visibility.Collapsed;
                    EnableILimit = value == Opaerate_Mode.CC ? Visibility.Collapsed : Visibility.Visible;
                    switch (value)
                    {
                        case Opaerate_Mode.CC: LoadUnit = "A"; break;
                        case Opaerate_Mode.CV: LoadUnit = "V"; break;
                        case Opaerate_Mode.CR: LoadUnit = "Ω"; break;
                        default: break;
                    }
                }
            }
        }

        private Visibility enableSetLoadValue;

        /// <summary>
        /// 设置参数显示
        /// </summary>
        public Visibility EnableSetLoadValue
        {
            get => enableSetLoadValue;
            set => Set(nameof(EnableSetLoadValue), ref enableSetLoadValue, value);
        }


        private Visibility enableILimit;

        /// <summary>
        /// 限定电流参数显示
        /// </summary>
        public Visibility EnableILimit
        {
            get => enableILimit;
            set => Set(nameof(EnableILimit), ref enableILimit, value);
        }

        private Visibility enableSlope;

        /// <summary>
        /// 斜率显示
        /// </summary>
        public Visibility EnableSlope
        {
            get => enableSlope;
            set => Set(nameof(EnableSlope), ref enableSlope, value);
        }

        private double set_loadvalue;
        /// <summary>
        /// 设置拉载值
        /// </summary>
        public double Set_LoadValue
        {
            get => set_loadvalue;
            set => Set(nameof(Set_LoadValue), ref set_loadvalue, value);
        }


        private double set_UpLimValue;
        /// <summary>
        /// 设置上限拉载值
        /// </summary>
        public double Set_UpLimValue
        {
            get => set_UpLimValue;
            set => Set(nameof(Set_UpLimValue), ref set_UpLimValue, value);
        }

        private double set_LimValue;
        /// <summary>
        /// 设置限制电流值
        /// </summary>
        public double Set_LimValue
        {
            get => set_LimValue;
            set => Set(nameof(Set_LimValue), ref set_LimValue, value);
        }


        private double set_RiseSploeValue;
        /// <summary>
        /// 设置上升斜率
        /// </summary>
        public double Set_RiseSploeValue
        {
            get => set_RiseSploeValue;
            set => Set(nameof(Set_RiseSploeValue), ref set_RiseSploeValue, value);
        }


        private double set_DownSploeValue;
        /// <summary>
        /// 设置上下降斜率
        /// </summary>
        public double Set_DownSploeValue
        {
            get => set_DownSploeValue;
            set => Set(nameof(Set_DownSploeValue), ref set_DownSploeValue, value);
        }

        private string loadUnit = "A";

        /// <summary>
        /// 拉载值单位
        /// </summary>
        public string LoadUnit
        {
            get => loadUnit;
            set => Set(nameof(LoadUnit), ref loadUnit, value);
        }

        //private string iLimitUnit = "A";

        ///// <summary>
        ///// 限定电流值单位
        ///// </summary>
        //public string ILimitUnit
        //{
        //    get => iLimitUnit;
        //    set => Set(nameof(ILimitUnit), ref iLimitUnit, value);
        //}

        private string upLimitUnit = "A";

        /// <summary>
        /// 上限拉载值单位
        /// </summary>
        public string UpLimitUnit
        {
            get => upLimitUnit;
            set => Set(nameof(UpLimitUnit), ref upLimitUnit, value);
        }
        #endregion

        #region 指令

        public RelayCommand SetValues => new RelayCommand(SetedValues);
        #endregion
        #endregion

        #region 私有方法

        private void SetedValues()
        {
            try
            {
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.DCLoad.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IDCLoad operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                string InfoString = string.Empty;
                if (Set_ShortState == DeviceOutPutState.ON)
                {
                    operate.SetShort(Set_ShortState).AttachIfFailed(result => { MessageBox.Show($"设备短路设置异常：【{result.Message}】"); })
                        .AttachIfSucceed(result =>
                        {
                            InfoString = $"设备短路设置成功";
                            SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelDCLoad), nameof(SetedValues), InfoString);
                        });
                    return;
                }

                OperateResult result;
                if (RunModeType == Opaerate_Mode.CC)
                {
                    result = operate.SetModel(RunModeType)
                        .And(operate.SetLoadValue(Set_LoadValue))
                        .And(operate.SetParameters(Set_UpLimValue, Set_LimValue))
                        .And(operate.SetCurrSLEW_POSitive(Set_RiseSploeValue))
                        .And(operate.SetCurrSLEW_NEGative(Set_DownSploeValue));
                    InfoString = $"设置\n[拉载模式]{RunModeType}\n" +
                        $"[拉载值]{Set_LoadValue}\n" +
                        $"[拉载上限值]{Set_UpLimValue}\n" +
                        $"[限制电流值]{Set_LimValue}\n" +
                        $"[上升斜率]{Set_RiseSploeValue}\n" +
                        $"[下降斜率]{Set_DownSploeValue}";
                }
                else
                {
                    result = operate.SetModel(RunModeType)
                        .And(operate.SetLoadValue(Set_LoadValue))
                        .And(operate.SetParameters(Set_UpLimValue, Set_LimValue));
                    InfoString = $"设置\n[拉载模式]{RunModeType}\n" +
                        $"[拉载值]{Set_LoadValue}\n" +
                        $"[拉载上限值]{Set_UpLimValue}\n" +
                        $"[限制电流值]{set_LimValue}";
                }

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }

                InfoString = $"{InfoString}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelDCLoad), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelDCLoad), nameof(SetedValues), $"设备已{InfoString}");
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelDCLoad), nameof(SetedValues), ex);
                MessageBox.Show("设置失败", "参数设置", MessageBoxButton.OK);
                OperationEnable = true;
            }
        }


        #endregion

        #region 公有方法

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

        public ManualViewModelDCLoad()
        {

        }

        #endregion
    }

}
