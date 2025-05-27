using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common.Enumerations;
using IMX.Device.Common;
using IMX.Function.Base;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IMX.Device.Base;

namespace IMX.ATS.Manual
{
    public class ManualViewModelAPU : WindowViewModelBaseEx
    {
        #region 私有变量

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

        private double set_VolValue;
        /// <summary>
        /// 设置输出电压值
        /// </summary>
        public double Set_VolValue
        {
            get => set_VolValue;
            set => Set(nameof(Set_VolValue), ref set_VolValue, value);
        }


        private double set_CurValue;
        /// <summary>
        /// 设置输出电流值
        /// </summary>
        public double Set_CurValue
        {
            get => set_CurValue;
            set => Set(nameof(Set_CurValue), ref set_CurValue, value);
        }

        #endregion

        #region 指令

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
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.APU.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IAPU operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                string InfoString = string.Empty;

                OperateResult result;
                result = operate.SetValue(Set_VolValue, Set_CurValue);

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }

                InfoString = $"设置\n[输出电压]{Set_VolValue}\n[输出电流]{Set_CurValue}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelAPU), nameof(SetedValues), InfoString);

                if (OperateType != SetOutPutState.Null)
                {
                    operate.SetOnOff(OperateType == SetOutPutState.ON ? DeviceOutPutState.ON : DeviceOutPutState.OFF);

                    InfoString = OperateType == SetOutPutState.ON ? "打开" : "关闭";

                    SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelAPU), nameof(SetedValues), $"设备已{InfoString}");
                }
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(ManualViewModelAPU), nameof(SetedValues), ex);
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
