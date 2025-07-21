#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DIOS.ViewModels
 * 唯一标识：df0c705a-d408-45cd-b616-a63beed42c94
 * 文件名：TestItemViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/12 19:34:24
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

using Aspose.Cells;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using H.WPF.Framework;
using IMX.Common;
using IMX.DB;
using IMX.DB.Model;
using IMX.Logger;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Debugger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using MessageBox = System.Windows.Forms.MessageBox;
using Style = Aspose.Cells.Style;

namespace IMX.ATS.DIOS
{
    public class TestItemViewModel : ExtendViewModelBase
    {
        #region 公共属性

        #region 界面绑定属性

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

        #region 检索条件
        private string flowname = string.Empty;
        /// <summary>
        /// 试验项名称（模糊查询）
        /// </summary>
        public string FlowName
        {
            get => flowname;
            set => Set(nameof(FlowName), ref flowname, value);
        }

        private int resultindex = -1;
        /// <summary>
        /// 当前检索试验结果条件
        /// </summary>
        public int ResultIndex
        {
            get => resultindex;
            set => Set(nameof(ResultIndex), ref resultindex, value);
        }

        #endregion

        private ObservableCollection<TestItemModel> datas = new ObservableCollection<TestItemModel>();
        /// <summary>
        /// 测试项数据列表
        /// </summary>
        public ObservableCollection<TestItemModel> Datas
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
        #endregion

        #region 界面绑定指令
        public RelayCommand Search => new RelayCommand(SearchItem);

        public RelayCommand<string> Export => new RelayCommand<string>(MulExport);

        public RelayCommand Clear => new RelayCommand(() =>
        {
            FlowName = string.Empty;
            ResultIndex = -1;
            SearchItem();
        });

        public RelayCommand<object> OpenDatas => new RelayCommand<object>(DatasWindowOpen);

        /// <summary>
        /// 全选指令
        /// </summary>
        public RelayCommand<string> SelectAllCommand => new RelayCommand<string>(SelectAll);

        private void SelectAll(string obj)
        {
            if (obj == "C")
            {
                SelectAll();
            }
            else if (obj == "U")
            {
                UnSelectAll();
            }
        }
        #endregion

        #endregion

        #region 私有变量
        private long proid = -1;
        /// <summary>
        /// 数据源（原始数据库内容，为检索原始列表）
        /// </summary>
        private ObservableCollection<Test_ItemInfo> datasources = new ObservableCollection<Test_ItemInfo>();

        private List<int> selectindexs = new List<int>();
        #endregion

        #region 私有方法

        #region 全选
        private void SelectAll()
        {
            Datas.ToList().FindAll(model => model.IsSelect = true);
            for (int i = 0; i < Datas.Count; i++)
            { selectindexs.Add(i); }
        }

        private void UnSelectAll()
        {
            Datas.ToList().FindAll(model => model.IsSelect = false);
            selectindexs.Clear();
        }
        #endregion

