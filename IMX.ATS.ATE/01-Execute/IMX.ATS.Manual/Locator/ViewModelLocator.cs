using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;

namespace IMX.ATS.Manual
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //测试主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //设备初始化/卸载界面
            SimpleIoc.Default.Register<DeviceInitViewModel>();
            ContentControlManager.Regiter<DeviceInitView>();

            //设备数据监控
            SimpleIoc.Default.Register<MonitorViewModel>();
            ContentControlManager.Regiter<MonitorView>();

            //设备操作
            SimpleIoc.Default.Register<ManualViewModel>();
            ContentControlManager.Regiter<ManualView>();


            #region Manual
            //交流源载一体机
            SimpleIoc.Default.Register < ManualViewModelACSourceLoad>();
            ContentControlManager.Regiter<ManualViewACSourceLoad>();

            //CCCP模拟信号
            SimpleIoc.Default.Register<ManualViewModelAnalogAignals>();
            ContentControlManager.Regiter<ManualViewAnalogAignals>();

            //稳压直流源
            SimpleIoc.Default.Register<ManualViewModelAPU>();
            ContentControlManager.Regiter<ManualViewAPU>();

            //高压直流负载
            SimpleIoc.Default.Register<ManualViewModelDCLoad>();
            ContentControlManager.Regiter<ManualViewDCLoad>();

            //高压直流源
            SimpleIoc.Default.Register<ManualViewModelHVDCSource>();
            ContentControlManager.Regiter<ManualViewHVDCSource>();

            //低压直流负载
            SimpleIoc.Default.Register<ManualViewModelLVDCLoad>();
            ContentControlManager.Regiter<ManualViewLVDCLoad>();

            //产品
            SimpleIoc.Default.Register<ManualViewModelProduct>();
            ContentControlManager.Regiter<ManualViewProduct>();

            //继电器
            SimpleIoc.Default.Register<ManualViewModelRelay>();
            ContentControlManager.Regiter<ManualViewRelay>();

            //信号源
            SimpleIoc.Default.Register<ManualViewModelSignalSource>();
            ContentControlManager.Regiter<ManualViewSignalSource>();

            //温箱
            SimpleIoc.Default.Register<ManualViewModelTempBox>();
            ContentControlManager.Regiter<ManualViewTempBox>();

            //水浴
            SimpleIoc.Default.Register<ManualViewModelWaterBox>();
            ContentControlManager.Regiter<ManualViewWaterBox>();
            
            #endregion
        }

        /// <summary>
        /// 功能测试主
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();


        /// <summary>
        /// 设备初始化/卸载
        /// </summary>
        public DeviceInitViewModel Init => ServiceLocator.Current.GetInstance<DeviceInitViewModel>();

        /// <summary>
        /// 设备数据监控
        /// </summary>
        public MonitorViewModel Monitor => ServiceLocator.Current.GetInstance<MonitorViewModel>();

        /// <summary>
        /// 设备操作
        /// </summary>
        public ManualViewModel Manual => ServiceLocator.Current.GetInstance<ManualViewModel>();


        #region Manual

        /// <summary>
        /// 交流源载一体机
        /// </summary>
        public ManualViewModelACSourceLoad ACSourceLoad => ServiceLocator.Current.GetInstance<ManualViewModelACSourceLoad>();

        /// <summary>
        /// CCCP模拟信号
        /// </summary>
        public ManualViewModelAnalogAignals AnalogAignals => ServiceLocator.Current.GetInstance<ManualViewModelAnalogAignals>();

        /// <summary>
        /// 稳压直流源
        /// </summary>
        public ManualViewModelAPU APU => ServiceLocator.Current.GetInstance<ManualViewModelAPU>();

        /// <summary>
        /// 高压直流负载
        /// </summary>
        public ManualViewModelDCLoad DCLoad => ServiceLocator.Current.GetInstance<ManualViewModelDCLoad>();

        /// <summary>
        /// 高压直流源
        /// </summary>
        public ManualViewModelHVDCSource HVDCSource => ServiceLocator.Current.GetInstance<ManualViewModelHVDCSource>();

        /// <summary>
        /// 低压直流负载
        /// </summary>
        public ManualViewModelLVDCLoad LVDCLoad => ServiceLocator.Current.GetInstance<ManualViewModelLVDCLoad>();

        /// <summary>
        /// 产品
        /// </summary>
        public ManualViewModelProduct Product => ServiceLocator.Current.GetInstance<ManualViewModelProduct>();

        /// <summary>
        /// 继电器
        /// </summary>
        public ManualViewModelRelay Relay => ServiceLocator.Current.GetInstance<ManualViewModelRelay>();

        /// <summary>
        /// 信号源
        /// </summary>
        public ManualViewModelSignalSource SignalSource => ServiceLocator.Current.GetInstance<ManualViewModelSignalSource>();

        /// <summary>
        /// 温箱
        /// </summary>
        public ManualViewModelTempBox TempBox => ServiceLocator.Current.GetInstance<ManualViewModelTempBox>();

        /// <summary>
        /// 水浴
        /// </summary>
        public ManualViewModelWaterBox WaterBox => ServiceLocator.Current.GetInstance<ManualViewModelWaterBox>();

        #endregion
    }
}
