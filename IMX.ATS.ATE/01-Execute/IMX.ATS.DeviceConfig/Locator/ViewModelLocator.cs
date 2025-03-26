using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;

namespace IMX.ATS.DeviceConfig
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //设备配置主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            ////设备初始化/卸载界面
            //SimpleIoc.Default.Register<DeviceInitViewModel>();
            //ContentControlManager.Regiter<DeviceInitView>();

        }

        /// <summary>
        /// 设备配置主界面
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();


        ///// <summary>
        ///// 设备初始化/卸载
        ///// </summary>
        //public DeviceInitViewModel Init => ServiceLocator.Current.GetInstance<DeviceInitViewModel>();
    }
}
