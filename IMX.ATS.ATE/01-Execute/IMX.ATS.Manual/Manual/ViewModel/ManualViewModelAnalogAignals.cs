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
    /// <summary>
    /// CCCP模拟信号
    /// </summary>
    public class ManualViewModelAnalogAignals : WindowViewModelBaseEx
    {
        #region 私有变量


        public List<CCResMode> CCResModeList { get; set; } =
        [
            CCResMode.Res_100,
            CCResMode.Res_220,
            CCResMode.Res_680,
            CCResMode.Res_1000,
            CCResMode.Res_1500,
            CCResMode.Res_2000,
            CCResMode.Res_3300
        ];
        #endregion

        #region 界面绑定

        #region 属性


        private CCResMode ccResMode = CCResMode.Res_220;

        public CCResMode CCResMode
        {
            get => ccResMode;
            set => Set(nameof(CCResMode), ref ccResMode, value);
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

                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.AnalogAignals.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IAnalogAignals operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }


                string InfoString = string.Empty;

                OperateResult result = operate.SetCCResValue(CCResMode);

                if (!result)
                {
                    MessageBox.Show($"设备参数设置异常：【{result.Message}】");
                    return;
                }
                InfoString = $"设置\n [CC阻值]{CCResMode}成功";

                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelAnalogAignals), nameof(SetedValues), InfoString);

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
