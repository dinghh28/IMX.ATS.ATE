using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;
using IMX.ATS.ATEConfig.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace IMX.ATS.ATEConfig
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //项目选择
            SimpleIoc.Default.Register<ProjectSelectViewModel>();
            ContentControlManager.Regiter<ProjectSelectView>();

            //项目选择
            SimpleIoc.Default.Register<ProjectInfoViewModel>();
            ContentControlManager.Regiter<ProjectInfoView>();

            //项目配置主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //DBC信号配置
            SimpleIoc.Default.Register<DBCConfigViewModel>();
            ContentControlManager.Regiter<DBCConfigView>();

            //DBC文件上传
            SimpleIoc.Default.Register<DBCFileUploadViewModel>();
            ContentControlManager.Regiter<DBCFileUploadView>();

            //DBC文件变更
            SimpleIoc.Default.Register<DBCFileChangeViewModel>();
            ContentControlManager.Regiter<DBCFileChangeView>();

            ////试验保护配置
            //SimpleIoc.Default.Register<PCProtectConfigViewModel>();
            //ContentControlManager.Regiter<PCProtectConfigView>();


            //试验步骤配置
            SimpleIoc.Default.Register<TestProcessViewModel>();
            ContentControlManager.Regiter<TestProcessView>();

            //试验步骤配置
            SimpleIoc.Default.Register<NewTestProcessViewModel>();
            ContentControlManager.Regiter<NewTestProcessView>();

            //试验方案配置
            SimpleIoc.Default.Register<TestProgrammeViewModel>();
            ContentControlManager.Regiter<TestProgrammeView>();

            

            #region 流程步骤配置
            //直流稳压源配置
            SimpleIoc.Default.Register<FunViewModelAPU>();
            ContentControlManager.Regiter<FunViewAPU>();

            //高压直流压源配置
            SimpleIoc.Default.Register<FunViewModelHVDCSource>();
            ContentControlManager.Regiter<FunViewHVDCSource>();

            //弹窗配置
            SimpleIoc.Default.Register<FunViewModelPOPUP>();
            ContentControlManager.Regiter<FunViewPOPUP>();

            //直流负载配置
            SimpleIoc.Default.Register<FunViewModelDCLoad>();
            ContentControlManager.Regiter<FunViewDCLoad>();

            //交流负载配置
            SimpleIoc.Default.Register<FunViewModelACSource>();
            ContentControlManager.Regiter<FunViewACSource>();

            //产品指令配置
            SimpleIoc.Default.Register<FunViewModelProduct>();
            ContentControlManager.Regiter<FunViewProduct>();

            //产品读取结果配置
            SimpleIoc.Default.Register<FunViewModelProductResult>();
            ContentControlManager.Regiter<FunViewProductResult>();


            //工装读取结果配置
            SimpleIoc.Default.Register<FunViewModelEquipmentResult>();
            ContentControlManager.Regiter<FunViewEquipmentResult>();
            #endregion
        }

        //#region PC-项目配置
        /// <summary>
        /// 项目信息选择
        /// </summary>
        public ProjectSelectViewModel ProjectSelect => ServiceLocator.Current.GetInstance<ProjectSelectViewModel>();

        /// <summary>
        /// 项目信息展示配置页
        /// </summary>
        public ProjectInfoViewModel ProjectInfo => ServiceLocator.Current.GetInstance<ProjectInfoViewModel>();

        /// <summary>
        /// 项目配置主界面
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// DBC信号配置
        /// </summary>
        public DBCConfigViewModel DBCConfig => ServiceLocator.Current.GetInstance<DBCConfigViewModel>();

        /// <summary>
        /// DBC文件上传
        /// </summary>
        public DBCFileUploadViewModel DBCFileUpload => ServiceLocator.Current.GetInstance<DBCFileUploadViewModel>();

        /// <summary>
        /// DBC文件变更
        /// </summary>
        public DBCFileChangeViewModel DBCFileChange => ServiceLocator.Current.GetInstance<DBCFileChangeViewModel>();

        ///// <summary>
        ///// 试验保护配置
        ///// </summary>
        //public PCProtectConfigViewModel ProtectConfig => ServiceLocator.Current.GetInstance<PCProtectConfigViewModel>();

        /// <summary>
        /// 试验步骤配置
        /// </summary>
        public TestProcessViewModel TestProcess => ServiceLocator.Current.GetInstance<TestProcessViewModel>();

        /// <summary>
        /// 新建试验流程
        /// </summary>
        public NewTestProcessViewModel NewTestProcess => ServiceLocator.Current.GetInstance<NewTestProcessViewModel>();

        /// <summary>
        /// 试验方案配置
        /// </summary>
        public TestProgrammeViewModel TestProgramme => ServiceLocator.Current.GetInstance<TestProgrammeViewModel>();

        //#endregion

        #region FUN

        ////工装设备操作模板
        //public FunViewModelEquip FunEquip => ServiceLocator.Current.GetInstance<FunViewModelEquip>();


        /// <summary>
        /// 交流源配置模板
        /// </summary>
        public FunViewModelACSource FunACSource=>ServiceLocator.Current.GetInstance<FunViewModelACSource>();

        /// <summary>
        /// 稳压直流源配置模板
        /// </summary>
        public FunViewModelAPU FunAPU=> ServiceLocator.Current.GetInstance<FunViewModelAPU>();

        /// <summary>
        /// 高压直流源配置模板
        /// </summary>
        public FunViewModelHVDCSource FunHVDCSource => ServiceLocator.Current.GetInstance<FunViewModelHVDCSource>();

        /// <summary>
        /// 弹窗配置模板
        /// </summary>
        public FunViewModelPOPUP FunPOPUP => ServiceLocator.Current.GetInstance<FunViewModelPOPUP>();

        /// <summary>
        /// 直流负载配置模板
        /// </summary>
        public FunViewModelDCLoad FunDCLoad => ServiceLocator.Current.GetInstance<FunViewModelDCLoad>();

        /// <summary>
        /// 产品指令配置模板
        /// </summary>
        public FunViewModelProduct FunProduct=>ServiceLocator.Current.GetInstance<FunViewModelProduct>();


        /// <summary>
        /// 产品结果读取模板
        /// </summary>
        public FunViewModelProductResult FunProductResult => ServiceLocator.Current.GetInstance<FunViewModelProductResult>();

        /// <summary>
        /// 工装结果读取模板
        /// </summary>
        public FunViewModelEquipmentResult FunEquipmentResult => ServiceLocator.Current.GetInstance<FunViewModelEquipmentResult>();
        
        #endregion

    }
}