        #region 导出方案
        private void MulExport(string obj)
        {
            if (Datas == null || Datas.Count < 1)
            {
                return;
            }

            List<long> indexes = new List<long>();
            List<DateTime> Starttimes = new List<DateTime>();
            List<DateTime> stoptimes = new List<DateTime>();
            List<Test_ItemInfo> itemInfos = new List<Test_ItemInfo>();
            Datas.ToList().FindAll(x => x.IsSelect == true).ForEach(x =>
            {
                itemInfos.Add(x.Data);
                indexes.Add(x.Data.Id);
                Starttimes.Add(x.Data.CreateTime);
                stoptimes.Add(x.Data.UpdateTime);
            });

            if (indexes.Count == 0) return;

            DialogResult dr = System.Windows.Forms.MessageBox.Show("是否导出所选" + indexes.Count + "项的测试数据？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.No) return;

            //设置进度条控件属性
            proBarValue = 0;
            proBarMaxValue = itemInfos.Count;
            string selectedFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                // 设置对话框属性
                folderBrowserDialog.Description = "请选择要操作的文件夹";
                folderBrowserDialog.SelectedPath = selectedFolderPath; // 设置初始文件夹路径
                folderBrowserDialog.ShowNewFolderButton = true; // 显示“新建文件夹”按钮

                // 显示对话框并等待用户选择文件夹
                DialogResult result = folderBrowserDialog.ShowDialog();

                // 检查用户是否点击了“确定”按钮
                if (result == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    selectedFolderPath = folderBrowserDialog.SelectedPath;
                }
            }
            ;

            if (obj == "DATA")
            {
                string dirpath = Path.Combine(selectedFolderPath, $"{itemInfos[0].ProductSN}_{itemInfos[0].ProjectName}");

                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

                Task.Run(async () =>
                {
                    ProBarVisily = Visibility.Visible;
                    await Task.Run(() =>
                    {


                        for (int i = 0; i < itemInfos.Count; i++)
                        {
                            DataTable table = new DataTable();
                            var itemdata = itemInfos[i];
                            var datatableresult =
                            DBOperate.Default.GetTestData(itemdata.Id, itemdata.CreateTime, itemdata.UpdateTime)
                            .ThenAnd(result => GetDataTableStructure(result.Data)
                            .AttachIfSucceed(result1 =>
                            {
                                table = result1.Data;
                            }).ConvertTo(result.Data));

                            if (!datatableresult)
                            {
                                ProBarVisily = Visibility.Hidden;
                                MessageBox.Show($"测试项【{itemdata.FlowName}】数据获取异常", "数据导出失败");
                                return;
                            }
                            var exresult = ExcelExport(itemdata, JsonToDataTableConverter(table, datatableresult.Data), dirpath);
                            if (!exresult)
                            {
                                ProBarVisily = Visibility.Hidden;
                                MessageBox.Show($"测试项【{itemdata.FlowName}】数据导出Excel文件失败", "数据导出失败");
                                return;
                            }
                            ProBarValue = i;
                        }

                        //ExcelExport(Datas[0].Data, new DataTable());.AttachIfSucceed(result =>
                        //{
                        //    MessageBox.Show($"数据导出完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //})
                        //.AttachIfFailed(result =>
                        //{
                        //    MessageBox.Show($"测试数据导出失败：{result.Message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        //});

                        //if (result)
                        //{ System.Windows.Forms.MessageBox.Show("数据导出完成"); }
                        ProBarVisily = Visibility.Hidden;

                        MessageBox.Show("数据导出完成");
                    });

                });
            }
            else
            {
                string dirpath = Path.Combine(selectedFolderPath, $"{itemInfos[0].ProductSN}_{itemInfos[0].ProjectName}报告");

                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

                Task.Run(async () =>
                {
                    ProBarVisily = Visibility.Visible;
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < itemInfos.Count; i++)
                        {
                            var itemdata = itemInfos[i];
                            var dataresult = DBOperate.Default.GetTestData(itemdata.Id, itemdata.CreateTime, itemdata.UpdateTime);
                            if (!dataresult)
                            {
                                ProBarVisily = Visibility.Hidden;
                                MessageBox.Show($"测试项【{itemdata.FlowName}】数据获取异常", "报告导出失败");
                                return;
                            }
                            var exresult = ExcelExport_Report(itemdata, dataresult.Data, dirpath);
                            if (!exresult)
                            {
                                ProBarVisily = Visibility.Hidden;
                                MessageBox.Show($"测试项【{itemdata.FlowName}】数据导出Excel报告文件失败", "报告导出失败");
                                return;
                            }
                            ProBarValue = i;
                        }
                        ProBarVisily = Visibility.Hidden;

                        MessageBox.Show("报告导出完成");
                    });
                });
            }
        }

        /// <summary>
        /// 创建测试数据表结构
        /// </summary>
        private OperateResult<DataTable> GetDataTableStructure(List<Test_DataInfo> value)
        {
            try
            {
                if (value.Count < 1)
                {
                    return OperateResult<DataTable>.Failed(null, "未检索到相关数据");
                }
                DataTable table = new DataTable();

                table.Columns.Add("ID");
                table.Columns.Add("记录时间");

                //table.Columns.Add("产品编号");
                //table.Columns.Add("试验项名称");
                table.Columns.Add("步骤序号");
                table.Columns.Add("步骤名称");
                // 遍历并添加列到DataTable  
                value[0].Pro_Data.ForEach(token => table.Columns.Add(token.Name));
                value[0].Pro_SetData.ForEach(token => table.Columns.Add(token.Name));

                // 遍历并添加列到DataTable  
                value[0].Euq_Data.ForEach(token => table.Columns.Add(token.Name));
                value[0].Euq_SetData.ForEach(token => table.Columns.Add(token.Name));

                if (value[0].EX_Data.Count > 1)
                {
                    value[0].EX_Data.ForEach(token => table.Columns.Add(token.Name));
                }


                table.Columns.Add("试验结果");
                table.Columns.Add("异常信息");

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
                //row["产品编号"] = x.ProductSN;
                //row["记录时间"] = x.RecordTime;
                row["记录时间"] = x.CreateTime;//.ToString("yyyy/MM/dd HH:mm:ss.fff");

                //row["试验项名称"] = x.FlowName;

                row["步骤序号"] = x.StepIndex;

                row["步骤名称"] = x.StepName;

                x.Pro_Data.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });
                x.Pro_SetData.ForEach(y => row[y.Name] = Math.Round(y.Value, 3));
                x.Euq_Data.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });
                x.Euq_SetData.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });

                if (x.EX_Data.Count > 1)
                {
                    x.EX_Data.ForEach(y => { row[y.Name] = Math.Round(y.Value, 3); });
                }


                row["试验结果"] = x.Result == ResultState.SUCCESS ? "OK" : "NG";
                row["异常信息"] = x.ErrorInfo;
                table.Rows.Add(row);
            });

            return table;
        }

        /// <summary>
        /// Excel格式文件导出
        /// </summary>
        /// <param name="item">试验条目</param>
        /// <param name="datas">测试数据</param>
        /// <param name="path">存储路径</param>
        private OperateResult ExcelExport(Test_ItemInfo item, DataTable datas, string path)
        {
            string errMsg = "";
            string fileName = "IMX.ATS.DIOS.Resource.ExcelTemp.IMX.ATS.Resource.Templet.xlsx";
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);

            using (stream)
            {
                try
                {
                    Workbook workbook = new Workbook(stream);

                    Worksheet worksheet = workbook.Worksheets[0];
                    #region Title
                    worksheet.Cells["B1"].Value = item.ProjectName;
                    worksheet.Cells["B3"].Value = item.ProductSN;
                    worksheet.Cells["E2"].Value = item.FlowName;
                    Aspose.Cells.Style resultstle = workbook.Styles[workbook.Styles.Add()];
                    resultstle.Font.Color = item.Result == ResultState.SUCCESS ? Color.Green : Color.Red;
                    worksheet.Cells["H2"].SetStyle(resultstle);
                    worksheet.Cells["H2"].Value = item.Result.ToString();


                    worksheet.Cells["I2"].PutValue(item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    worksheet.Cells["J2"].PutValue(item.UpdateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    worksheet.Cells["I4"].PutValue(item.ActualRunTime);
                    worksheet.Cells["J4"].PutValue(item.Operator);
                    worksheet.Cells["K2"].PutValue(item.ErrorInfo);
                    #endregion
                    Thread.Sleep(10);
                    #region 测试数据
                    ImportTableOptions tableOptions = new ImportTableOptions();
                    tableOptions.IsFieldNameShown = true;
                    worksheet.Cells.ImportData(datas, 6, 0, tableOptions);
                    #endregion
                    Thread.Sleep(10);
                    workbook.Save(Path.Combine(path, $"{item.FlowName}_{item.CreateTime:yyyyMMddHHmmss}.xlsx"));
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, "试验详细数据", "Excel格式文件导出", ex);
                    return OperateResult.Excepted(ex);
                }
            }
            ;

            return OperateResult.Succeed();
        }

        /// <summary>
        /// Excel格式文件导出
        /// </summary>
        /// <param name="item">试验条目</param>
        /// <param name="data">测试数据</param>
        /// <param name="path">存储路径</param>
        /// <returns></returns>
        private OperateResult ExcelExport_Report(Test_ItemInfo item, List<Test_DataInfo> data, string path)
        {
            string errMsg = "";
            string fileName = "IMX.ATS.DIOS.Resource.ExcelTemp.ATEReportTemp.xlsx";
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);

            using (stream)
            {
                try
                {
                    Workbook workbook = new Workbook(stream);

                    Worksheet worksheet = workbook.Worksheets[0];

                    #region Title
                    worksheet.Cells["A2"].Value = item.ProjectName;
                    worksheet.Cells["C2"].Value = item.ProductSN;
                    worksheet.Cells["F2"].Value = item.FlowName;
                    #region 测试结果样式
                    Aspose.Cells.Style resultstle = workbook.Styles[workbook.Styles.Add()];
                    resultstle.HorizontalAlignment = TextAlignmentType.Center;
                    resultstle.Font.Color = item.Result == ResultState.SUCCESS ? Color.Green : Color.Red;
                    worksheet.Cells["I2"].SetStyle(resultstle);
                    #endregion
                    worksheet.Cells["I2"].Value = item.Result == ResultState.SUCCESS ? "OK" : "NG";


                    worksheet.Cells["A5"].PutValue(item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    worksheet.Cells["C5"].PutValue(item.UpdateTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    worksheet.Cells["F5"].PutValue($"{Math.Round(new TimeSpan(item.ActualRunTime).TotalMinutes, 3)} 分钟");
                    worksheet.Cells["I5"].PutValue(item.Operator);
                    worksheet.Cells["K2"].PutValue(item.ErrorInfo);
                    #endregion

                    Thread.Sleep(10);
                    #region 测试数据

                    int count = 0;
                    int rowstratindex = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        Test_DataInfo testdata = data[i];
                        if (string.IsNullOrEmpty(testdata.StepName)) { continue; }

                        int rownum = 8 + rowstratindex;
                        int rowcount = 8 + rowstratindex;

                        for (int j = 0; j < testdata.Euq_DeviceRead.Count; j++)
                        {
                            var readdata = testdata.Euq_DeviceRead[j];
                            //int index = rownum + j;
                            worksheet.Cells.Merge(rowcount - 1, 4, 1, 3);
                            worksheet.Cells[$"E{rowcount}"].Value = readdata.DataInfo.Name;
                            worksheet.Cells.Merge(rowcount - 1, 7, 1, 2);
                            worksheet.Cells[$"H{rowcount}"].Value = readdata.DataInfo.Value;

                            worksheet.Cells.Merge(rowcount - 1, 9, 1, 2);
                            if (readdata.Limits_Upper == double.PositiveInfinity)
                            {
                                worksheet.Cells[$"J{rowcount}"].Value = "-";
                            }
                            else
                            {
                                worksheet.Cells[$"J{rowcount}"].Value = readdata.Limits_Upper;
                            }

                            worksheet.Cells.Merge(rowcount - 1, 11, 1, 2);
                            if (readdata.Limits_Lower == double.NegativeInfinity)
                            {
                                worksheet.Cells[$"L{rowcount}"].Value = "-";
                            }
                            else
                            {
                                worksheet.Cells[$"L{rowcount}"].Value = readdata.Limits_Lower;
                            }
                            

                            worksheet.Cells.Merge(rowcount - 1, 13, 1, 2);
                            worksheet.Cells[$"N{rowcount}"].Value = readdata.Judgment.GetDescription();
                            Aspose.Cells.Style dataresultstle = workbook.Styles[workbook.Styles.Add()];
                            dataresultstle.Font.Color = readdata.IsInRange ? Color.Green : Color.Red;
                            dataresultstle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.HorizontalAlignment = TextAlignmentType.Left;
                            dataresultstle.VerticalAlignment = TextAlignmentType.Center;
                            worksheet.Cells[$"P{rowcount}"].SetStyle(dataresultstle);
                            worksheet.Cells[$"P{rowcount}"].Value = readdata.IsInRange ? "PASS" : "FAIL";
                            rowcount++;
                        }

                        for (int j = 0; j < testdata.Pro_DeviceRead.Count; j++)
                        {
                            var readdata = testdata.Pro_DeviceRead[j];
                            //int index = rowcount + j;
                            worksheet.Cells[$"E{rowcount}"].Value = readdata.DataInfo.Name;
                            worksheet.Cells[$"H{rowcount}"].Value = readdata.DataInfo.Value;
                            worksheet.Cells[$"J{rowcount}"].Value = readdata.Limits_Upper;
                            worksheet.Cells[$"L{rowcount}"].Value = readdata.Limits_Lower;
                            worksheet.Cells[$"N{rowcount}"].Value = readdata.Judgment.GetDescription();
                            //判断结果
                            Style dataresultstle = workbook.Styles[workbook.Styles.Add()];
                            dataresultstle.Font.Color = readdata.IsInRange ? Color.Green : Color.Red;
                            dataresultstle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                            dataresultstle.HorizontalAlignment = TextAlignmentType.Left;
                            dataresultstle.VerticalAlignment = TextAlignmentType.Center;
                            worksheet.Cells[$"P{rowcount}"].SetStyle(dataresultstle);
                            worksheet.Cells[$"P{rowcount}"].Value = readdata.IsInRange ? "PASS" : "FAIL";
                            rowcount++;
                        }

                        count = testdata.Euq_DeviceRead.Count + testdata.Pro_DeviceRead.Count;
                        if (count < 1)
                        {
                            continue;
                        }
                        worksheet.Cells.Merge(rownum - 1, 0, count, 4);
                        worksheet.Cells[$"A{rownum}"].Value = testdata.StepName;
                        //选择区域
                        Range descRange = worksheet.Cells.CreateRange(rownum - 1, 0, count, 15);
                        //设置格式
                        Style descStyle = workbook.Styles[workbook.Styles.Add()];
                        descStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                        descStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                        descStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                        descStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                        descStyle.HorizontalAlignment = TextAlignmentType.Left;
                        descStyle.VerticalAlignment = TextAlignmentType.Center;
                        
                        descRange.ApplyStyle(descStyle, new StyleFlag { All = true });

                        rowstratindex += count;

                    }
                    #endregion
                    Thread.Sleep(10);
                    workbook.Save(Path.Combine(path, $"{item.FlowName}_{item.CreateTime:yyyyMMddHHmmss}.xlsx"));
                    Thread.Sleep(100);

                }
                catch (Exception ex)
                {
                    SuperDHHLoggerManager.Exception(LoggerType.FROMLOG, "试验报告数据", "Excel格式报告文件导出", ex);
                    return OperateResult.Excepted(ex);
                }

            }
            ;

            return OperateResult.Succeed();
        }
        #endregion
        /// <summary>
        /// 检索测试项
        /// </summary>
        private void SearchItem()
        {

            if (string.IsNullOrEmpty(FlowName) && ResultIndex == -1)
            {
                Datas.Clear();
                for (int i = 0; i < datasources.Count; i++)
                {
                    Datas.Add(new TestItemModel
                    {
                        IsSelect = false,
                        Data = datasources[i]
                    });
                }
            }
            else if (string.IsNullOrEmpty(FlowName))
            {
                DBOperate.Default.SelectTestItems(proid, (ResultState)Enum.Parse(typeof(ResultState), ResultIndex.ToString()))
                    .AttachIfSucceed(result =>
                    {
                        Datas.Clear();
                        for (int i = 0; i < result.Data.Count; i++)
                        {
                            Datas.Add(new TestItemModel
                            {
                                IsSelect = false,
                                Data = result.Data[i]
                            });
                        }
                    })
                    .AttachIfFailed(result =>
                    {
                        MessageBox.Show($"按试验结果获取数据失败：\r\n{result.Message}", "条件检索失败");
                    });
            }
            else
            {
                DBOperate.Default.SelectTestItems(proid, FlowName)
                        .AttachIfSucceed(result =>
                        {
                            Datas.Clear();
                            for (int i = 0; i < result.Data.Count; i++)
                            {
                                Datas.Add(new TestItemModel
                                {
                                    IsSelect = false,
                                    Data = result.Data[i]
                                });
                            }
                        })
                        .AttachIfFailed(result =>
                        {
                            MessageBox.Show($"按测试项名称查询获取数据失败：\r\n{result.Message}", "条件检索失败");
                        });
            }
        }

        /// <summary>
        /// 打开数据展示界面窗口
        /// </summary>
        private void DatasWindowOpen(object obj)
        {

            if (SelectIndex == -1)
            {
                return;
            }

            int index = SelectIndex;

            Test_ItemInfo item = Datas[index].Data;

            var result = DBOperate.Default.GetTestDataCount(item.Id, item.CreateTime, item.UpdateTime.AddSeconds(1));

            if (!result)
            {
                MessageBox.Show(result.Message, "实验数据查询异常");
                return;
            }

            var model = new TestDatasViewModel($"{item.ProjectName}-{item.FlowName}", item.Id, item.CreateTime, item.UpdateTime, result.Data);

            var view = new TestDatasView
            {
                DataContext = model,
            };
            Window win = System.Windows.Application.Current.MainWindow;
            WindowInteropHelper itemview = new WindowInteropHelper(win);
            WindowInteropHelper dataview = new WindowInteropHelper(view);
            itemview.Owner = IntPtr.Zero;
            dataview.Owner = itemview.Handle;
            view.Show();
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (proid == GlobalModel.ProjectItemId)
            {
                return;
            }

            proid = GlobalModel.ProjectItemId;

            Datas.Clear();
            datasources.Clear();
            DBOperate.Default.SelectTestItems(proid)
                .AttachIfSucceed(result =>
                {
                    for (int i = 0; i < result.Data.Count; i++)
                    {
                        TestItemModel testinfomodel = new TestItemModel
                        {
                            IsSelect = false,
                            Data = result.Data[i],
                        };
                        Datas.Add(testinfomodel);
                        datasources.Add(result.Data[i]);
                    }
                })
                .AttachIfFailed(result =>
                {
                    MessageBox.Show(result.Message, "试验条目数据加载失败");
                });
            //base.WindowLoadedExecute(obj);
        }

        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public TestItemViewModel() { }
        #endregion
    }

    /// <summary>
    /// 测试项页面展示类
    /// </summary>
    public class TestItemModel : ViewModelBase
    {
        private bool isselect = false;
        /// <summary>
        /// 当前选中状态
        /// </summary>
        public bool IsSelect
        {
            get => isselect;
            set => Set(nameof(IsSelect), ref isselect, value);
        }


        private Test_ItemInfo data;
        /// <summary>
        /// 测试项数据列表
        /// </summary>
        public Test_ItemInfo Data
        {
            get => data;
            set => Set(nameof(Data), ref data, value);
        }
    }
}
