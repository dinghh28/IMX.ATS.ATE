using H.WPF.Framework;
using IMX.DB.Model;
using IMX.Function.Base;
using IMX.Function.ViewModel;
using IMX.Function;
using IMX.Logger;
using Piggy.VehicleBus.Common;
using Piggy.VehicleBus.MessageProcess;
using Super.Zoo.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace IMX.ATS.ATEConfig.Function
{
    /// <summary>
    /// 产品指令配置模板
    /// </summary>
    public class FunViewModelProduct : FunViewModel
    {
        #region 继承属性
        //public override TestFunction Func{ get; set; } = new TestFunction(FuncitonType.PRODUCTFUNC);

        private TestFunction func = TestFunction.Create(FuncitonType.Product);

        public override TestFunction Func
        {
            get => func;
            set
            {
                if (func?.Config != null)
                {
                    func = value;


                    FunConfig_Product config = (value.Config as FunConfig_Product);

                    if ((Func?.Config as FunConfig_Product)?.SendSignals?.Count > 0)
                    {
                        sendSignals.Clear();
                        (Func.Config as FunConfig_Product).SendSignals.ForEach(x =>
                        {
                            sendSignals.Add(new SendSignalModel
                            {
                                IsSelected = x.IsSend,
                                DBCSignal = x
                            });
                        });
                    }
                }
            }
        }

        public override FuncitonType SupportFuncitonType => FuncitonType.Product;

        public override string SupportFuncitonString => "Product";

        //public override List<string> DeviceAddress { get; set; } = new List<string> { "Product_0", "Product_1", "Product_2" };
        #endregion

        #region 界面绑定

        /// <summary>
        /// 设备开关操作
        /// </summary>
        //public virtual List<FOutPutStateType> OperateTypeList { get; set; } = new List<FOutPutStateType> { FOutPutStateType.ON, FOutPutStateType.OFF, FOutPutStateType.Null };

        //#region 属性

        //private FOutPutStateType operateType;

        //public FOutPutStateType OperateType
        //{
        //    get => operateType;
        //    set => Set(nameof(OperateType), ref operateType, value);
        //}


        private ObservableCollection<SendSignalModel> sendSignals = new ObservableCollection<SendSignalModel>();
        /// <summary>
        /// 下发信号
        /// </summary>
        public ObservableCollection<SendSignalModel> SendSignals
        {
            get
            {
                return sendSignals;
            }
            set => Set(nameof(SendSignals), ref sendSignals, value);
        }

        private bool isSelectAll = false;

        public bool IsSelectAll
        {
            get => isSelectAll;
            set
            {
                if (Set(nameof(IsSelectAll), ref isSelectAll, value))
                {
                    SendSignals.ToList().FindAll(model => model.IsSelected = value);
                }
            }
        }

        #endregion

        #region （Command）


        #endregion

        #region 私有方法


        #endregion

        #region 构造函数

        public FunViewModelProduct()
        {
            //deviceNameIndex = DeviceAddress[0];

            if ((Func?.Config as FunConfig_Product)?.SendSignals?.Count > 0)
            {
                var signals = (Func.Config as FunConfig_Product).SendSignals;
                SendSignals.Clear();
                for (int i = 0; i < signals.Count; i++)
                {
                    SendSignals[i].DBCSignal = signals[i];
                    SendSignals[i].IsSelected = signals[i].IsSend;
                }
                return;
            }
            try
            {
                List<DBCSendSignal> dBCSignalModel = new List<DBCSendSignal>();

#if DEBUG //后续从数据库中调入DBC文件内容

                string DBCFileName = "BEV_E0X_OT_Car RMCU V3.72 Draft_202311220825";
                string path = Path.Combine(@"C:\Users\Administrator\Desktop", DBCFileName + ".dbc");

                OperateResult<IMessageFileLoader> rltCreate = MessageFileLoader.Create(Path.GetExtension(path), SuperDHHLoggerManager.DeviceLogger);
                if (!rltCreate)
                {
                    MessageBox.Show($"DBC文件加载失败:{rltCreate.Message}", "DBC文件解析异常");
                    return;
                }

                IMessageFileLoader messageFileLoader = rltCreate.Data;

                var rltLoad = rltCreate.Data.Paser(path);
                if (!rltLoad)
                {
                    MessageBox.Show($"DBC解析失败:{rltLoad.Message}", "DBC文件解析异常");
                    return;
                }

                messageFileLoader.MessageDic.ToList().ForEach(m =>
                {
                    m.Value.Signals.ForEach(sig =>
                    {
                        dBCSignalModel.Add(new DBCSendSignal
                        {
                            MessageID = m.Key,
                            MessageName = m.Value.Name,
                            SignalName = sig.Name,
                            SignalValue = sig.InitValue.ToString(),
                        });
                    });

                });

                //从数据库中获取到已配置好的DBC发送模型
                Test_DBCConfig test_DBCConfig = new Test_DBCConfig();
                test_DBCConfig.Test_DBCSendSignals = new List<Test_DBCInfo>
                {
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_CrashOutputSts" },
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_Resd1"},
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_RollgCntr1"}
                };

#else
                //从数据库中获取到已配置好的DBC发送模型
                Test_DBCConfig test_DBCConfig = new Test_DBCConfig();
                test_DBCConfig.Test_DBCSendSignals = new List<Test_DBCInfo>
                {
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_CrashOutputSts" },
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_Resd1"},
                    new Test_DBCInfo{ Message_ID = 0x21, Signal_Name = "ACU_3_RollgCntr1"}
                };

#endif

                SendSignals.Clear();
                foreach (var item in test_DBCConfig.Test_DBCSendSignals)
                {
                    SendSignals.Add(new SendSignalModel
                    {
                        IsSelected = false,
                        DBCSignal = new DBCSendSignal
                        {
                            CustomName = item.Custom_Name,
                            MessageName = dBCSignalModel.Find(x => x.SignalName == item.Signal_Name).MessageName,
                            MessageID = item.Message_ID,
                            SignalName = item.Signal_Name,
                            SignalValue = dBCSignalModel.Find(x => x.SignalName == item.Signal_Name).SignalValue,
                        }
                    });
                }
                for (Int32 i = 0; i < SendSignals.Count; i++)
                {
                    (Func.Config as FunConfig_Product).SendSignals.Add(SendSignals[i].DBCSignal);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }

    public class SendSignalModel : ExtendViewModelBase
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;// = DBCSignal.IsSend;
            set
            {
                if (Set(nameof(IsSelected), ref isSelected, value))
                {
                    DBCSignal.IsSend = value;
                }
            }
        }

        private DBCSendSignal dbcsignal = new DBCSendSignal();

        public DBCSendSignal DBCSignal
        {
            get { return dbcsignal; }
            set { dbcsignal = value; }
        }
    }
}
