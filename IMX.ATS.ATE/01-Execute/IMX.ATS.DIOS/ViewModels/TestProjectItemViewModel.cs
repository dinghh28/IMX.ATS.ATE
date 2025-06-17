#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DIOS.ViewModels
 * 唯一标识：424ede69-1d27-46f0-82ef-3efc29e0ab1a
 * 文件名：TestProjectItemViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/12 15:24:25
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
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMX.ATS.DIOS.ViewModels
{
    public class TestProjectItemViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性
        #region 查询条件
        private DateTime testStartTime = DateTime.Now;
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime TestStartTime
        {
            get => testStartTime;
            set => Set(nameof(TestStartTime), ref testStartTime, value);
        }

        private DateTime testEndTime = DateTime.Now;
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime TestEndTime
        {
            get => testEndTime;
            set => Set(nameof(TestEndTime), ref testEndTime, value);
        }

        /// <summary>
        /// 样品编号
        /// </summary>
        private string product_SN;

        public string Product_SN
        {
            get => product_SN;
            set => Set(nameof(Product_SN), ref product_SN, value);
        }


        private ObservableCollection<string> product_Tests = new ObservableCollection<string>();
        /// <summary>
        /// 项目编号列表
        /// </summary>
        public ObservableCollection<string> Product_Tests
        {
            get => product_Tests;
            set
            {
                if (Set(nameof(Product_Tests), ref product_Tests, value))
                {
                }
            }
        }


        private int product_TestSNIndex = -1;
        /// <summary>
        /// 当前选择项目编号序号
        /// </summary>
        public int Product_TestSNIndex
        {
            get => product_TestSNIndex;
            set => Set(nameof(Product_TestSNIndex), ref product_TestSNIndex, value);
        }
        #endregion

        private ObservableCollection<Test_ProjectItemInfo> datas = new ObservableCollection<Test_ProjectItemInfo>();
        /// <summary>
        /// 试验项目条目数据
        /// </summary>
        public ObservableCollection<Test_ProjectItemInfo> Datas
        {
            get => datas;
            set => Set(nameof(Datas), ref datas, value);
        }

        private int selectindex;
        /// <summary>
        /// 当前选择行
        /// </summary>
        public int SelectIndex
        {
            get => selectindex;
            set => Set(nameof(SelectIndex), ref selectindex, value);
        }


        #region 翻页

        private long datacount;
        /// <summary>
        /// 查询数据总数
        /// </summary>
        public long DataCount
        {
            get => datacount;
            set => Set(nameof(DataCount), ref datacount, value);
        }


        private double itemcount = 0;
        /// <summary>
        /// 测试数据行数
        /// </summary>
        public double ItemCount
        {
            get => itemcount;
            set => Set(nameof(ItemCount), ref itemcount, value);
        }

        private long selectpage = 1;

        /// <summary>
        /// 测试数据当前页
        /// </summary>
        public long SelectPage
        {
            get => selectpage;
            set => Set(nameof(SelectPage), ref selectpage, value);
        }

        private long pagecount;

        /// <summary>
        /// 测试数据总页数
        /// </summary>
        public long PageCount
        {
            get => pagecount;
            set => Set(nameof(PageCount), ref pagecount, value);
        }
        #endregion
        #endregion

        #region 界面绑定指令
        /// <summary>
        /// 查找指令
        /// </summary>
        public RelayCommand Search => new RelayCommand(SearchInfo);

        public RelayCommand Return => new RelayCommand(ReturnToItem);



        /// <summary>
        /// 翻页指令
        /// </summary>
        public RelayCommand<object> SelectNextDataCommand => new RelayCommand<object>(GetTestDataLimit);
        #endregion

        #endregion

        #region 私有变量
        /// <summary>
        /// 项目信息字典[项目名称，项目信息]
        /// </summary>
        Dictionary<string, Test_ProjectInfo> dicProjectInfo = new Dictionary<string, Test_ProjectInfo>();

        /// <summary>
        /// 单页数据数量
        /// </summary>
        private readonly int OnePageCount = 30;
        # region 历史检索条件(防止翻页期间更改查询条件)
        private bool isnotesttext = false;

        private bool isnotestsn = false;

        #region 历史检索条件存储
        private DateTime start;

        private DateTime end;

        private string lastprojectsn = string.Empty;

        private int lastprodectid = -1;
        #endregion
        #endregion

        #endregion

        #region 私有方法

        #region 数据初始化
        private void GetAllProject()
        {
            OperateResult<List<Test_ProjectInfo>> Testidresult = DBOperate.Default.SelectedProjectInfo_All();
            if (Testidresult)
            {
                if (Testidresult.Data.Count > 0)
                {
                    dicProjectInfo.Clear();

                    Product_Tests?.Clear();
                    Testidresult.Data.ForEach(x =>
                    {
                        Product_Tests.Add(x.ProjectSN);
                        dicProjectInfo.Add(x.ProjectSN, x);
                    });
                }
            }
        }
        #endregion

        private void SearchInfo()
        {
            isnotesttext = Product_TestSNIndex == -1;
            isnotestsn = string.IsNullOrEmpty(Product_SN);
            if (isnotesttext && isnotestsn)
            {
                MessageBox.Show($"请选择需要项目编号或填写产品编号！", "非法查询");
                return;
            }

            OperateResult<List<Test_ProjectItemInfo>> Testidresult;
            OperateResult<long> Countresult;

            start = TestStartTime.Date;
            end = TestEndTime.AddDays(1).Date;
            lastprojectsn = Product_Tests[Product_TestSNIndex];
            
            if (!isnotesttext && !isnotestsn)
            {
                lastprodectid = dicProjectInfo[lastprojectsn].Id;
                Countresult = DBOperate.Default.SelectTestProjectItem_Count(lastprodectid, Product_SN, start, end);
                Testidresult = DBOperate.Default.SelectTestProjectItem(lastprodectid, Product_SN, start, end, 1, OnePageCount);// : DBOperate.Default.GetTestItemByIDandTime(Product_SN, TestStartTime, TestEndTime);
            }
            else if (!isnotestsn)
            {
                Countresult = DBOperate.Default.SelectTestProjectItem_Count(Product_SN, start, end);
                Testidresult = DBOperate.Default.SelectTestProjectItem(Product_SN, start, end, 1, OnePageCount);
            }
            else
            {
                lastprodectid = dicProjectInfo[lastprojectsn].Id;
                Countresult = DBOperate.Default.SelectTestProjectItem_Count(lastprodectid,start, end);
                Testidresult = DBOperate.Default.SelectTestProjectItem(lastprodectid, start, end, 1, OnePageCount);
            }

            if (!Countresult)
            {
                MessageBox.Show(Countresult.Message, "试验项目条目数据查找失败");
                return;
            }
            if (!Testidresult)
            {
                System.Windows.Forms.MessageBox.Show($"数据库查询项目信息异常：{Testidresult.Message}", "试验项目条目数据查找失败");
                //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestITemData), nameof(DBOperate.Default.GetTetsItemByTestID), Testidresult.Message); 
                return;
            }

            DataCount = Countresult.Data;
            PageCount = ((DataCount % OnePageCount) == 0) ? (DataCount / OnePageCount) : ((DataCount / OnePageCount) + 1);

            Datas.Clear();
            for (int i = 0; i < Testidresult.Data.Count; i++)
            {
                Datas.Add(Testidresult.Data[i]);
            }
        }

        private void GetTestDataLimit(object pagename) 
        {
            if ((selectpage == 1) && pagename.ToString() == "上一页")
            {
                MessageBox.Show($"当前为数据最前一页，无法继续查询！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            if ((SelectPage == PageCount) && pagename.ToString() == "下一页")
            {
                MessageBox.Show($"当前为数据最后一页，无法继续查询！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            long pagenum = SelectPage;
            switch (pagename.ToString())
            {
                case "上一页":
                    pagenum--;
                    break;
                case "下一页":
                    pagenum++;
                    break;
                case "第一页":
                    pagenum = 1;
                    break;
                case "最后一页":
                    pagenum = PageCount;
                    break;
                default:
                    break;

            }

            OperateResult<List<Test_ProjectItemInfo>> Testidresult;
            if (!isnotesttext && !isnotestsn)
            {
                Testidresult = DBOperate.Default.SelectTestProjectItem(lastprodectid, Product_SN, start, end, (int)pagenum, OnePageCount);// : DBOperate.Default.GetTestItemByIDandTime(Product_SN, TestStartTime, TestEndTime);
            }
            else if (!isnotestsn)
            {
                
                Testidresult = DBOperate.Default.SelectTestProjectItem(Product_SN, start, end, (int)pagenum, OnePageCount);
            }
            else
            {
                Testidresult = DBOperate.Default.SelectTestProjectItem(lastprodectid, start, end, (int)pagenum, OnePageCount);
            }

            if (!Testidresult)
            {
                System.Windows.Forms.MessageBox.Show($"数据库查询第{pagenum}页项目信息异常：{Testidresult.Message}", "试验项目条目数据查找失败");
                //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestITemData), nameof(DBOperate.Default.GetTetsItemByTestID), Testidresult.Message); 
                return;
            }

            SelectPage = pagenum;

            Datas.Clear();
            for (int i = 0; i < Testidresult.Data.Count; i++)
            {
                Datas.Add(Testidresult.Data[i]);
            }
        }



        private void ReturnToItem()
        {
            Product_SN = SelectIndex.ToString();
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public TestProjectItemViewModel() 
        {
            GetAllProject();
        }
        #endregion

    }
}
