using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Base;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class ManualViewModelHVDCSource : WindowViewModelBaseEx
    {
        #region 私有变量

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

        private double set_loadvalue;
        /// <summary>
        /// 设置拉载值
        /// </summary>
        public double Set_LoadValue
        {
            get => set_loadvalue;
            set => Set(nameof(Set_LoadValue), ref set_loadvalue, value);
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

        private string loadUnit = "A";

        /// <summary>
        /// 拉载值单位
        /// </summary>
        public string LoadUnit
        {
            get => loadUnit;
            set => Set(nameof(LoadUnit), ref loadUnit, value);
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

                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.HVDCSource.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IHVDCSource operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                string InfoString = string.Empty;

                OperateResult result = operate.SetMode(RunModeType)
                      .And(operate.SetValue(Set_LoadValue, set_LimValue));
                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }

                InfoString = $"设置\n[启动电压]{Set_LoadValue}\n[限制电流]{set_LimValue}成功";
                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelHVDCSource), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelHVDCSource), nameof(SetedValues), $"设备已{InfoString}");
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

        #endregion

    }
}
