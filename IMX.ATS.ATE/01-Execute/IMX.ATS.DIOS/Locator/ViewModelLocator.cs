using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using H.WPF.Framework;

namespace IMX.ATS.DIOS
{
    public class ViewModelLocator: BaseViewModelLocator
    {
        public ViewModelLocator() 
        {
            //数据查询主窗口
            SimpleIoc.Default.Register<MainWindowModel>();
            ContentControlManager.Regiter<MainWindow>();

            //数据查询主窗口
            SimpleIoc.Default.Register<MainViewModel>();
            ContentControlManager.Regiter<MainView>();

            //试验项目信息条目查询界面
            SimpleIoc.Default.Register<TestProjectItemViewModel>();
            ContentControlManager.Regiter<TestProjectItemView>();

            //试验条目查询界面
            SimpleIoc.Default.Register<TestItemViewModel>();
            ContentControlManager.Regiter<TestItemView>();
        }

        /// <summary>
        /// 数据查询主窗口
        /// </summary>
        public MainWindowModel Main => ServiceLocator.Current.GetInstance<MainWindowModel>();

        /// <summary>
        /// 数据查询主窗口
        /// </summary>
        public MainViewModel Home => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// 试验项目信息条目查询
        /// </summary>
        public TestProjectItemViewModel Project=> ServiceLocator.Current.GetInstance<TestProjectItemViewModel>();

        /// <summary>
        /// 试验条目查询
        /// </summary>
        public TestItemViewModel TestItem=> ServiceLocator.Current.GetInstance<TestItemViewModel>();
    }
}
