using H.Maths.Encryption.AES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IMX.ATS.UserManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string[] args) 
        {
            InitializeComponent();

            //string arg = AES.Decrypt(args[0], "VXNlck1hbmFnZQ==");
            //var info = JsonConvert.DeserializeObject<UserInfo>(arg);
            //GlobalModel.UserInfo = info;
            //if (args.Length>0)
            //{
            //    MessageBox.Show(args[0]);
            //}
        }
    }
}
