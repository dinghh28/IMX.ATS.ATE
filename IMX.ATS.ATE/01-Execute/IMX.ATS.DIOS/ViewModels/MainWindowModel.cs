using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.ATE.Common;
using IMX.Common;
using IMX.DB;
using IMX.DB.Model;
using IMX.Logger;
using OfficeOpenXml;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using LicenseContext = OfficeOpenXml.LicenseContext;
using MessageBox = System.Windows.Forms.MessageBox;

namespace IMX.ATS.DIOS
{
    public class MainWindowModel : ExtendViewModelBase
    {
        #region 私有变量
        /// <summary>
        /// 项目信息字典[项目名称，项目信息]
        /// </summary>
        Dictionary<string, Test_ProjectInfo> dicProjectInfo = new Dictionary<string, Test_ProjectInfo>();

        /// <summary>
        /// 选择查询的itemID
        /// </summary>
        private int selectitemID;

        /// <summary>
        /// 选择查询记录的开始时间
        /// </summary>
        private DateTime Starttime;

        /// <summary>
        /// 选择查询记录的结束时间
        /// </summary>
        private DateTime Stoptime;

        /// <summary>
        /// 测试数据表结构
        /// </summary>
        private DataTable DataStructure = new DataTable();

        #endregion

        #region 公共属性

        #region 界面属性

        #region 进度条属性
        private Visibility proBarVisily = Visibility.Hidden;
        /// <summary>
        /// 进度条显示状态
        /// </summary>
        public Visibility ProBarVisily
        {
            get => proBarVisily;
            set => Set(nameof(ProBarVisily), ref proBarVisily, value);
        }

        private int proBarValue;
        /// <summary>
        /// 当前进度
        /// </summary>
        public int ProBarValue
        {
            get => proBarValue;
            set => Set(nameof(ProBarValue), ref proBarValue, value);
        }

        private int proBarMaxValue;
        /// <summary>
        /// 进度完成值
        /// </summary>
        public int ProBarMaxValue
        {
            get => proBarMaxValue;
            set => Set(nameof(ProBarMaxValue), ref proBarMaxValue, value);
        }

        private string proBarText;
        /// <summary>
        /// 进度条显示文本
        /// </summary>
        public string ProBarText
        {
            get => proBarText;
            set => Set(nameof(ProBarText), ref proBarText, value);
        }

        private double testDataCount = 0;

        /// <summary>
        /// 测试数据行数
        /// </summary>
        public double TestDataCount
        {
            get => testDataCount;
            set => Set(nameof(TestDataCount), ref testDataCount, value);
        }

        private long testDataSelectPage;

        /// <summary>
        /// 测试数据当前页
        /// </summary>
        public long TestDataSelectPage
        {
            get => testDataSelectPage;
            set => Set(nameof(TestDataSelectPage), ref testDataSelectPage, value);
        }

        private long testDataALLPage;

        /// <summary>
        /// 测试数据页数
        /// </summary>
        public long TestDataALLPage
        {
            get => testDataALLPage;
            set => Set(nameof(TestDataALLPage), ref testDataALLPage, value);
        }

        #endregion

        private int itemIDSelectindex;

        public int ItemIDSelectindex
        {
            get => itemIDSelectindex;
            set => Set(nameof(ItemIDSelectindex), ref itemIDSelectindex, value);
        }

        private List<int> selectindexs = new List<int>();

        private ObservableCollection<TestItem> testItemDataTable = new ObservableCollection<TestItem>();

        public ObservableCollection<TestItem> TestItemDataTable
        {
            get => testItemDataTable;
            set => Set(nameof(TestItemDataTable), ref testItemDataTable, value);
        }

        private DataTable testDataTable = new DataTable();

        public DataTable TestDataTable
        {
            get => testDataTable;
            set => Set(nameof(TestDataTable), ref testDataTable, value);
        }

        #region 查询条件
        private DateTime testStartTime = DateTime.Now;
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime TestStartTime
        {
            get => testStartTime;
            set
            {
                Set(nameof(TestStartTime), ref testStartTime, value);
            }
        }

