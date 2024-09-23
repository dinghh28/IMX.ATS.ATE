using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.ATS.ATEConfig
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //项目选择
            SimpleIoc.Default.Register<ProjectSelectViewModel>();
            ContentControlManager.Regiter<ProjectSelectView>();

            ////项目选择
            //SimpleIoc.Default.Register<PCProjectInfoViewModel>();
            //ContentControlManager.Regiter<PCProjectInfoView>();

            //项目配置主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //DBC信号配置
            SimpleIoc.Default.Register<DBCConfigViewModel>();
            ContentControlManager.Regiter<DBCConfigView>();

            ////DBC文件上传
            //SimpleIoc.Default.Register<PCDBCFileUploadViewModel>();
            //ContentControlManager.Regiter<PCDBCFileUploadView>();

            ////DBC文件变更
            //SimpleIoc.Default.Register<PCDBCFileChangeViewModel>();
            //ContentControlManager.Regiter<PCDBCFileChangeView>();

            ////试验保护配置
            //SimpleIoc.Default.Register<PCProtectConfigViewModel>();
            //ContentControlManager.Regiter<PCProtectConfigView>();


            //试验步骤配置
            SimpleIoc.Default.Register<TestProcessViewModel>();
            ContentControlManager.Regiter<TestProcessView>();

            ////工装设备操作模板
            //SimpleIoc.Default.Register<FunViewModelEquip>();
            //ContentControlManager.Regiter<FunViewEquip>();

            //SimpleIoc.Default.Register<FunViewModelProduct>();
            //ContentControlManager.Regiter<FunViewProduct>();
        }

        //#region PC-项目配置
        /// <summary>
        /// 项目信息选择
        /// </summary>
        public ProjectSelectViewModel ProjectSelect => ServiceLocator.Current.GetInstance<ProjectSelectViewModel>();

        ///// <summary>
        ///// 项目信息展示配置页
        ///// </summary>
        //public PCProjectInfoViewModel ProjectInfo => ServiceLocator.Current.GetInstance<PCProjectInfoViewModel>();

        /// <summary>
        /// 项目配置主界面
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// DBC信号配置
        /// </summary>
        public DBCConfigViewModel DBCConfig => ServiceLocator.Current.GetInstance<DBCConfigViewModel>();

        ///// <summary>
        ///// DBC文件上传
        ///// </summary>
        //public PCDBCFileUploadViewModel DBCFileUpload => ServiceLocator.Current.GetInstance<PCDBCFileUploadViewModel>();

        ///// <summary>
        ///// DBC文件变更
        ///// </summary>
        //public PCDBCFileChangeViewModel DBCFileChange => ServiceLocator.Current.GetInstance<PCDBCFileChangeViewModel>();

        ///// <summary>
        ///// 试验保护配置
        ///// </summary>
        //public PCProtectConfigViewModel ProtectConfig => ServiceLocator.Current.GetInstance<PCProtectConfigViewModel>();

        /// <summary>
        /// 试验步骤配置
        /// </summary>
        public TestProcessViewModel TestProcess => ServiceLocator.Current.GetInstance<TestProcessViewModel>();

        //#endregion

        //#region FUN

        ////工装设备操作模板
        //public FunViewModelEquip FunEquip => ServiceLocator.Current.GetInstance<FunViewModelEquip>();

        ////产品指令操作模版
        //public FunViewModelProduct FunEroduct => ServiceLocator.Current.GetInstance<FunViewModelProduct>();

        //#endregion

    }
}
