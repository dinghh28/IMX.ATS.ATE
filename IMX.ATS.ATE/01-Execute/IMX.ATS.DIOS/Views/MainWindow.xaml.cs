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

namespace IMX.ATS.DIOS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowModel();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                this.Width = 1250;
                this.Height = 700;
            }
            else if (WindowState == WindowState.Normal)
            {
                //double screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                //double screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;


                this.MaxHeight = SystemParameters.WorkArea.Size.Height;
                this.MaxWidth = SystemParameters.WorkArea.Size.Width;


                //this.MaxWidth = screenWidth * 0.5; // 设置为屏幕宽度的80%
                //this.MaxHeight = screenHeight * 0.48; // 设置为屏幕高度的80%
                //this.MaxWidth = screenWidth; // 设置为屏幕宽度的80%
                //this.MaxHeight = screenHeight; // 设置为屏幕高度的80%


                SystemCommands.MaximizeWindow(this);
            }
        }
    }
}
