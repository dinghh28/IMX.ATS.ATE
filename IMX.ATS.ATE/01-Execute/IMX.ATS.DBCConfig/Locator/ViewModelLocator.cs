using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace IMX.ATS.DBCConfig
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //项目配置主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //DBC配置选择界面
            SimpleIoc.Default.Register<SelectViewModel>();
            ContentControlManager.Regiter<SelectView>();

            //DBC文件上传界面
            SimpleIoc.Default.Register<FileUpLoadViewModel>();
            ContentControlManager.Regiter<FileUpLoadView>();

            //DBC上报信号配置
            SimpleIoc.Default.Register<DBCReceiveConfigViewModel>();
            ContentControlManager.Regiter<DBCReceiveConfigView>();

            //DBC下发信号配置
            SimpleIoc.Default.Register<DBCSendConfigViewModel>();
            ContentControlManager.Regiter<DBCSendConfigView>();

            //用户自定义下发消息配置信息
            SimpleIoc.Default.Register<CustomConfigViewModel>();
            ContentControlManager.Regiter<CustomConfigView>();

            //DBC配置信息
            SimpleIoc.Default.Register<DBCConfigViewModel>();
            ContentControlManager.Regiter<DBCConfigView>();

            //DBC文件变更
            SimpleIoc.Default.Register<DBCFileChangeViewModel>();
            ContentControlManager.Regiter<DBCFileChangeView>();
        }


        /// <summary>
        /// DBC配置主界面
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// DBC配置选择
        /// </summary>
        public SelectViewModel Select => ServiceLocator.Current.GetInstance<SelectViewModel>();

        /// <summary>
        /// DBC文件变更
        /// </summary>
        public DBCFileChangeViewModel DBCFileChange => ServiceLocator.Current.GetInstance<DBCFileChangeViewModel>();

        /// <summary>
        /// DBC文件上传
        /// </summary>
        public FileUpLoadViewModel FileUpLoad=> ServiceLocator.Current.GetInstance<FileUpLoadViewModel>();

        /// <summary>
        /// DBC上报信号配置
        /// </summary>
        public DBCReceiveConfigViewModel Receive => ServiceLocator.Current.GetInstance<DBCReceiveConfigViewModel>();

        /// <summary>
        /// DBC下发信号配置
        /// </summary>
        public DBCSendConfigViewModel Send => ServiceLocator.Current.GetInstance<DBCSendConfigViewModel>();

        /// <summary>
        /// 用户自定义下发消息配置
        /// </summary>
        public CustomConfigViewModel Custom => ServiceLocator.Current.GetInstance<CustomConfigViewModel>();

        /// <summary>
        /// DBC信号配置
        /// </summary>
        public DBCConfigViewModel DBCConfig => ServiceLocator.Current.GetInstance<DBCConfigViewModel>();


    }
}
