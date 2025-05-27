using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class ManualViewModelSignalSource : WindowViewModelBaseEx
    {
        #region 公有变量

        //设备开关操作
        public List<SetOutPutState> OperateTypeList { get; set; } = new List<SetOutPutState>
        {
            SetOutPutState.ON,
            SetOutPutState.OFF,
            SetOutPutState.Null
        };

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


        private double set_FreqValue;
        /// <summary>
        /// 频率
        /// </summary>
        public double Set_FreqValue
        {
            get => set_FreqValue;
            set => Set(nameof(Set_FreqValue), ref set_FreqValue, value);
        }


        private double set_AmplValue;
        /// <summary>
        /// 幅度
        /// </summary>
        public double Set_AmplValue
        {
            get => set_AmplValue;
            set => Set(nameof(Set_AmplValue), ref set_AmplValue, value);
        }

        private double set_DutyValue;
        /// <summary>
        /// 占空比
        /// </summary>
        public double Set_DutyValue
        {
            get => set_DutyValue;
            set => Set(nameof(Set_DutyValue), ref set_DutyValue, value);
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
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.SignalSource.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is ISignalSource operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }
                string InfoString = string.Empty;

                OperateResult result = operate.Set_Value(Set_FreqValue, Set_AmplValue,Set_DutyValue);

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }
                InfoString = $"设置\n [输出频率]{Set_FreqValue}\n [输出幅度]{Set_AmplValue}\n [输出占空比]{Set_DutyValue}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelAnalogAignals), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelAnalogAignals), nameof(SetedValues), $"设备已{InfoString}");
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelAnalogAignals), nameof(SetedValues), ex);

                MessageBox.Show("设置失败", "参数设置", MessageBoxButton.OK);
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
