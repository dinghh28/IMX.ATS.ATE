#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DBCConfig.ViewModel
 * 唯一标识：92abc2b0-cb09-4ffa-9160-5ee16375eb0e
 * 文件名：DBCConfigViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/3/11 16:25:56
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

using Force.DeepCloner;
using GalaSoft.MvvmLight.CommandWpf;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.DB;
using IMX.DB.Model;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.DBCConfig
{
    public class DBCConfigViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

        private string dbcfilename;
        /// <summary>
        /// DBC文件名称
        /// </summary>
        public string DBCFileName
        {
            get => dbcfilename;
            set => Set(nameof(DBCFileName), ref dbcfilename, value);
        }

        private string dbcconfigname;
        /// <summary>
        /// DBC配置名称
        /// </summary>
        public string DBCConfigName
        {
            get => dbcconfigname;
            set => Set(nameof(DBCConfigName), ref dbcconfigname, value);
        }

        private string dbcconfigdescription;
        /// <summary>
        /// DBC配置说明
        /// </summary>
        public string DBCConfigDescription
        {
            get => dbcconfigdescription;
            set => Set(nameof(DBCConfigDescription), ref dbcconfigdescription, value);
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
                    enableusereceive = value == GlobalModel.Test_DBC.Electricity;
                }
            }
        }


        /// <summary>
        /// 配置名称编辑使能
        /// </summary>
        public bool EnableEditName => !GlobalModel.IsNew;
        #endregion

        #region 界面绑定指令

        /// <summary>
        /// DBC文件变更
        /// </summary>
        public RelayCommand ChangeDBCFile => new(ChangedDBCFile);

        public RelayCommand Save => new(SaveConfig);
        #endregion

        #endregion

        #region 私有变量
        private Test_DBCFileInfo fileInfo = null;

        private MainViewModel mainviewmodel;

        /// <summary>
        /// DBC配置ID
        /// </summary>
        private int dbcid = -1;

        private bool enableuse = false;
        private bool enableusesend = false;
        private bool enableusereceive = false;
        #endregion

        #region 私有方法
        /// <summary>
        /// DBC文件变更
        /// </summary>
        private void ChangedDBCFile()
        {
            try
            {
                var viewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).DBCFileChange;
                Window mainwindow = ContentControlManager.GetWindow<DBCFileChangeView>(viewmodel);
                if (viewmodel.IsOpen) { MessageBox.Show($"界面已打，请勿重复操作！", "界面提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }

                var result = MessageBox.Show("是否切换当前配置DBC文件，若切换将自动清空当前配置信号", "DBC文件变更", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No) { return; }

                mainwindow.Topmost =  true;
                //Window mainwindow = ContentControlManager.GetWindow<DBCFileChangeView>(viewmodel);
                mainwindow.Show();

                

                Task.Run(() => 
                {
                    
                    Thread.Sleep(500);
                    while (true)
                    {
                        if (!viewmodel.IsOpen)
                        {
                            fileInfo = GlobalModel.TestDBCFileInfo_Change;

                            if (fileInfo != null)
                            {
                                DBCFileName = fileInfo.FileName;
                                enableuse = false;
                            }
                            return;
                        }
                        Thread.Sleep(100);
                    }
                });


                ////清空上报配置
                //SignalConfigPages[0].SignalConfigs.Clear();
                ////清空下发配置
                //SignalConfigPages[1].SignalConfigs.Clear();
                ////清空信号列表
                //DBCMessages.Clear();

                ////重新导入DBC文件信号
                //IsloadFile = true;
                //重新加载DBC配置信号
                //AddFixedSignal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"变更DBC文件异常：{ex.Message}", "异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 配置保存
        /// </summary>
        private void SaveConfig()
        {
            if (string.IsNullOrEmpty(DBCConfigName))
            {
                MessageBox.Show("DBC配置名称不可为空!请填写后再保存");
                return;
            }

            if (string.IsNullOrEmpty(DBCFileName))
            {
                MessageBox.Show("请先选择配置所需DBC文件后，再进行保存!");
                return;
            }


            if (fileInfo == null &&(GlobalModel.Test_DBCFileInfo == null || GlobalModel.Test_DBCFileInfo.IsDeleted))
            {
                MessageBox.Show("当前DBC文件已删除，请重新选择文件后再进行保存");
                return;
            }

            #region 新配置插入
            if (GlobalModel.IsNew && GlobalModel.Test_DBC.Id == 0)
            {
                GlobalModel.Test_DBC.Describe = DBCConfigDescription;
                GlobalModel.Test_DBC.DBCFileName = DBCFileName;
                GlobalModel.Test_DBC.ConfigName = DBCConfigName;
                GlobalModel.Test_DBC.DBCFileID = fileInfo.Id;
                GlobalModel.Test_DBC.Electricity = Electricity;
                GlobalModel.Test_DBC.EnableUse = true;
                DBOperate.Default.InsertDBCConfig(GlobalModel.Test_DBC).AttachIfSucceed(result =>
                {
                    MessageBox.Show($"DBC信息保存成功！");

                    GlobalModel.Test_DBCFileInfo = fileInfo.DeepClone();
                    fileInfo = null;
                    GlobalModel.TestDBCFileInfo_Change = null;
                    dbcid = GlobalModel.Test_DBC.Id;

                    mainviewmodel.MessageConfigVisbility = Visibility.Visible;
                }).AttachIfFailed(result => MessageBox.Show(result.Message, "DBC配置信息保存失败"));
                return;
            }
            #endregion

            #region 涉及文件保存变更

            #region 无效文件变更
            if (GlobalModel.Test_DBCFileInfo == null)
            {
                DBOperate.Default.UpdateDBCConfig(GlobalModel.Test_DBC.Id, DBCConfigName, DBCConfigDescription, fileInfo.FileName, fileInfo.Id, Electricity, false, false)
                    .AttachIfSucceed(result =>
                    {
                        GlobalModel.Test_DBC.Describe = DBCConfigDescription;
                        GlobalModel.Test_DBC.DBCFileName = DBCFileName;
                        GlobalModel.Test_DBC.ConfigName = DBCConfigName;
                        GlobalModel.Test_DBC.DBCFileID = fileInfo.Id;
                        GlobalModel.Test_DBC.Electricity = Electricity;
                        GlobalModel.Test_DBC.EnableUseReceive = false;
                        GlobalModel.Test_DBC.EnableUseSend = false;
                        GlobalModel.Test_DBC.EnableUse = true;
                        GlobalModel.Test_DBCFileInfo = fileInfo.DeepClone();
                        //fileInfo = null;
                        GlobalModel.TestDBCFileInfo_Change = null;
                        mainviewmodel.MessageConfigVisbility = Visibility.Visible;
                        MessageBox.Show($"DBC信息保存成功！");
                    });
                return;
            }
            #endregion

      
            if (fileInfo.Id != GlobalModel.Test_DBCFileInfo.Id)
            {
                if (MessageBox.Show("当前项目DBC文件发生变更，是否保存（保存后会清空当前上报和下发信号配置）", "DBC文件变更提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }

                DBOperate.Default.UpdateDBCConfig(GlobalModel.Test_DBC.Id, DBCConfigName, DBCConfigDescription, fileInfo.FileName, fileInfo.Id, Electricity, false, false)
                                    .AttachIfSucceed(result =>
                                    {
                                        GlobalModel.Test_DBC.Describe = DBCConfigDescription;
                                        GlobalModel.Test_DBC.DBCFileName = DBCFileName;
                                        GlobalModel.Test_DBC.ConfigName = DBCConfigName;
                                        GlobalModel.Test_DBC.DBCFileID = fileInfo.Id;
                                        GlobalModel.Test_DBC.Electricity = Electricity;
                                        GlobalModel.Test_DBC.EnableUse = true;
                                        GlobalModel.Test_DBC.EnableUseReceive = false;
                                        GlobalModel.Test_DBC.EnableUseSend = false;
                                        GlobalModel.Test_DBCFileInfo = fileInfo.DeepClone();
                                        //fileInfo = null;
                                        GlobalModel.TestDBCFileInfo_Change = null;
                                        mainviewmodel.MessageConfigVisbility = Visibility.Visible;
                                        MessageBox.Show($"DBC信息保存成功！");
                                    });
                return;
            }
            #endregion


            DBOperate.Default.UpdateDBCConfig(GlobalModel.Test_DBC.Id, DBCConfigName, DBCConfigDescription, Electricity, enableusereceive)
                .AttachIfSucceed(result=> 
                {
                    GlobalModel.Test_DBC.Describe = DBCConfigDescription;
                    GlobalModel.Test_DBC.ConfigName = DBCConfigName;
                    GlobalModel.Test_DBC.Electricity = Electricity;
                    GlobalModel.Test_DBC.EnableUseReceive = enableusereceive;
                    MessageBox.Show($"DBC信息保存成功！");
                    mainviewmodel.MessageConfigVisbility = Visibility.Visible;
                });
        }

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (dbcid == GlobalModel.Test_DBC.Id)
            {
                return;
            }

            dbcid = GlobalModel.Test_DBC.Id;

            fileInfo = GlobalModel.Test_DBCFileInfo;

            if (!GlobalModel.IsNew)
            {
                if (GlobalModel.Test_DBCFileInfo!=null)
                {
                    DBCFileName = GlobalModel.Test_DBC.DBCFileName;
                }
                
                DBCConfigDescription = GlobalModel.Test_DBC.Describe;
                Electricity = GlobalModel.Test_DBC.Electricity;
                DBCConfigName = GlobalModel.Test_DBC.ConfigName;
            }
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCConfigViewModel() 
        {
            mainviewmodel = ((ViewModelLocator)Application.Current.FindResource("Locator")).Main;
        }
        #endregion

    }
}
