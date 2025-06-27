#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ProjectConfig.ViewModel
 * 唯一标识：61e30a83-ba14-4617-be77-80e7a6e3ca9c
 * 文件名：PCProjectInfoViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/7/17 15:46:26
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using FastDeepCloner;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.DB;
using IMX.DB.Model;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig
{
    /// <summary>
    /// 项目信息配置界面
    /// </summary>
    public class ProjectInfoViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性
        private bool enableedite;
        /// <summary>
        /// 项目名称/SN允许编辑状态
        /// </summary>
        public bool EnableEdite
        {
            get => enableedite;
            set => Set(nameof(EnableEdite), ref enableedite, value);
        }

        //public List<ACInputVol> InputVol => new List<ACInputVol> { ACInputVol.VOL_110, ACInputVol.VOL_220 };

        //private ACInputVol selectedvol;
        ///// <summary>
        ///// 当前选择输入电压
        ///// </summary>
        //public ACInputVol SelectedVol
        //{
        //    get => selectedvol;
        //    set => Set(nameof(SelectedVol), ref selectedvol, value);
        //}

        private bool usedbc;
        /// <summary>
        /// DBC通讯启用
        /// </summary>
        public bool UseDBC
        {
            get => usedbc;
            set => Set(nameof(UseDBC), ref usedbc, value);
        }

        //private bool enableprotect;
        ///// <summary>
        ///// 试验保护使能
        ///// </summary>
        //public bool EnableProtect
        //{
        //    get => enableprotect;
        //    set => Set(nameof(EnableProtect), ref enableprotect, value);
        //}

        private string projectname;
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get => projectname;
            set => Set(nameof(ProjectName), ref projectname, value);
        }


        private string projectsn;
        /// <summary>
        /// 项目SN
        /// </summary>
        public string ProjectSN
        {
            get => projectsn;
            set => Set(nameof(ProjectSN), ref projectsn, value);
        }

        private Electricity electricity = Electricity.Single;
        /// <summary>
        /// 电能类型
        /// </summary>
        public Electricity Electricity
        {
            get => electricity;
            set
            {
                if (Set(nameof(Electricity), ref electricity, value))
                {
                    GlobalModel.NowElectricity = value;
                }
            }
        }

        private string dbcconfigname;
        /// <summary>
        /// dbc配置名称
        /// </summary>
        public string DBCConfigName
        {
            get => dbcconfigname;
            set => Set(nameof(DBCConfigName), ref dbcconfigname, value);
        }


        //private uint ratedvol = 220;
        ///// <summary>
        ///// 标定电压
        ///// </summary>
        //public uint RatedVol
        //{
        //    get => ratedvol;
        //    set => Set(nameof(RatedVol), ref ratedvol, value);
        //}


        //private uint ratedcur = 20;
        ///// <summary>
        ///// 标定电流
        ///// </summary>
        //public uint RatedCur
        //{
        //    get => ratedcur;
        //    set => Set(nameof(RatedCur), ref ratedcur, value);
        //}

        //private uint ratedpow = 4400;
        ///// <summary>
        ///// 标定电压
        ///// </summary>
        //public uint RatedPow
        //{
        //    get => ratedpow;
        //    set => Set(nameof(RatedPow), ref ratedpow, value);
        //}

        //private uint runtime = 1;
        ///// <summary>
        ///// 老化总时长
        ///// </summary>
        //public uint RunTime
        //{
        //    get => runtime;
        //    set
        //    {
        //        if (value > 0)
        //        { Set(nameof(RunTime), ref runtime, value); }
        //        else
        //        {
        //            MessageBox.Show($"老化总时长不能少于1小时！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        /// <summary>
        /// CAN支持波特率列表
        /// </summary>
        public List<string> BaudRates => new List<string> { "5Kbps", "10Kbps", "20Kbps", "50Kbps", "100Kbps", "125Kbps", "250Kbps", "500Kbps", "800Kbps", "1000Kbps" };

        private string baudrate;
        /// <summary>
        /// 仲裁波特率
        /// </summary>
        public string BaudRate
        {
            get => baudrate;
            set => Set(nameof(BaudRate), ref baudrate, value);
        }


        private string databaudrate;
        /// <summary>
        /// 数据域波特率
        /// </summary>
        public string DataBaudRate
        {
            get => databaudrate;
            set => Set(nameof(DataBaudRate), ref databaudrate, value);
        }

        /// <summary>
        /// 电能类型编辑使能
        /// </summary>
        public bool EnableEditElectricity => GlobalModel.IsNewProject;

        #endregion

        #region 界面绑定指令
        public RelayCommand SaveConfig => new RelayCommand(SavedConfig);

        public RelayCommand ChangeDBC => new RelayCommand(() =>
        {
            var model = ((ViewModelLocator)Application.Current.FindResource("Locator")).SelectDBC;
            Window view = ContentControlManager.GetWindow<SelectDBCView>(model);
            if (model.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            
            if (GlobalModel.TestDBCconfig != null || GlobalModel.TestDBCconfig.Id != 0)
            {
                if (MessageBox.Show("是否切换当前配置DBC?\r\n若切换请核对试验项操作步骤，避免产品相关指令不符合预期", "DBC配置变更", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            view.Topmost = true;
            view.Show();

            Task.Run(() => 
            {
                Thread.Sleep(500);
                while (true)
                {
                    if (!model.IsOpen)
                    {
                        dbcconfig = GlobalModel.TestDBCconfig_Change;
                        if (dbcconfig != null)
                        {
                            DBCConfigName = dbcconfig.ConfigName;
                        }
                    }
                }
            });
        });

        #endregion

        /// <summary>
        /// 项目信息
        /// </summary>
        public Test_ProjectInfo ProjectInfo { get; set; } = null;
        #endregion

        #region 公有变量
        /// <summary>
        /// 是否为新建项目
        /// </summary>
        public bool IsNewProject { get; set; } = false;
        #endregion

        #region 私有变量
        private bool isnew;

        private MainViewModel mainviewmodel;

        private Test_DBCConfig dbcconfig = null;
        #endregion

        #region 私有方法

        private void SavedConfig()
        {
            try
            {
                if (string.IsNullOrEmpty(ProjectName)) { MessageBox.Show($"项目名称不允许为空！", "项目保存", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (string.IsNullOrEmpty(ProjectSN)) { MessageBox.Show($"项目SN不允许为空！", "项目保存", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (string.IsNullOrEmpty(DBCConfigName)) {MessageBox.Show($"请选择DBC配置后再进行保存！", "项目保存", MessageBoxButton.OK, MessageBoxImage.Error); return;}

                //ProjectInfo.InputVol = SelectedVol;
                //ProjectInfo.RunTime = RunTime;

                //ProjectInfo.IsUseDDBC = UseDBC;
                //ProjectInfo.RatedCur = RatedCur;
                //ProjectInfo.RatedVol = RatedVol;
                //ProjectInfo.RatedPow = RatedPow;
                ProjectInfo.BaudRate = BaudRate;
                ProjectInfo.DataBaudrate = DataBaudRate;
                ProjectInfo.Electricity = Electricity;
                ProjectInfo.DBCConfigName = DBCConfigName;

                if (dbcconfig != null)
                {
                    ProjectInfo.DBCConfigID = GlobalModel.TestDBCconfig_Change.Id;
                }

                if (isnew && GlobalModel.Test_ProjectInfo.Id == 0) 
                {
                    ProjectInfo.ProjectName = ProjectName;
                    ProjectInfo.ProjectSN = ProjectSN;

                    DBOperate.Default.InsertProjectInfo(ProjectInfo).AttachIfSucceed(result => 
                    {
                        mainviewmodel.ProjectName = ProjectName;
                        mainviewmodel.FlowConfigVisbility = Visibility.Visible;
                        mainviewmodel.DBCConfigVisbility = Visibility.Visible;
                        dbcconfig = null;
                        MessageBox.Show("项目信息保存成功!", "新建项目", MessageBoxButton.OK, MessageBoxImage.Information);
                    }).AttachIfFailed(result=> MessageBox.Show($"项目信息保存失败：\r\n{result.Message}!", "新建项目"));
                }
                else
                {
                    DBOperate.Default.UpdataProjectInfo(ProjectInfo).AttachIfSucceed(result => 
                    {
                        dbcconfig = null;
                        mainviewmodel.FlowConfigVisbility = Visibility.Visible;
                        mainviewmodel.DBCConfigVisbility = Visibility.Visible;
                        MessageBox.Show("项目信息保存成功!", "项目更新", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }

                //ProjectInfo.EnableProtect = EnableProtect;
                //if (isnew)
                //{
                //    ProjectInfo.ProjectName = ProjectName;
                //    ProjectInfo.ProjectSN = ProjectSN;
                //    DBOperate.Default.InsertProjectInfo(ProjectInfo).AttachIfSucceed(result =>
                //    {
                //        mainviewmodel.DBCConfigVisbility = ProjectInfo.IsUseDDBC ? Visibility.Visible : Visibility.Collapsed;
                //        //if (ProjectInfo.IsUseDDBC && GlobalModel.TestDBCconfig.Id == 0)
                //        //{
                //        //    DBOperate.Default.InsertDBCConfig(GlobalModel.TestDBCconfig);
                //        //}
                //        //mainviewmodel.ProtectConfigVisbility = ProjectInfo.EnableProtect ? Visibility.Visible : Visibility.Collapsed;
                //        mainviewmodel.FlowConfigVisbility = Visibility.Visible;
                //        isnew = false;
                //        mainviewmodel.ProjectName = ProjectName;
                //        //mainviewmodel.ProjectInfo.Id = ProjectInfo.Id;
                //        EnableEdite = true;
                //        MessageBox.Show("项目信息保存成功!", "[新建]", MessageBoxButton.OK, MessageBoxImage.Information);
                //    })
                //        .AttachIfFailed(result =>
                //        {
                //            MessageBox.Show($"项目信息保存失败：\r\n{result.Message}!", "[新建]");
                //            return;
                //        }).
                //        AttachIfExcepted(result =>
                //        {
                //            MessageBox.Show($"项目信息保存异常：\r\n{result.Message}!", "[新建]");
                //            return;
                //        });
                //}
                //else
                //{
                //    DBOperate.Default.UpdataProjectInfo(ProjectInfo).AttachIfSucceed(result =>
                //    {
                //        mainviewmodel.DBCConfigVisbility = ProjectInfo.IsUseDDBC ? Visibility.Visible : Visibility.Collapsed;
                //        //mainviewmodel.ProtectConfigVisbility = ProjectInfo.EnableProtect ? Visibility.Visible : Visibility.Collapsed;
                //        mainviewmodel.FlowConfigVisbility = Visibility.Visible;
                //        if (ProjectInfo.IsUseDDBC && GlobalModel.TestDBCconfig.Id == 0)
                //        {
                //            DBOperate.Default.InsertDBCConfig(GlobalModel.TestDBCconfig);
                //        }

                //        MessageBox.Show("项目信息更新成功!", "[更新]", MessageBoxButton.OK, MessageBoxImage.None);
                //    })
                //        .AttachIfFailed(result => { MessageBox.Show($"项目信息更新失败：\r\n{result.Message}!", "[更新]"); })
                //        .AttachIfExcepted(result => { MessageBox.Show($"项目信息更新异常：\r\n{result.Message}!", "[更新]"); });
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"项目信息更新异常:\r\n{ex.Message}!", "项目保存异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           

        }

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            mainviewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
            
            isnew = GlobalModel.IsNewProject;
            //isnew = mainviewmodel.IsNewProject;
            EnableEdite = !isnew;
            if (!isnew)
            {
                ProjectName = GlobalModel.Test_ProjectInfo.ProjectName;
                ProjectSN = GlobalModel.Test_ProjectInfo.ProjectSN;
                //UseDBC = GlobalModel.Test_ProjectInfo.IsUseDDBC;
                //RatedPow = GlobalModel.Test_ProjectInfo.RatedPow;
                BaudRate = GlobalModel.Test_ProjectInfo.BaudRate;
                DataBaudRate= GlobalModel.Test_ProjectInfo.DataBaudrate ;
                DBCConfigName = ProjectInfo.DBCConfigName;
                Electricity = ProjectInfo.Electricity;

            }
            //((ViewModelLocator)Application.Current.FindResource("Locator")).ProjectSelect.WindowClose.Execute("ProjectSelect");
        }

        protected override void WindowClosedExecute(object obj)
        {
            ProjectName = "";
            ProjectSN = "";
            //RunTime = 1;
            //UseDBC = false;
            //SelectedVol = 0;
            //EnableProtect = false;
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public ProjectInfoViewModel()
        {
            ProjectInfo = GlobalModel.Test_ProjectInfo;
        }
        #endregion

    }
}
