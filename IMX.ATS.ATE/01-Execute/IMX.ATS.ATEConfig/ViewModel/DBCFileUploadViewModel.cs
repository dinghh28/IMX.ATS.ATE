#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.ProjectConfig
 * 唯一标识：fb15d7d0-2556-40f0-a3e7-b5147a6ab2e0
 * 文件名：PCDBCFileUploadViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2024/7/8 11:06:59
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
using IMX.WPF.Resource;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.ATEConfig
{
    public class DBCFileUploadViewModel : ExtendViewModelBase
    {


        #region 公共属性


        #region 界面绑定属性
        private string dbcfilepath = "";
        /// <summary>
        /// 文件地址
        /// </summary>
        public string DBCFilePath
        {
            get => dbcfilepath;
            set => Set(nameof(DBCFilePath), ref dbcfilepath, value);
        }

        private string dbcfilename = "";
        /// <summary>
        /// 文件名称
        /// </summary>
        public string DBCFileName
        {
            get => dbcfilename;
            set => Set(nameof(DBCFileName), ref dbcfilename, value);
        }

        private string dbcfiledescription = "";
        /// <summary>
        /// 文件说明
        /// </summary>
        public string DBCFileDescription
        {
            get => dbcfiledescription;
            set => Set(nameof(DBCFileDescription), ref dbcfiledescription, value);
        }
        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 文件选择指令
        /// </summary>
        public RelayCommand SelectFile => new RelayCommand(SelectedFile);

        /// <summary>
        /// 文件上传指令
        /// </summary>
        public RelayCommand UploadFile => new RelayCommand(UploadedFile);

        //public RelayCommand<DragEventArgs> DropFile => new RelayCommand<DragEventArgs>(ExecuteDrop);

        #endregion

        #endregion

        #region 公有变量

        public bool IsOpen = false;

        #endregion

        #region 私有变量
        /// <summary>
        /// 当前窗口
        /// </summary>
        private Window Win = null;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        private string dbcfileextension = ".dbc";
        #endregion


        #region 私有方法
        /// <summary>
        /// 选择DBC文件
        /// </summary>
        private void SelectedFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "请选择消息文件",
                Filter = "DBC文件|*.dbc",
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            DBCFilePath = openFileDialog.FileName;
            DBCFileName = Path.GetFileNameWithoutExtension(DBCFilePath);
            dbcfileextension = Path.GetExtension(DBCFilePath);
            //DBCFileName = openFileDialog.SafeFileName;
        }

        /// <summary>
        /// 上传DBC文件
        /// </summary>
        private void UploadedFile()
        {
            if (string.IsNullOrEmpty(DBCFilePath) || string.IsNullOrEmpty(DBCFileName))
            {
                MessageBox.Show("请选择上传文件", "文件上传异常");
                return;
            }

            try
            {
                // 读取txt文件内容
                Byte[] fileContent = File.ReadAllBytes(DBCFilePath);

                //GlobalModel.TestInfo.Test_DBCFile.Test_FileContent = fileContent;
                //GlobalModel.TestInfo.Test_DBCFile.Test_FileSize = fileContent.Length;
                //GlobalModel.TestInfo.Test_DBCFile.Test_FileName = DBCFileName;

                DBOperate.Default.InsertDBCFile(new DB.Model.Test_DBCFileInfo
                {
                    FileName = DBCFileName,
                    FileContent = fileContent,
                    FileSize = fileContent.Length,
                    FileExtension = dbcfileextension,
                    FileDescription = DBCFileDescription,
                    Operator = GlobalModel.UserInfo.UserName,
                }).AttachIfSucceed(result=>
                { MessageBox.Show($"文件上传成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); })
                .AttachIfFailed(result=> { MessageBox.Show($"文件上传：{result.Message}！", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error); })
                 .AttachIfFailed(result => { MessageBox.Show($"文件上传：{result.Message}！", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error); });

                if (MessageBox.Show("是否关闭当前窗口", "DBC文件变更窗口关闭", MessageBoxButtons.YesNo) !=  DialogResult.Yes) return;

                WindowClosedExecute(Win);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetMessage(), "文件上传异常");
            }
        }

        ///// <summary>
        ///// 文件拖拽事件
        ///// </summary>
        ///// <param name="args"></param>
        ///// <exception cref="NotImplementedException"></exception>
        //private void ExecuteDrop(DragEventArgs args)
        //{
        //   var x =  args.Data.GetData(System.Windows.DataFormats.FileDrop);
        //}

        private void ExecuteDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                return;
            }

            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            string extension = Path.GetExtension(files[0]);

            if (extension.ToUpper() != ".DBC")
            {
                MessageBox.Show("当前仅接收 .dbc 后缀文件，请确认文件格式", "文件异常");
                return;
            }

            DBCFilePath = files[0];
            DBCFileName = Path.GetFileNameWithoutExtension(DBCFilePath);
            dbcfileextension = extension;
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            DBCFilePath = "";
            DBCFileName = "";
            DBCFileDescription = "";

            dbcfileextension = string.Empty;

            if (!(obj is Window win))
            {
                return;
            }

            WindowLeftDown_MoveEvent.LeftDown_MoveEventRegister(win);
            if (win != null) { Win = win; Win.Drop += ExecuteDrop; }
            IsOpen = true;
            
            //var window = (obj as Window);
            //if (window != null)
            //{
            //    window.Drop += ExecuteDrop;
            //}

        }

        protected override void WindowClosedExecute(object obj)
        {
            #region 保证再次开启MinLines属性生效
            DBCFilePath = "";
            DBCFileName = "";
            DBCFileDescription = "";
            #endregion

            IsOpen = false;

            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public DBCFileUploadViewModel() { }
        #endregion

    }
}