        private DateTime testEndTime = DateTime.Now;
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime TestEndTime
        {
            get => testEndTime;
            set
            {
                Set(nameof(TestEndTime), ref testEndTime, value);

            }
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

        public ObservableCollection<string> Product_Tests
        {
            get
            {

                return product_Tests;
            }
            set
            {
                if (Set(nameof(Product_Tests), ref product_Tests, value))
                {
                    //OperateResult<List<string>> Testidresult = DBOperate.Default.GetTestInfo_TestSN();
                    //if (Testidresult)
                    //{
                    //    if (Testidresult.Data.Count > 0)
                    //    {
                    //        Product_Tests?.Clear();
                    //        Testidresult.Data.ForEach(x => Product_Tests.Add(x));
                    //    }
                    //}
                }
            }
        }


        private string product_TestSN;
        /// <summary>
        /// 项目编号
        /// </summary>
        public string Product_TestSN
        {
            get
            {
                return product_TestSN;
            }
            set
            {
                Set(nameof(Product_TestSN), ref product_TestSN, value);
            }
        }
        #endregion
        private int testSelectindex;
        public int TestSelectindex
        {
            get => testSelectindex;
            set => Set(nameof(TestSelectindex), ref testSelectindex, value);
        }

        public List<string> FilterItems { get; set; }

        private string searchTestText;
        /// <summary>
        /// 项目编号
        /// </summary>
        public string SearchTestText
        {
            get => searchTestText;
            set
            {
                Set(nameof(SearchTestText), ref searchTestText, value);
                //if (value.Length < searchTestText.Length)
                //{
                //    TestSelectindex = -1;
                //}
                //if (Set(nameof(SearchTestText), ref searchTestText, value))
                //{
                //    if (value == "请输入项目编号")
                //    {
                //        FilterItems = Product_Tests.ToList();
                //    }
                //    else
                //    {
                //        FilterItems = Product_Tests.Where(x => x.Contains(value.ToUpper())).ToList();
                //        IsTestDropDown = true;
                //    }
                //}
            }
        }

        private bool isTestDropDown = false;

        public bool IsTestDropDown
        {
            get => isTestDropDown;
            set => Set(nameof(IsTestDropDown), ref isTestDropDown, value);
        }


        private string searchtestsn;
        /// <summary>
        /// 查询产品编号
        /// </summary>
        public string SearchTestSN
        {
            get => searchtestsn;
            set => Set(nameof(SearchTestSN), ref searchtestsn, value);
        }


        private string softwareversion;
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string SoftwareVersion
        {
            get => softwareversion;
            set => Set(nameof(SoftwareVersion), ref softwareversion, value);
        }
        #endregion

        #region 指令
        public RelayCommand SelectTestDataCommand => new RelayCommand(QueryTestITemData);
        public RelayCommand AddSelectItemID => new RelayCommand(ADDItemID);

        public RelayCommand<object> SelectTestDataToSave => new RelayCommand<object>(MulExport);

        public RelayCommand<object> SelectedItemChangedCommand => new RelayCommand<object>(QueryTestData);

        //public RelayCommand SelectTestCommand => new RelayCommand(SelectTest);

        //public RelayCommand DelectTestCommand => new RelayCommand(DelectTest);

        public RelayCommand SelectAllCommand => new RelayCommand(SelectAll);

        public RelayCommand UnSelectAllCommand => new RelayCommand(UnSelectAll);

        public RelayCommand<object> SelectNextDataCommand => new RelayCommand<object>(GetTestDataLimit);
        #endregion

        /// <summary>
        /// 数据库内项目ID
        /// </summary>
        public int Product_TestID { get; set; }

        #endregion

        #region 私有方法
        /// <summary>
        /// 查询数据，输出数据item表
        /// </summary>
        private void QueryTestITemData()
        {

            try
            {
                bool isnotesttext = string.IsNullOrEmpty(SearchTestText);
                bool isnotestsn = string.IsNullOrEmpty(SearchTestSN);

                if (isnotesttext && isnotestsn)
                {
                    MessageBox.Show($"请选择需要项目编号或填写产品编号！", "非法查询");
                    return;
                }

                OperateResult<List<Test_ItemInfo>> Testidresult;

                DateTime start = TestStartTime.Date;
                DateTime end = TestEndTime.AddDays(1).Date;

                if (!isnotesttext && !isnotestsn)
                {
                    Testidresult = DBOperate.Default.GetTestItems(dicProjectInfo[SearchTestText].Id, SearchTestSN, start, end);// : DBOperate.Default.GetTestItemByIDandTime(Product_SN, TestStartTime, TestEndTime);
                }
                else if (!isnotestsn)
                {
                    Testidresult = DBOperate.Default.GetTestItems(SearchTestSN, start, end);
                }
                else
                {
                    Testidresult = DBOperate.Default.GetTestItems(dicProjectInfo[SearchTestText].Id, start, end);
                }


                //// 从数据库获取数据的方法  
                //OperateResult<int> idresult = DBOperate.Default.GetTestInfoByTestSN(SearchTestText);
                //if (!idresult)
                //{
                //    System.Windows.Forms.MessageBox.Show($"数据库查询项目信息异常：{idresult.Message}");
                //    return;
                //    //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestITemData), nameof(DBOperate.Default.GetTestInfoByTestSN), idresult.Message);
                //}

                //OperateResult<List<Test_ItemInfo>> Testidresult = DBOperate.Default.GetTestItems(dicProjectInfo[SearchTestText].Id, TestStartTime, TestEndTime.AddDays(1));// : DBOperate.Default.GetTestItemByIDandTime(Product_SN, TestStartTime, TestEndTime);


                if (!Testidresult)
                {
                    System.Windows.Forms.MessageBox.Show($"数据库查询项目信息异常：{Testidresult.Message}");
                    //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestITemData), nameof(DBOperate.Default.GetTetsItemByTestID), Testidresult.Message); 
                    return;
                }

                TestItemDataTable.Clear();
                foreach (Test_ItemInfo item in Testidresult.Data)
                {
                    TestItemDataTable.Add(new TestItem
                    {
                        IsSelect = false,
                        Test_ID = item.ProjectID,
                        TestItem_ID = item.Id,
                        Pro_SN = item.ProductSN,
                        Result = item.Result,
                        ErrorInfo = item.ErrorInfo,
                        Test_ActTime = TimeSpan.FromTicks(item.ActualRunTime).ToString(@"hh\:mm\:ss"),
                        Test_StartTime = item.CreateTime,
                        Test_EndTime = item.UpdateTime,
                        Operator = item.Operator,
                    });
                }
                //TestItemDataTable = Testidresult.Data.Clone();
                //TestItemDataTable = Testidresult.Data;

                //for (int i = 0; i < Testidresult.Data.Rows.Count; i++)
                //{
                //    DataRow dr = TestItemDataTable.NewRow();

                //    for (int k = 0; k < Testidresult.Data.Columns.Count; k++)
                //    {
                //        dr[k] = Testidresult.Data.Rows[i][k];
                //    }

                //    TestItemDataTable.Rows.Add(dr);

                //}
                //foreach (var item in Testidresult.Data)
                //{
                //    TestItemDataTable.Add(item);
                //}
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.DBLOG, nameof(DIOS), nameof(QueryTestITemData), ex); return;

            }

        }

