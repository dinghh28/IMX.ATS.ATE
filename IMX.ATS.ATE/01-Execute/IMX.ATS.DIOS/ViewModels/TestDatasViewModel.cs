#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2025   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LAPTOP-9Q9TTD5V
 * 公司名称：
 * 命名空间：IMX.ATS.DIOS.ViewModels
 * 唯一标识：694ad6b7-d555-490c-a8eb-1badc22b68d4
 * 文件名：TestDatasViewModel
 * 当前用户域：LAPTOP-9Q9TTD5V
 * 
 * 创建者：58274
 * 创建时间：2025/6/13 17:31:43
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

using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using H.WPF.Framework;
using IMX.Common;
using IMX.DB;
using IMX.DB.Model;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMX.ATS.DIOS.ViewModels
{
    /// <summary>
    /// 试验数据展示窗口类
    /// </summary>
    public class TestDatasViewModel : WindowViewModelBaseEx
    {
        #region 公共属性

        #region 界面绑定属性
        private string title = "未知项目";
        /// <summary>
        /// 数据来源标题
        /// </summary>
        public string Title
        {
            get => title;
            set => Set(nameof(Title), ref title, value);
        }

        private DataTable testDataTable = new DataTable();
        /// <summary>
        /// 数据内容
        /// </summary>
        public DataTable TestDataTable
        {
            get => testDataTable;
            set => Set(nameof(TestDataTable), ref testDataTable, value);
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

        private int selectpage = 1;

        /// <summary>
        /// 测试数据当前页
        /// </summary>
        public int SelectPage
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
        /// 翻页指令
        /// </summary>
        public RelayCommand<object> SelectNextDataCommand => new RelayCommand<object>(GetTestDataLimit);
        #endregion

        /// <summary>
        /// 条目ID
        /// </summary>
        public long ItemId { get; set; } = -1;
        #endregion

        #region 私有变量
        private long id = -1;

        private DateTime start;

        private DateTime end;

        private const int OnePageCount = 50;

        /// <summary>
        /// 测试数据表结构
        /// </summary>
        private DataTable datastructure = new DataTable();
        #endregion

        #region 私有方法
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
            DBOperate.Default.GetLimitTestData(id, start, end, (int)pagenum, OnePageCount).ThenAnd(result =>
            {
                if (datastructure.Columns.Count < 1) 
                {
                    var strresult = GetDataTableStructure(result.Data);
                    if (!strresult)
                    {
                        return OperateResult<List<Test_DataInfo>>.Failed(null, strresult.Message);
                    }
                    datastructure = strresult.Data;
                }
                TestDataTable.Clear();
                TestDataTable = JsonToDataTableConverter(datastructure, result.Data);
                ItemCount = result.Data.Count;
                return OperateResult<List<Test_DataInfo>>.Succeed(result.Data);
            }).AttachIfFailed(result => 
            {
                MessageBox.Show(result.Message, "试验数据获取异常");
            }).AttachIfSucceed(result => 
            {
                SelectPage = (int)pagenum;
            });
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

                table.Columns.Add("产品编号");
                table.Columns.Add("试验项名称");
                table.Columns.Add("步骤序号");
                table.Columns.Add("步骤名称");
                // 遍历并添加列到DataTable  
                value[0].Pro_Data.ForEach(token => table.Columns.Add(token.Name));
                value[0].Pro_SetData.ForEach(token => table.Columns.Add(token.Name));

                // 遍历并添加列到DataTable  
                value[0].Euq_Data.ForEach(token => table.Columns.Add(token.Name));
                value[0].Euq_SetData.ForEach(token => table.Columns.Add(token.Name));

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
                row["产品编号"] = x.ProductSN;
                //row["记录时间"] = x.RecordTime;
                row["记录时间"] = x.CreateTime;//.ToString("yyyy/MM/dd HH:mm:ss.fff");

                row["试验项名称"] = x.FlowName;

                row["步骤序号"] = x.StepIndex;

                row["步骤名称"] = x.StepName;

                x.Pro_Data.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });
                x.Pro_SetData.ForEach(y=> row[y.Name] = Math.Round(y.Value, 3));
                x.Euq_Data.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });
                x.Euq_SetData.ForEach(y =>
                {
                    row[y.Name] = Math.Round(y.Value, 3);
                });

                row["试验结果"] = x.Result == ResultState.SUCCESS ? "OK" : "NG";
                row["异常信息"] = x.ErrorInfo;
                table.Rows.Add(row);
            });

            return table;
        }
        #endregion

        #region 保护方法
        protected override void WindowLoadedExecute(object obj)
        {
            if (ItemId <1)
            {
                return;
            }


            PageCount = ((DataCount % OnePageCount) == 0) ? (DataCount / OnePageCount) : ((DataCount / OnePageCount) + 1);
            GetTestDataLimit("第一页");
            //base.WindowLoadedExecute(obj);
        }
         
        protected override void WindowClosedExecute(object obj)
        {
            base.WindowClosedExecute(obj);
        }
        #endregion


        #region 构造方法
        public TestDatasViewModel() { }

        public TestDatasViewModel(string wintitle, long itemid, DateTime strattim, DateTime stoptime, long datascount)
        {
            Title = wintitle;
            id = itemid;
            start = strattim;
            end = stoptime;
            DataCount = datascount;
        }
        #endregion

    }
}
