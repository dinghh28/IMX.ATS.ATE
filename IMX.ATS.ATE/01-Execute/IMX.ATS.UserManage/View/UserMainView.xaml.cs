﻿using System;
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
using System.Windows.Shapes;

namespace IMX.ATS.UserManage
{
    /// <summary>
    /// UserMainView.xaml 的交互逻辑
    /// </summary>
    public partial class UserMainView : Window
    {
        public UserMainView()
        {
            InitializeComponent();
        }

        public UserMainView(string[] args)
        {
            InitializeComponent();
            //if (args.Length > 0)
            //{
            //    MessageBox.Show(args[0]);
            //}
            //this.DataContext = ((ViewModelLocator)System.Windows.Application.Current.FindResource("Locator")).User;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}