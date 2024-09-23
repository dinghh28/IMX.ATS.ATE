using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;

namespace IMX.ATS.Lander
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {

            #region Main

            //登录
            SimpleIoc.Default.Register<LoginViewModel>();
            ContentControlManager.Regiter<LoginView>();

            //功能选择门户
            SimpleIoc.Default.Register<PortalViewModel>();
            ContentControlManager.Regiter<PortalView>();


            #endregion
        }

        /// <summary>
        /// 功能选择门户
        /// </summary>
        public PortalViewModel Portal => ServiceLocator.Current.GetInstance<PortalViewModel>();

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
    }
}
