using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
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
    public class ManualViewModel : WindowViewModelBaseEx
    {
        #region 私有变量

        private readonly Dictionary<string, string> dicManualItems = new Dictionary<string, string>
        {
            { "交流电源载一体机", "ACSourceLoad"},
            { "高压直流电源", "HVDCSource"},
            { "直流稳压电源", "APU"},
            { "高压直流负载", "DCLoad"},
            { "低压直流负载", "LVDCLoad"},
            { "信号发生器", "SignalSource"},
            { "CCCP模拟信号", "AnalogAignals"},
            { "继电器", "Relay"},
            { "产品", "Product"},
            { "水浴", "WaterBox"},
            { "温箱", "TempBox"},
        };

        #endregion

        #region 公共属性

        #region 界面绑定属性

        private FrameworkElement content;
        /// <summary>
        /// 方案各配置界面
        /// </summary>
        public FrameworkElement Content
        {
            get => content;
            set => Set(nameof(Content), ref content, value);
        }


        private ObservableCollection<OperateViewModel> operationViews = new ObservableCollection<OperateViewModel>();

        public ObservableCollection<OperateViewModel> OperationViews
        {
            get => operationViews;
            set => Set(nameof(OperationViews), ref operationViews, value);
        }

        #endregion

        #region 界面绑定指令

        #endregion

        #endregion

        #region 私有方法
        private void DoNavChange(object obj)
        {
            try
            {
                Type win = Type.GetType($"IMX.ATS.Manual.ManualView{obj}");
                Type model = Type.GetType($"IMX.ATS.Manual.ManualViewModel{obj}");
                Content = ContentControlManager.GetControl(win, ((ViewModelLocator)Application.Current.FindResource("Locator")).GetModel(model));
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, nameof(ManualViewModel), nameof(DoNavChange), ex);
                MessageBox.Show($"界面切换异常:{ex.GetMessage()}");
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

        public ManualViewModel()
        {
            //GlobalModel.DicDeviceInfo = new Dictionary<string, DeviceInfo_ALL>
            //{
            //    { "ACSourceLoad" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.ACSourceLoad,DeviceModel="AN23600"} } },
            //    { "APU" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true, DeviceType = Device.Common.EDeviceType.APU,DeviceModel="AN3800"}} },
            //    { "DCLoad" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.DCLoad,DeviceModel="IT8900"} } },
            //    { "LVDCLoad" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.LVDCLoad, DeviceModel = "IT8900"} } },
            //    { "HVDCSource" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.HVDCSource, DeviceModel = "IT89300"} } },
            //    { "AnalogAignals" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.AnalogAignals, DeviceModel = "IT89800"} } },
            //    //{ "Relay" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.Relay, DeviceModel = "ZS24"} } },
            //    { "SignalSource" , new DeviceInfo_ALL(){ Config=new SysteamSupportDeviceConfigInfo(){ EnableManual=true,DeviceType=Device.Common.EDeviceType.SignalSource, DeviceModel = "AN300"} } },

            //};
            foreach (var item in GlobalModel.DicDeviceInfo)
            {
                if (!item.Value.Config.EnableManual)
                {
                    continue;
                }
                OperationViews.Add(new OperateViewModel()
                {
                    Name = $"{item.Value.Config.DeviceType.GetDescription()}[{ item.Value.Config.DeviceModel}]",
                    VisibilityEnable = item.Value.Config.EnableManual ? Visibility.Visible : Visibility.Collapsed,
                    Description = item.Key.ToString(),
                    SelectView = new RelayCommand<object>(DoNavChange)
                });
            }
        }

        #endregion
    }
    public class OperateViewModel : ExtendViewModelBase
    {
        /// <summary>
        /// 界面名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 界面显示
        /// </summary>
        public Visibility VisibilityEnable { get; set; }

        /// <summary>
        /// 界面描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 步骤切换
        /// </summary>
        public RelayCommand<object> SelectView { get; set; }

    }
}
