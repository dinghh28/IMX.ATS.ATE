using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace IMX.ATS.DIOS
{
    public class MainWindowModel : ExtendViewModelBase
    {
        #region 私有变量
        /// <summary>
        /// 项目信息字典[项目名称，项目信息]
        /// </summary>
        Dictionary<string, Test_ProjectInfo> dicProjectInfo = new Dictionary<string, Test_ProjectInfo>();
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
        private DateTime testStartTime;
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

        private DateTime testEndTime;
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
        /// <summary>
        /// 项目编号
        /// </summary>
        private string searchTestText ;

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
                if (string.IsNullOrEmpty(SearchTestText))
                {
                    System.Windows.Forms.MessageBox.Show($"请选择需要项目编号！");
                    return;
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


                //if (!Testidresult)
                //{
                //    System.Windows.Forms.MessageBox.Show($"数据库查询项目信息异常：{Testidresult.Message}");
                //    //SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(QueryTestITemData), nameof(DBOperate.Default.GetTetsItemByTestID), Testidresult.Message); 
                //    return;
                //}

                //TestItemDataTable.Clear();
                //foreach (Test_ItemInfo item in Testidresult.Data)
                //{
                //    TestItemDataTable.Add(new TestItem
                //    {
                //        IsSelect = false,
                //        Test_ID = item.ProjectID,
                //        TestItem_ID = item.Id,
                //        Pro_SN = item.ProductSN,
                //        Result = item.Result,
                //        ErrorInfo = item.ErrorInfo,
                //        Test_ActTime = item.ActualRunTime,
                //        Test_StartTime = item.CreateTime,
                //        Test_EndTime = item.UpdateTime,
                //    });
                //}
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

        private void MulExport(object selectindex)
        {
            try
            {
                if (this.TestItemDataTable == null || this.TestItemDataTable.Count == 0)
                    return;

                //Dictionary<int, DateTime> indexes = new Dictionary<int, DateTime>();
                List<int> indexes = new List<int>();

                for (int i = 0; i < TestItemDataTable?.Count; i++)
                {
                    if (TestItemDataTable[i].IsSelect)
                    {
                        indexes.Add(i);
                    }
                }

                //TestItemDataTable.ToList().FindAll(x => x.IsSelect == true).ForEach(x => indexes.Add(x.TestItem_ID, x.Test_StartTime));
                //{
                //    //if (this.dgv_Item.Rows[i].Cells[0].Value != null && (Boolean)this.dgv_Item.Rows[i].Cells[0].Value)
                //    indexes.Add(Convert.ToInt32(TestItemDataTable.Rows[Convert.ToInt32(selectindexs[i])][0]));
                //}

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
                        MulExportData(indexes);

                        ProBarVisily = Visibility.Hidden;
                    });
                    System.Windows.Forms.MessageBox.Show("数据导出完成");
                });
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.TESTLOG, nameof(MainWindowModel), nameof(MulExport), ex);

            }
        }

        //private void MulExportData(int[] indexes)
        //private void MulExportData(Dictionary<int, DateTime> info)

        private void MulExportData(List<int> indexes)
        {
            //ProBarVisily = Visibility.Visible;
            string path = "";
            int pageCount = 1000;
            int success = 0, failed = 0;

            //List<int> indexes = info?.Keys?.ToList();

            for (int i = 0; i < indexes?.Count; i++)
            {
                ProBarValue = i;
                Task.Run(async () =>
                {
                    //while (i < indexes.Length)
                    //{
                    await Task.Delay(0);

                    ProBarText = $"当前进度：正在获取记录({i + 1}/{indexes.Count})";
                    //}
                });

                //int id = indexes[i];
                //int id = indexes[i];
                DataTable getData = new DataTable();

                ////var result = DBOperate.Default.GetTestDataByTestItemID(id);
                //var result = DBOperate.Default.GetTestItemByIDAndTime(id, info[id]);

                //if (!result)
                //{
                //    SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(DIOS), nameof(MulExportData), result.Message); return; 
                //}


                //getData = JsonToDataTableConverter(result.Data);

                GetTestData(indexes[i]).AttachIfSucceed(result => { getData = result.Data; });

                try
                {
                    //DataTable temp = getData.Clone();
                    //temp.Columns[0].DataType = Type.GetType("System.String");

                    //for (int j = 0; j < getData.Rows.Count; j++)
                    //{
                    //    DataRow dr = temp.NewRow();

                    //    for (int k = 0; k < getData.Columns.Count; k++)
                    //    {
                    //        if (k == 0)
                    //        {
                    //            dr[k] = ((DateTime)getData.Rows[j]["RECORDTIME"]).ToString("yyyy/MM/dd HH:mm:ss");
                    //            continue;
                    //        }
                    //        dr[k] = getData.Rows[j][k];
                    //    }

                    //    temp.Rows.Add(dr);
                    //}
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // 创建一个ExcelPackage  
                    ExcelPackage excel = new ExcelPackage();

                    // 添加一个worksheet并命名为"Data"  
                    ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("TestData");

                    //Workbook workBook = new Workbook();
                    //Worksheet workSheet = workBook.Worksheets[0];

                    if (workSheet != null && getData.Rows.Count != 0)
                    {

                        for (int j = 0; j < getData.Columns.Count; j++)
                        {
                            workSheet.Cells[1, j + 1].Value = getData.Columns[j].ColumnName;
                        }
                        for (int n = 0; n < getData.Rows.Count; n++)
                        {
                            for (int j = 0; j < getData.Columns.Count; j++)
                            {
                                workSheet.Cells[n + 2, j + 1].Value = getData.Rows[n][j];
                            }
                        }

                        //workSheet.Cells.ImportDataTable(temp, false, 1, 0, false);
                        //workSheet.Cells.SetColumnWidthPixel(0, 100);

                        //Range range = workSheet.Cells.CreateRange(0, 0, temp.Rows.Count + 1, getData.Columns.Count);

                        //Style style = workBook.Styles[workBook.Styles.Add()];
                        //style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        //style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                        //style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        //style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        //style.Font.Size = 12;
                        //range.SetStyle(style);
                    }
                    string proSN = TestItemDataTable.First(x => x.TestItem_ID == indexes[i]).Pro_SN.ToString().Replace("/", "").Replace(" ", "");
                    string starttime = TestItemDataTable.First(x => x.TestItem_ID == indexes[i]).Test_StartTime.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
                    //string TestName = TestItemDataTable.Rows[selectindexs[i];// this.lists.Rows[indexes[i]]["DV测试开始作业员"].ToString() + this.lists.Rows[indexes[i]]["试验项目"].ToString();
                    path = $"D:\\测试数据\\{proSN}";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //String SN = this.lists.Rows[indexes[i]]["产品SN码"].ToString();
                    //String sTime = ((DateTime)(this.lists.Rows[indexes[i]]["DV测试开始时间"])).ToString("yyyy年MM月dd日 HHmmss");

                    String fileName = Path.Combine(path, $"{proSN}_{starttime}.xlsx");
                    excel.SaveAs(new FileInfo(fileName));

                    success++;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    ProBarVisily = Visibility.Hidden;
                    failed++;
                }
            }

            //this.Invoke(new Action(() =>
            //{
            //    this.p_Main.Enabled = true;
            //    this.p_GetData.Visible = false;

            //    MessageBox.Show("导出成功" + success + "组数据，失败" + failed + "组数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}));
        }

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
                GetTestData((int)selectindex).AttachIfSucceed(result =>
                {
                    TestDataTable = result.Data;
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

        private OperateResult<DataTable> GetTestData(int Index) 
        {
            try
            {
                if (Index < 0) { return OperateResult<DataTable>.Failed(null, "序列号不可为负"); }
                OperateResult<List<Test_DataInfo>> Testdataresult;
                int testitemid = Convert.ToInt32(TestItemDataTable[Convert.ToInt32(Index)].TestItem_ID);
                DateTime Starttime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(Index)].Test_StartTime);
                DateTime stoptime = Convert.ToDateTime(TestItemDataTable[Convert.ToInt32(Index)].Test_EndTime);
                string prosn = TestItemDataTable[Convert.ToInt32(Index)].Pro_SN;

                //// 从数据库获取数据的方法  
                ////Testdataresult = DBOperate.Default.GetTestData(testitemid, $"{prosn}_{time.Ticks}");
                //Testdataresult = DBOperate.Default.GetTestData(testitemid, Starttime, stoptime);
                //if (!Testdataresult)
                //{ 
                //    SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(DIOS), nameof(GetTestData), Testdataresult.Message);
                //    return OperateResult<DataTable>.Failed(null, Testdataresult.Message);
                //}

                //DataTable item = JsonToDataTableConverter(Testdataresult.Data);
                //return OperateResult<DataTable>.Succeed(item);
                return OperateResult<DataTable>.Succeed(new DataTable());
            }
            catch (Exception ex)
            {
                SuperDHHLoggerManager.Exception(LoggerType.DBLOG, nameof(DIOS), nameof(GetTestData), ex);

                return OperateResult<DataTable>.Failed(null,ex.GetMessage());
            }



        }


        private DataTable JsonToDataTableConverter(List<Test_DataInfo> value)
        {
            var table = new DataTable();

            //table.Columns.Add("ID");
            table.Columns.Add("记录时间");

            for (int i = 0; i < value?.Count; i++)
            {
                // 遍历并添加列到DataTable  
                value[i].Pro_Data.ForEach(token =>
                {
                    if (!table.Columns.Contains(token.Name))
                    { table.Columns.Add(token.Name); }
                    //table.Columns.Add(token.Name);

                    //var columnName = token.Data_Name.ToString();
                    //if (!table.Columns.Contains(columnName))
                    //{ table.Columns.Add(columnName, typeof(System.Object)); }
                });

                // 遍历并添加列到DataTable  
                value[i].Euq_Data.ForEach(token =>
                {
                    if (!table.Columns.Contains(token.Name))
                    { table.Columns.Add(token.Name); }
                    //table.Columns.Add(token.Name);

                    //var columnName = token.Data_Name.ToString();
                    //if (!table.Columns.Contains(columnName))
                    //{ table.Columns.Add(columnName, typeof(System.Object)); }
                });
            }
            



            value.ForEach(x =>
            {
                DataRow row = table.NewRow();
                //row["ID"] = table.Rows.Count + 1;
                row["记录时间"] = x.CreateTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
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
        /// 获取测试起始和结束时间范围
        /// </summary>
        private void GetDateRange()
        {
            //OperateResult<DateTime[]> Testidresult = DBOperate.Default.GetTestRangeTime();

            //if (!Testidresult)
            //{
            //    System.Windows.Forms.MessageBox.Show($"试验数据时间范围获取异常:{Testidresult.Message}");
            //    SuperDHHLoggerManager.Error(LoggerType.DBLOG, nameof(GetDateRange), nameof(DBOperate.Default.GetTestRangeTime), Testidresult.Message);
            //    return;
            //}
            //TestStartTime = Testidresult.Data[0];
            //TestEndTime = Testidresult.Data[1];
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

            TestStartTime = DateTime.Today;
            TestEndTime = DateTime.Now.AddDays(1);
        }
        #endregion

        public MainWindowModel()
        {
            DBOperate.Default.Init().AttachIfSucceed(result =>
            {
                GetDateRange();
            }).AttachIfFailed(result => { System.Windows.Forms.MessageBox.Show($"请重新打开查询工具,数据库初始化异常:{result.Message}"); });
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

        public double Test_ActTime { get; set; }

        public DateTime Test_StartTime { get; set; }

        public DateTime Test_EndTime { get; set; }

        /// <summary>
        /// 故障信息
        /// </summary>
        public string ErrorInfo { get; set; }
    }
}