        private void ADDItemID()
        {
            if (!selectindexs.Contains(ItemIDSelectindex))
            { selectindexs.Add(ItemIDSelectindex); }
            else
            {
                selectindexs.Remove(ItemIDSelectindex);
            }
        }
        /// <summary>
        /// 数据导出
        /// </summary>
        //private void MulExport(object selectindex)
        //{
        //    try
        //    {
        //        if (this.TestItemDataTable == null || this.TestItemDataTable.Count == 0)
        //            return;

        //        //Dictionary<int, DateTime> indexes = new Dictionary<int, DateTime>();
        //        List<int> indexes = new List<int>();

        //        for (int i = 0; i < TestItemDataTable?.Count; i++)
        //        {
        //            if (TestItemDataTable[i].IsSelect)
        //            {
        //                indexes.Add(i);
        //            }
        //        }

        //        //TestItemDataTable.ToList().FindAll(x => x.IsSelect == true).ForEach(x => indexes.Add(x.TestItem_ID, x.Test_StartTime));
        //        //{
        //        //    //if (this.dgv_Item.Rows[i].Cells[0].Value != null && (Boolean)this.dgv_Item.Rows[i].Cells[0].Value)
        //        //    indexes.Add(Convert.ToInt32(TestItemDataTable.Rows[Convert.ToInt32(selectindexs[i])][0]));
        //        //}

        //        if (indexes.Count == 0) return;

        //        DialogResult dr = System.Windows.Forms.MessageBox.Show("是否导出所选" + indexes.Count + "项的测试数据？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //        if (dr == DialogResult.No) return;

        //        #region  ///选择存储的文件路径，暂将此功能屏蔽，将存储路径默认至D盘数据文件夹中
        //        //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        //        //folderBrowserDialog.Description = "请选择数据文件保存文件夹";
        //        //folderBrowserDialog.ShowNewFolderButton = true;

        //        //if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
        //        //    return;

        //        //while (true)
        //        //{
        //        //    if (!Directory.Exists(folderBrowserDialog.SelectedPath))
        //        //    {
        //        //        DialogResult fdr = MessageBox.Show("文件夹选择错误，是否重新选择？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //        //        if (fdr == DialogResult.No)
        //        //            return;
        //        //    }
        //        //    else
        //        //        break;

        //        //    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
        //        //        return;
        //        //}
        //        #endregion
        //        //设置进度条控件属性
        //        proBarValue = 0;
        //        proBarMaxValue = indexes.Count;

        //        //this.p_Main.Enabled = false;
        //        //this.p_GetData.Visible = true;


        //        Task.Run(async () =>
        //        {
        //            ProBarVisily = Visibility.Visible;
        //            await Task.Run(() =>
        //            {
        //                MulExportData(indexes);

        //                ProBarVisily = Visibility.Hidden;
        //            });
        //            System.Windows.Forms.MessageBox.Show("数据导出完成");
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(MainWindowModel), nameof(MulExport), ex);

        //    }
        //}

        ////private void MulExportData(int[] indexes)
        ////private void MulExportData(Dictionary<int, DateTime> info)

        //private void MulExportData(List<int> indexes)
        //{
        //    //ProBarVisily = Visibility.Visible;
        //    string path = "";
        //    int pageCount = 1000;
        //    int success = 0, failed = 0;

        //    //List<int> indexes = info?.Keys?.ToList();

        //    for (int i = 0; i < indexes?.Count; i++)
        //    {
        //        ProBarValue = i;
        //        Task.Run(async () =>
        //        {
        //            //while (i < indexes.Length)
        //            //{
        //            await Task.Delay(0);

        //            ProBarText = $"当前进度：正在获取记录({i + 1}/{indexes.Count})";
        //            //}
        //        });

        //        //int id = indexes[i];
        //        //int id = indexes[i];
        //        DataTable getData = new DataTable();

        //        ////var result = DBOperate.Default.GetTestDataByTestItemID(id);
        //        //var result = DBOperate.Default.GetTestItemByIDAndTime(id, info[id]);

        //        //if (!result)
        //        //{
        //        //    SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(DIOS), nameof(MulExportData), result.Message); return; 
        //        //}


        //        //getData = JsonToDataTableConverter(result.Data);

        //        GetTestData(indexes[i]).AttachIfSucceed(result => { getData = result.Data; });

