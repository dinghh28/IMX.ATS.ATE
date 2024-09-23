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
        }

        /// <summary>
        /// 功能测试主界面
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
