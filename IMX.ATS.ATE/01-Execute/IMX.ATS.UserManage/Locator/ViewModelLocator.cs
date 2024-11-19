using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.UserManage
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //用户管理
            SimpleIoc.Default.Register<UserMainViewModel>();
            ContentControlManager.Regiter<UserMainView>();


            //新增用户
            SimpleIoc.Default.Register<NewUserViewModel>();
            ContentControlManager.Regiter<NewUserView>();
        }

        public UserMainViewModel User => ServiceLocator.Current.GetInstance<UserMainViewModel>();

        public NewUserViewModel NewUser => ServiceLocator.Current.GetInstance<NewUserViewModel>();
    }
}
