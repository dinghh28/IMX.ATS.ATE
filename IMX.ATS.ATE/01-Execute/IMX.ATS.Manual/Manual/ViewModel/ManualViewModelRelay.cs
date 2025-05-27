using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Device.Base;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common;
using IMX.Device.Common.Enumerations;
using IMX.Device.Relay;
using IMX.Logger;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.Manual
{
    public class ManualViewModelRelay : WindowViewModelBaseEx
    {
        #region 公有变量
        /// <summary>
        /// 继电器状态
        /// </summary>
        public char[] SetLoadValueLists { get; set; } = { '0', '0', '0' };

        #endregion

        #region 界面绑定

        #region 属性

        private ObservableCollection<RelayboardShow> relayboards = new ObservableCollection<RelayboardShow>();
        /// <summary>
        /// 继电器状态表
        /// </summary>
        public ObservableCollection<RelayboardShow> Relayboards
        {
            get
            {
                for (int i = 0; i < relayboards?.Count; i++)
                {
                    //relayboards[i].IsOn = (Func.Config as FunConfig_Relay).Set_OutPutStates[i];
                    relayboards[i].Name = $"通道{i + 1}";
                    relayboards[i].Index = i;
                    relayboards[i].SetLoadValueLists = SetLoadValueLists;
                }
                return relayboards;
            }
            set
            {
                if (Set(nameof(Relayboards), ref relayboards, value))
                {
                    //SetLoadValue(RelayMonitorModel.RelayMonitorAdress);
                }
            }
        }
        #endregion

        #region 指令

        public RelayCommand<object> ReadState => new RelayCommand<object>(DoReadRelayAllState);

        #endregion
        #endregion

        #region 私有方法

        private void DoReadRelayAllState(object obj)
        {
            //try
            //{
            //    OperateResult<List<ModDeviceReadData>> operateResult = GlobalModel.DicDeviceOperates[SelectedAddress].Device_ReadAll();
            //    if (!operateResult)
            //    {
            //        MessageBox.Show($"继电器状态读取异常：{operateResult.Message}");
            //    }
            //    string modRelayData = Convert.ToString(Convert.ToInt32(operateResult.Data), 2).PadLeft(16, '0');
            //    for (int i = 0; i < modRelayData.Length; i++)
            //    {
            //        Relayboards[i].IsOn = modRelayData[i].ToString() == "0";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"继电器状态读取异常：{ex.GetMessage()}");
            //}
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
        public ManualViewModelRelay()
        {
            relayboards.Clear();

            for (int i = 0; i < 3; i++)
            {
                relayboards.Add(new RelayboardShow { Name = $"通道{i + 1}", Index = i, SetLoadValueLists = SetLoadValueLists, RelayMonitorAdress = "Relay_0" });
            }
        }


        #endregion
    }

    public class RelayboardShow : ExtendViewModelBase
    {
        /// <summary>
        /// 继电器名字
        /// </summary>
        public string Name { get; set; } = "通道1";

        public int Index { get; set; } = 0;

        public char[] SetLoadValueLists { get; set; } = new char[16];

        public string RelayMonitorAdress { get; set; }

        private bool isOn = false;
        /// <summary>
        /// 继电器状态
        /// </summary>
        public bool IsOn
        {
            get => isOn;
            set
            {
                if (Set(nameof(IsOn), ref isOn, value))
                {
                    SetLoadValue(RelayMonitorAdress);
                }
            }
        }

        private void SetLoadValue(string address)
        {
            try
            {
                if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.Relay.ToString(), out DeviceInfo_ALL deviceInfo)))
                {
                    MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                    return;
                }

                if (!(deviceInfo.DeviceOperate is IRelay operate))
                {
                    MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                    return;
                }

                //SetLoadValueLists[15 - Index] = IsOn ? '1' : '0';

                OperateResult operateResult = operate.Device_Set((uint)Index, IsOn ? DeviceOutPutState.ON : DeviceOutPutState.OFF);
                if (!operateResult)
                {
                    //LastError = operateResult.Message;
                    //SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(ManualViewModelRelay), $" GlobalModel.DicDeviceOperates[{address}].Device_Set", operateResult.Message);
                    MessageBox.Show(operateResult.Message);
                    //return OperateResult.Failed(LastError);
                }
                SuperDHHLoggerManager.Info(LoggerType.TESTLOG, nameof(ManualViewModelRelay), nameof(SetLoadValue), $"设置参数值成功!设置值：{(IsOn ? DeviceOutPutState.ON : DeviceOutPutState.OFF)}");
                //return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                //SuperDHHLoggerManager.Error(LoggerType.DEVICELOG, nameof(ManualViewModelRelay), $" GlobalModel.DicDeviceOperates[{address}].Device_Set", ex.Message);
                MessageBox.Show($"继电器设置失败：{ex.GetMessage()}");
                //return OperateResult.Failed(LastError);
            }

        }
    }
}
