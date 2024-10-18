#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ProjectConfig
 * 唯一标识：dbe7e256-5cbf-42cc-bb41-e4d0ca98c5a5
 * 文件名：PCDBCFileChangeViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/7/8 11:10:30
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

using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.DB;
using IMX.DB.Model;
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IMX.ATS.ATEConfig
{
    public class DBCFileChangeViewModel : ExtendViewModelBase
    {


        #region 公共属性

        #region 界面绑定属性
        private ObservableCollection<Test_DBCFileInfo> dbcfileinfos = new ObservableCollection<Test_DBCFileInfo>();
        /// <summary>
        /// DBC文件检索列表
        /// </summary>
        public ObservableCollection<Test_DBCFileInfo> DBCFileInfos
        {
            get => dbcfileinfos;
            set => Set(nameof(DBCFileInfos), ref dbcfileinfos, value);
        }

        private int selecteddbcindex;
        /// <summary>
        /// 当前选择DBC文件Index
        /// </summary>
        public int SelectedDBCIndex
        {
            get => selecteddbcindex;
            set => Set(nameof(SelectedDBCIndex), ref selecteddbcindex, value);
        }

        private string searchstr = string.Empty;
        /// <summary>
        /// 检索字符串
        /// </summary>
        public string SearchStr
        {
            get => searchstr;
            set => Set(nameof(SearchStr), ref searchstr, value);
        }
        #endregion

        #region 界面绑定指令

        public RelayCommand Submit => new RelayCommand(SubmitChange);

        /// <summary>
        /// 项目信息检索指令
        /// </summary>
        public RelayCommand Search => new RelayCommand(() =>
        {
            DBCFileInfos.Clear();
            List<Test_DBCFileInfo> data = new List<Test_DBCFileInfo>();

            if (string.IsNullOrEmpty(SearchStr))
            {
                data = fileinfos;
            }
            else
            {
                data = fileinfos.FindAll(x => x.FileName.Contains(SearchStr));
            }

            for (int i = 0; i < data.Count; i++)
            {
                DBCFileInfos.Add(data[i]);
            }
        });

        #endregion

        /// <summary>
        /// DBC信号配置ID
        /// </summary>
        public int ConfigID { get; set; } = -1;
        #endregion

        #region 公有变量
        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        public bool IsOpen = false;

        #endregion

        #region 私有变量
        private List<Test_DBCFileInfo> fileinfos = new List<Test_DBCFileInfo>();
        #endregion

        #region 私有方法
        private void SubmitChange()
        {
            if (SelectedDBCIndex == -1)
            {
                MessageBox.Show("请先选择DBC文件","变更提交异常");
                return;
            }

            Test_DBCFileInfo info = DBCFileInfos[SelectedDBCIndex];

            string path;

            try
            {
                if (!Directory.Exists(SupportConfig.DBCFileDownPath))
                {
                    Directory.CreateDirectory(SupportConfig.DBCFileDownPath);
                }

                 path = Path.Combine(SupportConfig.DBCFileDownPath, info.FileName+info.FileExtension);

                File.WriteAllBytes(path, info.FileContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "DBC文件变更异常");
                return;
            }

            DBOperate.Default.UpdateDBCFile(ConfigID, info.Id);

            ((ViewModelLocator)Application.Current.FindResource("Locator")).Main.DBCConfig.DBCFileID= info.Id;
            ((ViewModelLocator)Application.Current.FindResource("Locator")).Main.DBCFileInfo.FileName= DBCFileInfos[SelectedDBCIndex].FileName;

            ((ViewModelLocator)Application.Current.FindResource("Locator")).DBCConfig.DBCFileName = DBCFileInfos[SelectedDBCIndex].FileName;
            ((ViewModelLocator)Application.Current.FindResource("Locator")).DBCConfig.LoadFile(path);

            WindowClosedExecute(Win);
        }

        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (!(obj is Window win))
            {
                return;
            }
            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);

            //Window win = new MouseLeftButtonDown().MouseLeft(obj);
            //if (win != null) { Win = win; }

            fileinfos.Clear();
            DBCFileInfos.Clear();
            SearchStr = string.Empty;

            DBOperate.Default.GetFiles().AttachIfSucceed(result => 
            {
                fileinfos = result.Data;
                for (int i = 0; i < result.Data.Count; i++)
                {
                    DBCFileInfos.Add(result.Data[i]);
                }
            });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCFileChangeViewModel() { }
        #endregion

    }
}
