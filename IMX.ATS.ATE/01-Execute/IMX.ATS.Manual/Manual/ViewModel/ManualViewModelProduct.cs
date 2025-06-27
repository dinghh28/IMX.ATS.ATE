using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.Device.Base;
using IMX.Device.Base.DeviceInerfaces;
using IMX.Device.Common;
using IMX.Device.Product;
using IMX.Funciton;
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
    public class ManualViewModelProduct : WindowViewModelBaseEx
    {
        #region 私有变量

        #endregion

        #region 界面绑定

        #region 属性
        ///// <summary>
        ///// 设备地址列表
        ///// </summary>
        //public List<string> ProductIndexs { get; } = new List<string>() { "ALL", "Product_0", "Product_1", "Product_2" };

        //private string productIndex = "Product_0";
        ///// <summary>
        ///// 设备编号
        ///// </summary>
        //public string ProductIndex
        //{
        //    get
        //    {
        //        return productIndex;
        //    }
        //    set
        //    {
        //        var info = infos.Find(x => x.Product_Index == value);
        //        if (info == null)
        //        {
        //            System.Windows.Forms.MessageBox.Show($"设备不存在");
        //            return;
        //        }

        //        if (info.SendValueModel.Count < 1)
        //        {
        //            System.Windows.Forms.MessageBox.Show($"该设备DBC发送帧为空，请先配置！");
        //            return;
        //        }

        //        if (Set(nameof(ProductIndex), ref productIndex, value))
        //        {
        //            GetCommunicationState(OperationName, true);
        //            GetCommunicationState(RevOperationName, false);
        //        }
        //    }
        //}


        private bool isSelectAll = false;
        /// <summary>
        /// 是否全选指令
        /// </summary>
        public bool IsSelectAll
        {
            get => isSelectAll;
            set
            {
                if (Set(nameof(IsSelectAll), ref isSelectAll, value))
                {
                    SendSignals.ToList().FindAll(model => model.IsSelected = value);
                }
            }
        }

        private ObservableCollection<SendSignalModel> sendSignals = new ObservableCollection<SendSignalModel>();
        /// <summary>
        /// 下发信号
        /// </summary>
        public ObservableCollection<SendSignalModel> SendSignals
        {
            get
            {
                return sendSignals;
            }
            set => Set(nameof(SendSignals), ref sendSignals, value);
        }

        private string operationName = "发送指令";
        /// <summary>
        /// 指令下发
        /// </summary>
        public string OperationName
        {
            get => operationName;
            set => Set(nameof(OperationName), ref operationName, value);
        }

        private string revOperationName = "接收报文";
        /// <summary>
        /// 接收报文
        /// </summary>
        public string RevOperationName
        {
            get => revOperationName;
            set => Set(nameof(RevOperationName), ref revOperationName, value);
        }


        #endregion

        #region 指令

        public RelayCommand SaveMessage => new RelayCommand(SaveMessageOperate);

        public RelayCommand OperateMessage => new RelayCommand(OperateMesssaegeIsSend);

        public RelayCommand OperateRevMessage => new RelayCommand(OperateMesssaegeIsRev);
        #endregion
        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        private void OperateMesssaegeIsSend()
        {
            bool issend = OperationName == "发送指令";

            if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.Product.ToString(), out DeviceInfo_ALL deviceInfo)))
            {
                MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                return;
            }

            if (!(deviceInfo.DeviceOperate is Product_CAN_Operate))
            {
                MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                return;
            }

            if ((deviceInfo.DeviceOperate as Product_CAN_Operate).IsInitOK)
            {
                var rlt = issend ? (deviceInfo.DeviceOperate as Product_CAN_Operate).StartCommunication() : (deviceInfo.DeviceOperate as Product_CAN_Operate).StopCommunication();
            }

            OperationName = issend ? "停止发送" : "发送指令";
        }


        private void OperateMesssaegeIsRev()
        {
            bool isrecvice = RevOperationName == "接收报文";

            if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.Product.ToString(), out DeviceInfo_ALL deviceInfo)))
            {
                MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                return;
            }

            if (!(deviceInfo.DeviceOperate is Product_CAN_Operate))
            {
                MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                return;
            }

            if ((deviceInfo.DeviceOperate as Product_CAN_Operate).IsInitOK)
            {
                var rlt = (deviceInfo.DeviceOperate as Product_CAN_Operate).SetReceiveState(isrecvice);
            }

            RevOperationName = isrecvice ? "停止接收" : "接收报文";
        }


        private void SaveMessageOperate()
        {

            if (!(GlobalModel.DicDeviceInfo.TryGetValue(EDeviceType.Product.ToString(), out DeviceInfo_ALL deviceInfo)))
            {
                MessageBox.Show($"设备初始化异常：【{deviceInfo.GetType()}】");
                return;
            }

            if (!(deviceInfo.DeviceOperate is Product_CAN_Operate device))
            {
                MessageBox.Show($"设备类型异常：【{deviceInfo.DeviceOperate.GetType()}】");
                return;
            }

            if (device.IsInitOK)
            {
                SavedMessage(device);
            }
        }

        private void SavedMessage(Product_CAN_Operate device)
        {


            SendSignals?.ToList().ForEach(x =>
            {
                OperateResult rlt = null;
                //增加或删除发送信号
                rlt = x.DBCSignal.IsSend ?
                device
                .StartSignalSend(x.DBCSignal.SignalName)
                .AttachIfFailed(result =>
                {
                    MessageBox.Show($"产品增加下发信号失败:{result.Message}");
                })
                : device
                .StopSignalSend(x.DBCSignal.SignalName)
                .AttachIfFailed(result =>
                {
                    MessageBox.Show($"产品删除下发信号失败:{result.Message}");
                });

                //修改发送信号值
                device
                .SetSendSignalValue(x.DBCSignal.SignalName, Convert.ToDouble(x.DBCSignal.SignalValue))
                .AttachIfFailed(result =>
                {
                    MessageBox.Show($"产品修改发送信号值失败:{result.Message}");
                });
            });

            //计算下发列表
            device
                .CalcMessageValue()
                .AttachIfFailed(result =>
                {
                    MessageBox.Show($"产品信号计算下发值失败:{result.Message}");
                });
        }

        //private void GetCommunicationState(string state, bool issend)
        //{
        //    if (issend)
        //    {
        //        if (ProductIndex == "ALL")
        //        {
        //            try
        //            {
        //                ProductIndexs.ForEach(x =>
        //                {
        //                    if (x.Contains("Product") && ((GlobalModel.DicDeviceOperates[x] as Product_CAN_Operate).IsInitOK))
        //                    {
        //                        if (!(GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsSendData)
        //                        {
        //                            state = "发送指令";
        //                            return;
        //                        }
        //                    }
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"产品发送状态获取失败:{ex.GetMessage()}");
        //            }

        //            state = "停止发送";
        //        }
        //        else
        //        {
        //            try
        //            {
        //                if (((GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsInitOK))
        //                {
        //                    state = (GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsSendData ? "停止发送" : "发送指令";
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"产品发送状态获取失败:{ex.GetMessage()}");
        //                state = "发送指令";
        //            }

        //        }

        //        return;
        //    }

        //    if (ProductIndex == "ALL")
        //    {
        //        try
        //        {
        //            ProductIndexs.ForEach(x =>
        //            {
        //                if (x.Contains("Product") && ((GlobalModel.DicDeviceOperates[x] as Product_CAN_Operate).IsInitOK))
        //                {
        //                    if (!(GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsReceiveData)
        //                    {
        //                        state = "接收报文";
        //                        return;
        //                    }
        //                }
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"产品接收状态获取失败:{ex.GetMessage()}");
        //        }

        //        state = "停止接收";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (((GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsInitOK))
        //            {
        //                state = (GlobalModel.DicDeviceOperates[ProductIndex] as Product_CAN_Operate).IsReceiveData ? "停止接收" : "接收报文";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"产品接收状态获取失败:{ex.GetMessage()}");
        //            state = "接收报文";
        //        }
        //    }
        //}

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

    public class SendSignalModel : ExtendViewModelBase
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;// = DBCSignal.IsSend;
            set
            {
                if (Set(nameof(IsSelected), ref isSelected, value))
                {
                    DBCSignal.IsSend = value;
                }
            }
        }

        private string sendvalue;

        public string SendValue
        {
            get => sendvalue;// = DBCSignal.IsSend;
            set
            {
                if (Set(nameof(SendValue), ref sendvalue, value))
                {
                    DBCSignal.SignalValue = value;
                }
            }
        }


        private DBCSendSignal dbcsignal = new DBCSendSignal();

        public DBCSendSignal DBCSignal
        {
            get { return dbcsignal; }
            set { dbcsignal = value; }
        }
    }
}
