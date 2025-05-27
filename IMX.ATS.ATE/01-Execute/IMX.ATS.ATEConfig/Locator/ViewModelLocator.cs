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

            //项目信息配置
            SimpleIoc.Default.Register<ProjectInfoViewModel>();
            ContentControlManager.Regiter<ProjectInfoView>();

            //项目配置主界面
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //DBC配置选择界面
            SimpleIoc.Default.Register<SelectDBCViewModel>();
            ContentControlManager.Regiter<SelectDBCView>();

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

            //固定试验步骤配置
            SimpleIoc.Default.Register<FixedProcessViewModel>();
            ContentControlManager.Regiter<FixedProcessView>();

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

            //倒灌电源配置
            SimpleIoc.Default.Register<FunViewModelRPU>();
            ContentControlManager.Regiter<FunViewRPU>();

            //高压直流压源配置
            SimpleIoc.Default.Register<FunViewModelHVDCSource>();
            ContentControlManager.Regiter<FunViewHVDCSource>();

            //弹窗配置
            SimpleIoc.Default.Register<FunViewModelPOPUP>();
            ContentControlManager.Regiter<FunViewPOPUP>();

            //直流负载配置
            SimpleIoc.Default.Register<FunViewModelDCLoad>();
            ContentControlManager.Regiter<FunViewDCLoad>();

            //低压直流负载配置
            SimpleIoc.Default.Register<FunViewModelLVDCLoad>();
            ContentControlManager.Regiter<FunViewLVDCLoad>();

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

            //条件跳转延时配置
            SimpleIoc.Default.Register<FunViewModelReturn>();
            ContentControlManager.Regiter<FunViewReturn>();

            //CCCP模拟信号配置
            SimpleIoc.Default.Register<FunViewModelAnalogAignals>();
            ContentControlManager.Regiter<FunViewAnalogAignals>();


            //信号模拟器配置
            SimpleIoc.Default.Register<FunViewModelSignalSource>();
            ContentControlManager.Regiter<FunViewSignalSource>();

            //原载一体机号配置
            SimpleIoc.Default.Register<FunViewModelACSourceLoad>();
            ContentControlManager.Regiter<FunViewACSourceLoad>();
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
        /// 选择DBC配置
        /// </summary>
        public SelectDBCViewModel SelectDBC=> ServiceLocator.Current.GetInstance<SelectDBCViewModel>();

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
        /// 固定试验步骤配置
        /// </summary>
        public FixedProcessViewModel FixedProcess => ServiceLocator.Current.GetInstance<FixedProcessViewModel>();

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
        /// 倒灌电源配置模板
        /// </summary>
        public FunViewModelRPU FunRPU => ServiceLocator.Current.GetInstance<FunViewModelRPU>();

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
        /// 低压直流负载配置模板
        /// </summary>
        public FunViewModelLVDCLoad FunLVDCLoad => ServiceLocator.Current.GetInstance<FunViewModelLVDCLoad>();

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

        /// <summary>
        /// 条件跳转延时模板
        /// </summary>
        public FunViewModelReturn FunReturn => ServiceLocator.Current.GetInstance<FunViewModelReturn>();

        /// <summary>
        /// CCCP模拟信号模板
        /// </summary>
        public FunViewModelAnalogAignals FunAnalogAignals=> ServiceLocator.Current.GetInstance<FunViewModelAnalogAignals>();

        /// <summary>
        /// 信号模拟器模板
        /// </summary>
        public FunViewModelSignalSource FunSignalSource => ServiceLocator.Current.GetInstance<FunViewModelSignalSource>();

        /// <summary>
        /// 源载一体机模板
        /// </summary>
        public FunViewModelACSourceLoad FunACSourceLoad => ServiceLocator.Current.GetInstance<FunViewModelACSourceLoad>();

        #endregion

    }
}