        //        try
        //        {
        //            //DataTable temp = getData.Clone();
        //            //temp.Columns[0].DataType = Type.GetType("System.String");

        //            //for (int j = 0; j < getData.Rows.Count; j++)
        //            //{
        //            //    DataRow dr = temp.NewRow();

        //            //    for (int k = 0; k < getData.Columns.Count; k++)
        //            //    {
        //            //        if (k == 0)
        //            //        {
        //            //            dr[k] = ((DateTime)getData.Rows[j]["RECORDTIME"]).ToString("yyyy/MM/dd HH:mm:ss");
        //            //            continue;
        //            //        }
        //            //        dr[k] = getData.Rows[j][k];
        //            //    }

        //            //    temp.Rows.Add(dr);
        //            //}
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //            // 创建一个ExcelPackage  
        //            ExcelPackage excel = new ExcelPackage();

        //            // 添加一个worksheet并命名为"Data"  
        //            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("TestData");

        //            //Workbook workBook = new Workbook();
        //            //Worksheet workSheet = workBook.Worksheets[0];

        //            if (workSheet != null && getData.Rows.Count != 0)
        //            {

        //                for (int j = 0; j < getData.Columns.Count; j++)
        //                {
        //                    workSheet.Cells[1, j + 1].Value = getData.Columns[j].ColumnName;
        //                }
        //                for (int n = 0; n < getData.Rows.Count; n++)
        //                {
        //                    for (int j = 0; j < getData.Columns.Count; j++)
        //                    {
        //                        workSheet.Cells[n + 2, j + 1].Value = getData.Rows[n][j];
        //                    }
        //                }

        //                //workSheet.Cells.ImportDataTable(temp, false, 1, 0, false);
        //                //workSheet.Cells.SetColumnWidthPixel(0, 100);

        //                //Range range = workSheet.Cells.CreateRange(0, 0, temp.Rows.Count + 1, getData.Columns.Count);

