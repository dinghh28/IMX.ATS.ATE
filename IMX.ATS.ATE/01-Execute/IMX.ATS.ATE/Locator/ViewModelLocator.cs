using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;

namespace IMX.ATS.ATE
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

        }

        /// <summary>
        /// 功能测试主
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();


        /// <summary>
        /// 设备初始化/卸载
        /// </summary>
        public DeviceInitViewModel Init => ServiceLocator.Current.GetInstance<DeviceInitViewModel>();
    }
}