        //                //Style style = workBook.Styles[workBook.Styles.Add()];
        //                //style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //                //style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //                //style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //                //style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //                //style.Font.Size = 12;
        //                //range.SetStyle(style);
        //            }
        //            string proSN = TestItemDataTable.First(x => x.TestItem_ID == indexes[i]).Pro_SN.ToString().Replace("/", "").Replace(" ", "");
        //            string starttime = TestItemDataTable.First(x => x.TestItem_ID == indexes[i]).Test_StartTime.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
        //            //string TestName = TestItemDataTable.Rows[selectindexs[i];// this.lists.Rows[indexes[i]]["DV测试开始作业员"].ToString() + this.lists.Rows[indexes[i]]["试验项目"].ToString();
        //            path = $"D:\\测试数据\\{proSN}";
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }
        //            //String SN = this.lists.Rows[indexes[i]]["产品SN码"].ToString();
        //            //String sTime = ((DateTime)(this.lists.Rows[indexes[i]]["DV测试开始时间"])).ToString("yyyy年MM月dd日 HHmmss");

        //            String fileName = Path.Combine(path, $"{proSN}_{starttime}.xlsx");
        //            excel.SaveAs(new FileInfo(fileName));

        //            success++;
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Windows.MessageBox.Show(ex.Message);
        //            ProBarVisily = Visibility.Hidden;
        //            failed++;
        //        }
        //    }

        //    //this.Invoke(new Action(() =>
        //    //{
        //    //    this.p_Main.Enabled = true;
        //    //    this.p_GetData.Visible = false;

        //    //    MessageBox.Show("导出成功" + success + "组数据，失败" + failed + "组数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //}));
        //}

        private void SelectTest()
        {
            //IsTestDropDown = true;
            //if (SearchTestText.Equals("请输入项目编号"))
            //{ SearchTestText = ""; }
            //else
            //{
            //    FilterItems = Product_Tests.Where(x => x.Contains(SearchTestText.ToUpper())).ToList();
            //    IsTestDropDown = true;

            //}
        }

        private void DelectTest()
        {
            IsTestDropDown = false;
            if (string.IsNullOrEmpty(SearchTestText))
            {
                SearchTestText = "请输入项目编号";
            }
        }

        private void SelectAll()
        {
            TestItemDataTable.ToList().FindAll(model => model.IsSelect = true);
            for (int i = 0; i < TestItemDataTable.Count; i++)
            { selectindexs.Add(i); }
        }

        private void UnSelectAll()
        {
            TestItemDataTable.ToList().FindAll(model => model.IsSelect = false);
            selectindexs.Clear();
        }
        /// <summary>
        /// 测试数据查询
        /// </summary>
        /// <param name="selectindex"></param>
        private void QueryTestData(object selectindex)
        {
            try
            {
                if ((int)selectindex < 0) { return; }
                TestDataTable.Clear();

                selectitemID = Convert.ToInt32(TestItemDataTable[Convert.ToInt32(selectindex)].TestItem_ID);
                Starttime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(selectindex)].Test_StartTime);
                Stoptime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(selectindex)].Test_EndTime);
                string prosn = TestItemDataTable[Convert.ToInt32(selectindex)].Pro_SN;

                DBOperate.Default.GetTestDataCount(selectitemID, Starttime, Stoptime).AttachIfSucceed(result =>
                {
                    const int count = 1080;

                    TestDataALLPage = ((result.Data % count) == 0) ? (result.Data / count) : ((result.Data / count) + 1);
                    TestDataSelectPage = 1;
                    DataStructure = new DataTable();
                    GetTestDataLimit( "第一页");
                })
                 .AttachIfFailed(result =>
                 {
                     MessageBox.Show($"测试数据查询失败：{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                 });

                //OperateResult<List<Test_DataInfo>> Testdataresult;
                //int testitemid = Convert.ToInt32(TestItemDataTable[Convert.ToInt32(selectindex)].TestItem_ID);
                //DateTime time = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(selectindex)].Test_StartTime);
                //string prosn = TestItemDataTable[Convert.ToInt32(selectindex)].Pro_SN;

                //// 从数据库获取数据的方法  
                //Testdataresult = DBOperate.Default.GetTestData(testitemid, $"{prosn}_{time.Ticks}");

                //if (!Testdataresult)
                //{ SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(DIOS), nameof(QueryTestData), Testdataresult.Message); return; }

                //TestDataTable = JsonToDataTableConverter(Testdataresult.Data);
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.DBLOG, nameof(DIOS.MainWindow), nameof(QueryTestData), ex);
            }
        }
        /// <summary>
        /// 查询所有测试数据 暂停使用-20240919 dhh
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        //private OperateResult<DataTable> GetTestData(int Index) 
        //{
        //    try
        //    {
        //        if (Index < 0) { return OperateResult<DataTable>.Failed(null, "序列号不可为负"); }
        //        OperateResult<List<Test_DataInfo>> Testdataresult;
        //        int testitemid = Convert.ToInt32(TestItemDataTable[Convert.ToInt32(Index)].TestItem_ID);
        //        DateTime Starttime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(Index)].Test_StartTime);
        //        DateTime stoptime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(Index)].Test_EndTime);
        //        string prosn = TestItemDataTable[Convert.ToInt32(Index)].Pro_SN;

        //        // 从数据库获取数据的方法  
        //        //Testdataresult = DBOperate.Default.GetTestData(testitemid, $"{prosn}_{time.Ticks}");
        //        Testdataresult = DBOperate.Default.GetTestData(testitemid, Starttime, stoptime);
        //        if (!Testdataresult)
        //        { 
        //            SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(DIOS), nameof(GetTestData), Testdataresult.Message);
        //            return OperateResult<DataTable>.Failed(null, Testdataresult.Message);
        //        }

        //        DataTable item = JsonToDataTableConverter(Testdataresult.Data);
        //        return OperateResult<DataTable>.Succeed(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        SuperDHHLoggerManager.Exception(LoggerType.DBLOG, nameof(DIOS), nameof(GetTestData), ex);

        //        return OperateResult<DataTable>.Failed(null,ex.GetMessage());
        //    }
        //}

        /// <summary>
        /// 分页查询
        /// </summary>
        private void GetTestDataLimit(object pagename)
        {
            try
            {
                if ((TestDataSelectPage == 1) && pagename.ToString() == "上一页")
                {
                    MessageBox.Show($"当前为数据最前一页，无法继续查询！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if ((TestDataSelectPage == TestDataALLPage) && pagename.ToString() == "下一页")
                {
                    MessageBox.Show($"当前为数据最后一页，无法继续查询！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                TestDataTable.Clear();
                TestDataCount = 0;

                switch (pagename.ToString())
                {
                    case "上一页":
                        TestDataSelectPage--;
                        break;
                    case "下一页":
                        TestDataSelectPage++;
                        break;
                    case "第一页":
                        TestDataSelectPage = 1;
                        break;
                    case "最后一页":
                        TestDataSelectPage = TestDataALLPage;
                        break;
                    default:
                        break;

                }

                // 从数据库获取数据的方法  
                DBOperate.Default.GetLimitTestData(selectitemID, Starttime, Stoptime, TestDataSelectPage).AttachIfFailed(result =>
                {
                    MessageBox.Show($"数据查询失败：{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                })
                .AttachIfSucceed(result =>
                {
                    if (DataStructure.Columns.Count == 0)
                    {
                        GetDataTableStructure(result.Data).AttachIfSucceed(resul =>
                        {
                            DataStructure = resul.Data;
                        });
                        //if (result)
                        //{
                        //    DataStructure = result.Data;
                        //}
                    }

                    TestDataTable = JsonToDataTableConverter(DataStructure, result.Data);

                    TestDataCount = result.Data.Count;
                });

                //if (!Testdataresult)
                //{
                //    //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestData), nameof(DBOperate.Default.GetLimitTestData), Testdataresult.Message);
                //    MessageBox.Show($"数据查询失败：{Testdataresult.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                //if (DataStructure.Columns.Count == 0)
                //{
                //    OperateResult<DataTable> result = GetDataTableStructure(Testdataresult.Data);
                //    if (result)
                //    {
                //        DataStructure = result.Data;
                //    }
                //}

                //TestDataTable = JsonToDataTableConverter(DataStructure, Testdataresult.Data);

                //TestDataCount = Testdataresult.Data.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据查询异常：{ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// 测试数据导出
        /// </summary>
        /// <param name="selectindex"></param>
        private void MulExport(object selectindex)
        {
            try
            {
                if (this.TestItemDataTable == null || this.TestItemDataTable.Count == 0)
                    return;

                List<int> indexes = new List<int>();
                List<DateTime> Starttimes = new List<DateTime>();
                List<DateTime> stoptimes = new List<DateTime>();
                TestItemDataTable.ToList().FindAll(x => x.IsSelect == true).ForEach(x =>
                {
                    indexes.Add(x.TestItem_ID);
                    Starttimes.Add(x.Test_StartTime);
                    stoptimes.Add(x.Test_EndTime);
                });

                if (indexes.Count == 0) return;

                DialogResult dr = System.Windows.Forms.MessageBox.Show("是否导出所选" + indexes.Count + "项的测试数据？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No) return;

                #region  ///选择存储的文件路径，暂将此功能屏蔽，将存储路径默认至D盘数据文件夹中
                //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                //folderBrowserDialog.Description = "请选择数据文件保存文件夹";
                //folderBrowserDialog.ShowNewFolderButton = true;

                //if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                //    return;

                //while (true)
                //{
                //    if (!Directory.Exists(folderBrowserDialog.SelectedPath))
                //    {
                //        DialogResult fdr = MessageBox.Show("文件夹选择错误，是否重新选择？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //        if (fdr == DialogResult.No)
                //            return;
                //    }
                //    else
                //        break;

                //    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                //        return;
                //}
                #endregion
                //设置进度条控件属性
                proBarValue = 0;
                proBarMaxValue = indexes.Count;

                //this.p_Main.Enabled = false;
                //this.p_GetData.Visible = true;


                Task.Run(async () =>
                {
                    ProBarVisily = Visibility.Visible;
                    await Task.Run(() =>
                    {
                        MulExportData(indexes.ToArray(), Starttimes.ToArray(), stoptimes.ToArray()).AttachIfSucceed(result =>
                        {
                            MessageBox.Show($"数据导出完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        })
                        .AttachIfFailed(result =>
                        {
                            MessageBox.Show($"测试数据导出失败：{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        });

                        //if (result)
                        //{ System.Windows.Forms.MessageBox.Show("数据导出完成"); }
                        ProBarVisily = Visibility.Hidden;
                    });

                });
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.DBLOG, nameof(MainWindowModel), nameof(MulExport), ex);
            }
        }

        private OperateResult MulExportData(int[] indexes, DateTime[] starttimes, DateTime[] endtimes)
        {
            //ProBarVisily = Visibility.Visible;
            string path = "";
            int pageCount = 1000;
            int success = 0, failed = 0;
            long pages = 0;
            long pageindex = 0;

            #region 通过SaveFileDialog获取文件保存路径和文件名
            //// 创建保存对话框
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            #endregion

            string selectedPath = "";

            Thread t = new Thread(() =>
            {
                FolderBrowserDialog saveFileDialog1 = new FolderBrowserDialog();

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = saveFileDialog1.SelectedPath;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            //点了保存按钮进入
            if (selectedPath != "")
            {
                try
                {
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        DataTable getData = new DataTable();

                        ProBarValue = i;

                        int id = indexes[i];
                        DateTime startime = starttimes[i];
                        DateTime endime = endtimes[i];
                        #region 分页获取测试数据

                        OperateResult<long> resu = DBOperate.Default.GetTestDataCount(id, startime, endime);
                        if (!resu)
                        {
                            if (MessageBox.Show($"测试数据查询失败：{resu.Message}\r\n 是否继续导出未完成数据？", "失败", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                            {
                                return OperateResult.Failed();
                            }
                        }
                        pages = ((resu.Data % pageCount) == 0) ? (resu.Data / pageCount) : ((resu.Data / pageCount) + 1);



                        for (long n = 0; n < pages; n++)
                        {
                            Task.Run(async () =>
                            {
                                //while (i < indexes.Length)
                                //{
                                await Task.Delay(0);

                                ProBarText = $"当前进度：正在获取第{i + 1}记录分页数据：{pageindex}/{pages}";
                                //}
                            });
                            pageindex = n + 1;

                            DBOperate.Default.GetLimitTestData(id, startime, endime, n + 1)
                            .AttachIfSucceed(rlt =>
                            {
                                if (getData.Columns.Count == 0)
                                {
                                    GetDataTableStructure(rlt.Data).AttachIfSucceed(resl =>
                                    { getData = resl.Data; });
                                    //if (result1)
                                    //{
                                    //    getData = result1.Data;
                                    //}
                                }

                                DataTable pagedata = JsonToDataTableConverter(getData, rlt.Data);

                                //if (getData.Columns.Count == 0) getData = pagedata.Clone();

                                for (int x = 0; x < pagedata.Rows.Count; x++)
                                {
                                    DataRow row = getData.NewRow();
                                    foreach (var column in pagedata.Columns)
                                    {
                                        row[column.ToString()] = pagedata.Rows[x][column.ToString()];
                                        row[0] = getData.Rows.Count;
                                    }

                                    //for (int y = 0; y < pagedata.Columns.Count; y++)
                                    //{
                                    //    row[y] = pagedata.Rows[x][y];
                                    //    row[0] = getData.Rows.Count;
                                    //}
                                    getData.Rows.Add(row);
                                }
                            }).AttachIfFailed(result =>
                            {
                                SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestData), nameof(DBOperate.Default.GetLimitTestData), result.Message);
                                return OperateResult.Failed(result.Message);
                            });

                            //if (!result)
                            //{
                            //    SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestData), nameof(DBOperate.Default.GetLimitTestData), result.Message); 
                            //    return OperateResult.Failed(result.Message);
                            //}

                            //if (getData.Columns.Count == 0)
                            //{
                            //     GetDataTableStructure(result.Data).AttachIfSucceed(resl=>
                            //    { getData = resl.Data; });
                            //    //if (result1)
                            //    //{
                            //    //    getData = result1.Data;
                            //    //}
                            //}

                            //DataTable pagedata = JsonToDataTableConverter(getData, result.Data);

                            ////if (getData.Columns.Count == 0) getData = pagedata.Clone();

                            //for (int x = 0; x < pagedata.Rows.Count; x++)
                            //{
                            //    DataRow row = getData.NewRow();
                            //    foreach (var column in pagedata.Columns)
                            //    {
                            //        row[column.ToString()] = pagedata.Rows[x][column.ToString()];
                            //        row[0] = getData.Rows.Count;
                            //    }

                            //    //for (int y = 0; y < pagedata.Columns.Count; y++)
                            //    //{
                            //    //    row[y] = pagedata.Rows[x][y];
                            //    //    row[0] = getData.Rows.Count;
                            //    //}
                            //    getData.Rows.Add(row);
                            //}

                        }
                        #endregion

                        try
                        {
                            ////给文件名前加上时间
                            string SN = TestItemDataTable.ToList().Find(x => x.TestItem_ID == id).Pro_SN;
                            string Starttime = TestItemDataTable.ToList().Find(x => x.TestItem_ID == id).Test_StartTime.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");//"yyyy/MM/dd HH:mm:ss"

                            string newFileName = $"[{SN}]-{Starttime}.csv";// + "_" + DateTime.Now.ToString("yyyyMMdd");
                            newFileName = selectedPath + "\\" + newFileName;

                            OperateResult result1 = EstablishCSV(getData, newFileName);
                            if (!result1)
                            {
                                if (System.Windows.MessageBox.Show($"导出失败：{result1.Message}！是否继续导出未完成数据？", "导出数据", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                {
                                    return OperateResult.Failed();
                                }
                            }

                            success++;
                        }
                        catch (Exception ex)
                        {
                            if (System.Windows.MessageBox.Show($"导出异常:{ex.Message}!是否继续导出未完成数据？", "异常", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return OperateResult.Failed();
                            }
                            ProBarVisily = Visibility.Hidden;
                            failed++;
                        }

                    }
                }
                catch (Exception ex)
                {
                    if (System.Windows.MessageBox.Show($"导出异常:{ex.Message}!", "异常", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return OperateResult.Failed();
                    }
                }

            }

            return OperateResult.Succeed();

            //this.Invoke(new Action(() =>
            //{
            //    this.p_Main.Enabled = true;
            //    this.p_GetData.Visible = false;

            //    MessageBox.Show("导出成功" + success + "组数据，失败" + failed + "组数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}));
        }


        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件名</param>
        private OperateResult EstablishCSV(DataTable dt, string fileName)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                string head = "";
                //拼接列头
                for (int cNum = 0; cNum < dt.Columns.Count; cNum++)
                {
                    head += dt.Columns[cNum].ColumnName + ",";
                }
                //csv文件写入列头
                sw.WriteLine(head);
                string data = "";
                //csv写入数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string data2 = string.Empty;
                    //拼接行数据
                    for (int cNum1 = 0; cNum1 < dt.Columns.Count; cNum1++)
                    {
                        if (dt.Columns[cNum1].ColumnName == "记录时间")
                        { data2 = data2 + "\"‘" + dt.Rows[i][dt.Columns[cNum1].ColumnName].ToString() + "’\","; }
                        else
                        { data2 = data2 + "\"" + dt.Rows[i][dt.Columns[cNum1].ColumnName].ToString() + "\","; }
                    }
                    bool flag = data != data2;
                    if (flag)
                    {
                        sw.WriteLine(data2);
                    }
                    data = data2;

                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                string error = $"导出csv失败！{ex.Message} ";
                return OperateResult.Failed(error);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                sw = null;
                fs = null;
            }
        }
        /// <summary>
        /// 创建测试数据表结构
        /// </summary>
        private OperateResult<DataTable> GetDataTableStructure(List<Test_DataInfo> value)
        {
            try
            {
                DataTable table = new DataTable();

                table.Columns.Add("ID");
                table.Columns.Add("记录时间");

                table.Columns.Add("试验项名称");
                table.Columns.Add("步骤序号");
                table.Columns.Add("步骤名称");
                // 遍历并添加列到DataTable  
                value[0].Pro_Data.ForEach(token =>
                {
                    table.Columns.Add(token.Name);

                });

                // 遍历并添加列到DataTable  
                value[0].Euq_Data.ForEach(token =>
                {
                    table.Columns.Add(token.Name);
                });

                return OperateResult<DataTable>.Succeed(table);
            }
            catch (Exception ex)
            {
                return OperateResult<DataTable>.Failed(null, ex.Message);
            }

        }

        /// <summary>
        /// 加载测试表中的测试数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private DataTable JsonToDataTableConverter(DataTable datastructtable, List<Test_DataInfo> value)
        {
            var table = datastructtable.Clone();

            value.ForEach(x =>
            {
                DataRow row = table.NewRow();
                row["ID"] = table.Rows.Count + 1;
                //row["记录时间"] = x.RecordTime;
                row["记录时间"] = x.CreateTime;//.ToString("yyyy/MM/dd HH:mm:ss.fff");

                row["试验项名称"] = x.FlowName;

                row["步骤序号"] = x.StepIndex;

                row["步骤名称"] = x.StepName;

                x.Pro_Data.ForEach(y =>
                {
                    row[y.Name] = y.Value;
                });
                x.Euq_Data.ForEach(y =>
                {
                    row[y.Name] = y.Value;
                });
                table.Rows.Add(row);
            });

            return table;

        }


        /// <summary>
        /// 转换测试数据格式，从JSON格式转为表格 暂停使用-20240919 dhh
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //private DataTable JsonToDataTableConverter(List<Test_DataInfo> value)
        //{
        //    var table = new DataTable();

        //    //table.Columns.Add("ID");
        //    table.Columns.Add("记录时间");

        //    for (int i = 0; i < value?.Count; i++)
        //    {
        //        // 遍历并添加列到DataTable  
        //        value[i].Pro_Data.ForEach(token =>
        //        {
        //            if (!table.Columns.Contains(token.Name))
        //            { table.Columns.Add(token.Name); }
        //            //table.Columns.Add(token.Name);

        //            //var columnName = token.Data_Name.ToString();
        //            //if (!table.Columns.Contains(columnName))
        //            //{ table.Columns.Add(columnName, typeof(System.Object)); }
        //        });

        //        // 遍历并添加列到DataTable  
        //        value[i].Euq_Data.ForEach(token =>
        //        {
        //            if (!table.Columns.Contains(token.Name))
        //            { table.Columns.Add(token.Name); }
        //            //table.Columns.Add(token.Name);

        //            //var columnName = token.Data_Name.ToString();
        //            //if (!table.Columns.Contains(columnName))
        //            //{ table.Columns.Add(columnName, typeof(System.Object)); }
        //        });
        //    }




        //    value.ForEach(x =>
        //    {
        //        DataRow row = table.NewRow();
        //        //row["ID"] = table.Rows.Count + 1;
        //        row["记录时间"] = x.CreateTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
        //        x.Pro_Data.ForEach(y =>
        //        {
        //            row[y.Name] = y.Value;
        //        });
        //        x.Euq_Data.ForEach(y =>
        //        {
        //            row[y.Name] = y.Value;
        //        });
        //        table.Rows.Add(row);
        //    });

        //    return table;
        //}

        /// <summary>
        /// 获取测试起始和结束时间范围
        /// </summary>
        private void GetDateRange()
        {

            try
            {
                DBOperate.Default.GetTestRangeStartTime().AttachIfSucceed(result =>
                {
                    TestStartTime = result.Data;
                }).And(DBOperate.Default.GetTestRangeEndTime().AttachIfSucceed(result =>
                {
                    TestEndTime = result.Data;
                })).AttachIfFailed(result =>
                {
                    MessageBox.Show($"测试时间获取失败：{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                });

                //if (!result || !resu)
                //{
                //    string lasterr = !result ? result.Message : resu.Message;
                //    MessageBox.Show($"测试时间获取失败：{lasterr}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                //TestStartTime = result.Data;
                //TestEndTime = resu.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试时间获取异常：{ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

        }

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

        public MainWindowModel()
        {
            DBOperate.Default.Init().AttachIfSucceed(result =>
            {

                //GetDateRange();
                GetAllProject();
            }).AttachIfFailed(result => { System.Windows.Forms.MessageBox.Show($"请重新打开查询工具,数据库初始化异常:{result.Message}"); });

            SoftwareVersion = SysteamInfo.SoftwareVersion;
            //TestStartTime = DateTime.Now;
            //TestEndTime = DateTime.Now.AddDays(1);
        }

        public MainWindowModel(string sn, int product_TestID)
        {

            Product_TestSN = sn;
            Product_TestID = product_TestID;

        }
    }
    public class TestItem : ViewModelBase
    {
        private bool isSelect;
        public bool IsSelect
        {
            get => isSelect;
            set => Set(nameof(IsSelect), ref isSelect, value);
        }

        /// <summary>
        /// 对应项目ID
        /// </summary>
        public int Test_ID { get; set; }

        public int TestItem_ID { get; set; }

        //样品编号
        public string Pro_SN { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Pro_Name { get; set; }

        //public uint Test_CycleNum { get; set; }

        /// <summary>
        /// 实验结果
        /// </summary>
        public ResultState Result { get; set; }

        //public double Test_ActTime { get; set; }

        /// <summary>
        /// 实际运行时间（hh:mm:ss）
        /// </summary>
        public string Test_ActTime { get; set; }

        public DateTime Test_StartTime { get; set; }

        public DateTime Test_EndTime { get; set; }

        /// <summary>
        /// 故障信息
        /// </summary>
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 操作人员
        /// </summary>
        public string Operator { get; set; }

    }
}
