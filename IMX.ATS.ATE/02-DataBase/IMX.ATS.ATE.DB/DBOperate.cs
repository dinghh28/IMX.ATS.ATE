using FreeSql;
using IMX.DB.Model;
using IMX.Logger;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static FreeSql.Internal.GlobalFilter;

namespace IMX.DB
{
    public class DBOperate : SingletonObject<DBOperate>, ILogRecord, IDisposableObject, IDisposable
    {
        #region 公共属性
        public Guid Identify { get; } = Guid.NewGuid();

        /// <summary>
        /// 数据库初始化状态
        /// </summary>
        public bool IsInitOK { get; private set; } = false;

        public IFreeSql Sqlite => sqliteLazy?.Value;

        public ILogger Logger { get; }

        public bool OutterLogger { get; }

        public string LastError { get; set; }

        public bool Disposed { get; private set; } = false;
        #endregion

        #region 私有变量
        /// <summary>
        /// 数据库连接单例
        /// </summary>
        private Lazy<IFreeSql> sqliteLazy = null;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly string ConnectionString = @"Data Source=Data\data.db;Version=3;journal_mode=WAL;locking_mode=NORMAL;Pooling=True;Max Pool Size=100;";

        /// <summary>
        /// 从库连接字符
        /// </summary>
        private readonly string SlaveConnectionString = @"Data Source=Data\slavedata.db;Version=3;journal_mode=WAL;Pooling=True;Max Pool Size=100;";

        /// <summary>
        /// 数据库类型
        /// </summary>
        private readonly DataType DataType = DataType.Sqlite;

        //private List<Test_DataInfo> lisdatainfo = new List<Test_DataInfo>();
        #endregion

        #region 公共方法

        /// <summary>
        /// 数据库初始化
        /// </summary>
        /// <returns></returns>
        public OperateResult Init()
        {
            if (IsInitOK)
            {
                LastError = $"数据库已初始化";
                Logger.Error(nameof(DBOperate), nameof(Init), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                if (ConnectionString == null)
                {
                    LastError = $"未设置数据库连接字符";
                    Logger.Error(nameof(DBOperate), nameof(Init), LastError);
                    return OperateResult.Failed(LastError);
                }
                //#if DEBUG
                sqliteLazy = new Lazy<IFreeSql>(() => new FreeSqlBuilder()
                .UseMonitorCommand(cmd => Trace.WriteLine($"Sql：{cmd.CommandText}"))//监听SQL语句,Trace在输出选项卡中查看
                .UseConnectionString(DataType, ConnectionString)//DataType
                //.UseSlave(SlaveConnectionString)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                //.UseAdoConnectionPool(true)
                .Build());
                //#else
                //                           sqliteLazy = new Lazy<IFreeSql>(() => new FreeSqlBuilder()
                //                          .UseConnectionString(DataType, ConnectionString)
                //                          .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                //                          //.UseAdoConnectionPool(true)
                //                          //.UseSlave(SlaveConnectionString)
                //                          .Build());
                //#endif
                try
                {
                    Sqlite.UseJsonMap();
                    BaseEntity.Initialization(Sqlite, null);
                    //var tableName = Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo))
                    //.AsTableImpl
                    //.GetTableNameByColumnValue(DateTime.Now.Date, autoExpand: true);

                    ////创建数据库表
                    //if (Sqlite.DbFirst.ExistsTable(tableName) == false)
                    //    Sqlite.CodeFirst.SyncStructure(typeof(Test_ItemInfo), tableName);

                    IsInitOK = true;
                    return OperateResult.Succeed();
                }
                catch (Exception ex)
                {
                    IsInitOK = false;

                    LastError = ex.GetMessage();
                    Logger.Error(nameof(DBOperate), nameof(Init), LastError);
                    return OperateResult.Excepted(ex);
                }
            }
            catch (Exception ex)
            {
                IsInitOK = false;
                sqliteLazy = null;
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(Init), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        #region 用户信息操作
        /// <summary>
        /// 用户登录信息确认
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="psw">用户密码</param>
        /// <returns></returns>
        public OperateResult<UserInfo> Login(string username, string psw)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(Login), LastError);
                return OperateResult<UserInfo>.Failed(null, LastError);
            }
            try
            {
                UserInfo item = Sqlite.Select<UserInfo>().Where(x => x.UserName == username && x.Password == psw && x.IsDeleted == false).ToOne();

                if (item == null)
                {
                    LastError = $"用户名不存在或者密码错误";
                    Logger.Error(nameof(DBOperate), nameof(Login), LastError);
                    return OperateResult<UserInfo>.Failed(null, LastError);
                }

                return OperateResult<UserInfo>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(Login), LastError);
                return OperateResult<UserInfo>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="userInfo">用户相关信息</param>
        /// <returns></returns>
        public OperateResult AddNewUser(UserInfo userInfo)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                if (userInfo == null)
                {
                    LastError = $"用户信息为空，请填写用户名和密码！";
                    Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                    return OperateResult.Failed(LastError);
                }

                long exrow = Sqlite.Select<UserInfo>().Where(x => x.UserName == userInfo.UserName && x.IsDeleted == false).Count();
                if (exrow > 0)
                {
                    LastError = $"该用户名已存在，请重新填写用户名！";
                    Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                    return OperateResult.Failed(LastError);
                }
                userInfo.Insert();

                //int row = Sqlite.Insert(userInfo).ExecuteAffrows();

                //if (row < 1)
                //{
                //    LastError = $"用户信息未实际发生存储";
                //    Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 用户权限更新
        /// </summary>
        /// <param name="id">用户信息列表ID</param>
        /// <param name="privilege">变更后权限</param>
        /// <returns></returns>
        public OperateResult UpdateUserPrivilege(int id, int privilege)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPrivilege), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                int row = Sqlite.Update<UserInfo>(id)
                    .Set(x => x.Privilege, privilege)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"用户权限未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateUserPrivilege), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPrivilege), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 用户密码变更
        /// </summary>
        /// <param name="id">用户信息列表ID</param>
        /// <param name="pws">变更后密码</param>
        /// <returns></returns>
        public OperateResult UpdateUserPassword(int id, string pws_old, string pws_new)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                UserInfo item = Sqlite.Select<UserInfo>().Where(x => x.Id == id && x.Password == pws_old).ToOne();
                if (item == null)
                {
                    LastError = $"用户原密码输入错误";
                    Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                    return OperateResult.Failed(LastError);
                }

                int row = Sqlite.Update<UserInfo>(id).Set(x => x.Password, pws_new).Set(x => x.UpdateTime, DateTime.Now).ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"用户密码未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns></returns>
        public OperateResult<List<UserInfo>> GetAllusers()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetAllusers), LastError);
                return OperateResult<List<UserInfo>>.Failed(null, LastError);
            }
            try
            {
                List<UserInfo> user = Sqlite.Select<UserInfo>().Where(x => x.IsDeleted != true).ToList();
                return OperateResult<List<UserInfo>>.Succeed(user);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetAllusers), LastError);
                return OperateResult<List<UserInfo>>.Excepted(null, ex);
            }
        }


        public OperateResult DeleteUserinfo(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(DeleteUserinfo), LastError);
                return OperateResult<List<UserInfo>>.Failed(null, LastError);
            }
            try
            {
                int row = Sqlite.Update<UserInfo>(id).Set(x => x.IsDeleted, true).ExecuteAffrows();
                if (row < 1)
                {
                    LastError = $"用户删除失败";
                    Logger.Error(nameof(DBOperate), nameof(DeleteUserinfo), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(DeleteUserinfo), LastError);
                return OperateResult<List<UserInfo>>.Excepted(null, ex);
            }
        }
        #endregion

        #region 项目信息操作
        /// <summary>
        /// 获取所有项目信息
        /// </summary>
        /// <returns>项目信息列表</returns>
        public OperateResult<List<Test_ProjectInfo>> SelectedProjectInfo_All()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectedProjectInfo_All), LastError);
                return OperateResult<List<Test_ProjectInfo>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_ProjectInfo>().ToList();
                return OperateResult<List<Test_ProjectInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectedProjectInfo_All), LastError);
                return OperateResult<List<Test_ProjectInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 更新项目信息
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        public OperateResult UpdataProjectInfo(Test_ProjectInfo projectInfo)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                projectInfo.Update();
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 添加项目信息
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        public OperateResult InsertProjectInfo(Test_ProjectInfo projectInfo)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                int count = Test_ProjectInfo.Where(a => a.ProjectSN == projectInfo.ProjectSN).ToList().Count;
                if (count > 0)
                {
                    LastError = $"项目编号已存在，请重新输出项目编号！";
                    Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                    return OperateResult.Failed(LastError);
                }
                projectInfo.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateUserPassword), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 获取所有项目名称
        /// </summary>
        /// <returns></returns>
        public OperateResult<List<string>> GetProjectNames()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_ProjectInfo>().Distinct().ToList(x => x.ProjectName);
                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取项目详细信息(通过项目名称)
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <returns></returns>
        public OperateResult<Test_ProjectInfo> GetProjectInfo_ByName(string name)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<Test_ProjectInfo>.Failed(null, LastError);
            }

            try
            {
                var items = Test_ProjectInfo.Where(x => x.ProjectName == name).ToOne();

                return OperateResult<Test_ProjectInfo>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<Test_ProjectInfo>.Excepted(null, ex);
            }
        }


        #endregion

        #region DBC文件操作
        public OperateResult InsertDBCFile(Test_DBCFileInfo fileInfo)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertDBCFile), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                if (fileInfo == null)
                {
                    LastError = $"DBC文件信息不存在";
                    Logger.Error(nameof(DBOperate), nameof(InsertDBCFile), LastError);
                    return OperateResult.Failed(LastError);
                }

                fileInfo.Insert();
                //int row = Sqlite.Insert(fileInfo).ExecuteAffrows();

                //if (row < 1)
                //{
                //    LastError = $"DBC文件未实际发生存储";
                //    Logger.Error(nameof(DBOperate), nameof(AddNewUser), LastError);
                //    return OperateResult.Failed(LastError);
                //}

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertDBCFile), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        public OperateResult<List<Test_DBCFileInfo>> GetFiles()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetFiles), LastError);
                return OperateResult<List<Test_DBCFileInfo>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_DBCFileInfo>().Where(x => x.IsDeleted != true).ToList();

                return OperateResult<List<Test_DBCFileInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetFiles), LastError);
                return OperateResult<List<Test_DBCFileInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取DBC文件信息（通过DBC文件库ID）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult<Test_DBCFileInfo> GetFile_ByID(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetFiles), LastError);
                return OperateResult<Test_DBCFileInfo>.Failed(null, LastError);
            }
            try
            {
                var items = Test_DBCFileInfo.Find(id);

                return OperateResult<Test_DBCFileInfo>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetFiles), LastError);
                return OperateResult<Test_DBCFileInfo>.Excepted(null, ex);
            }
        }

        public OperateResult DeleteDBCFile(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(DeleteDBCFile), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                //var item = Test_DBCFileInfo.Find(id);

                //if (item == null)
                //{
                //    LastError = $"DBC文件不存在";
                //    Logger.Error(nameof(DBOperate), nameof(DeleteDBCFile), LastError);
                //    return OperateResult.Failed(LastError);
                //}
                //else
                //{
                int row = Sqlite.Update<Test_DBCFileInfo>(id)
                    .Set(x => x.IsDeleted, true)
                    .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"DBC文件未实际发生删除";
                    Logger.Error(nameof(DBOperate), nameof(DeleteDBCFile), LastError);
                    return OperateResult.Failed(LastError);
                }
                //item.DBCFileID = fileid;
                //item.UpdateOperator = username;
                //item.Update();
                //}

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(DeleteDBCFile), LastError);
                return OperateResult.Excepted(ex);
            }
        }
        #endregion

        #region DBC配置信息操作
        /// <summary>
        /// 更新DBC配置所关联的DBC文件
        /// </summary>
        /// <param name="configid">DBC配置id</param>
        /// <param name="fileid">DBC文件ID</param>
        /// <returns></returns>
        public OperateResult UpdateDBCFile(int configid, int fileid, string username)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCFile), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var item = Test_DBCConfig.Find(configid);

                if (item == null)
                {
                    LastError = $"DBC配置不存在";
                    Logger.Error(nameof(DBOperate), nameof(UpdateDBCFile), LastError);
                    return OperateResult.Failed(LastError);
                }
                else
                {
                    int row = Sqlite.Update<Test_DBCConfig>(configid)
                        .Set(x => x.DBCFileID, fileid)
                        .Set(x => x.UpdateOperator, username)
                        .Set(x => x.UpdateTime, DateTime.Now)
                        .ExecuteAffrows();

                    if (row < 1)
                    {
                        LastError = $"DBC文件未实际发生变更";
                        Logger.Error(nameof(DBOperate), nameof(UpdateDBCFile), LastError);
                        return OperateResult.Failed(LastError);
                    }
                    //item.DBCFileID = fileid;
                    //item.UpdateOperator = username;
                    //item.Update();
                }



                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCFile), LastError);
                return OperateResult.Excepted(ex);
            }
        }



        /// <summary>
        /// 保存DBC配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public OperateResult SaveDBCConfig(Test_DBCConfig config)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SaveDBCConfig), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                config.Save();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SaveDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新DBC配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public OperateResult UpdateDBCConfig(Test_DBCConfig config)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SaveDBCConfig), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                config.Update();

                //var repo = Sqlite.GetRepository<Test_DBCConfig>(); //可以从 IOC 容器中获取
                //var item = repo.Where(a => a.Id == config.Id).First();  //此时快照 item
                //item.UpdateTime = DateTime.Now;
                //item.Test_DBCReceiveSignals = config.Test_DBCReceiveSignals;
                //item.Test_DBCSendSignals = config.Test_DBCSendSignals;
                //repo.Update(item);

                // config.UpdateTime = DateTime.Now;
                // Sqlite.Update<Test_DBCConfig>(config.Id).Set(x=>x.Test_DBCReceiveSignals, config.Test_DBCReceiveSignals);
                //bool reslut= config.Update();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SaveDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }
        }
        /// <summary>
        /// 插入DBC配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public OperateResult InsertDBCConfig(Test_DBCConfig config)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertDBCConfig), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                config.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 获取DBC配置信息(通过项目ID)
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<Test_DBCConfig> GetDBCConfig_ByProjectID(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByProjectID), LastError);
                return OperateResult<Test_DBCConfig>.Failed(null, LastError);
            }

            try
            {
                var items = Sqlite.Select<Test_DBCConfig>().Where(x => x.ProjectID == id).ToOne();
                if (items == null)
                {
                    return OperateResult<Test_DBCConfig>.Succeed(new Test_DBCConfig());
                }
                return OperateResult<Test_DBCConfig>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByProjectID), LastError);
                return OperateResult<Test_DBCConfig>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取DBC上报信息(通过项目ID)
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<Test_DBCInfo>> GetDBCReceiveSignals(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetDBCReceiveSignals), LastError);
                return OperateResult<List<Test_DBCInfo>>.Failed(null, LastError);
            }

            try
            {

                var items = Sqlite.Select<Test_DBCConfig>().Where(x => x.ProjectID == id).ToOne(x => x.Test_DBCReceiveSignals);

                return OperateResult<List<Test_DBCInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetDBCReceiveSignals), LastError);
                return OperateResult<List<Test_DBCInfo>>.Excepted(null, ex);
            }
        }
        #endregion

        #region 项目流程操作

        /// <summary>
        /// 获取试验阶段名字
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<Test_Programme> GetProgrammeName(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<Test_Programme>.Failed(null, LastError);
            }
            try
            {
                Test_Programme items = Sqlite.Select<Test_Programme>().Where(x => x.ProjectID == id).ToOne();

                return OperateResult<Test_Programme>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<Test_Programme>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验阶段名字
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetOffProgrammeName(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Programme>().Where(x => x.ProjectID == id).ToOne(x => x.TestOff_FlowNames);

                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 更新当前项目试验阶段和下电时序
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="funcname">流程名称</param>
        /// <param name="processes">试验步骤</param>
        /// <returns></returns>
        public OperateResult UpdateProgram(int id, List<string> testprocess, List<string> testpoweroff)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                int row = Sqlite.Update<Test_Programme>()
                            .Where(x => x.ProjectID == id)
                            .Set(X => X.Test_FlowNames, testprocess)
                            .Set(X => X.TestOff_FlowNames, testpoweroff)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"试验方案未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="funcname">流程名称</param>
        /// <param name="processes">试验步骤</param>
        /// <returns></returns>
        public OperateResult<Test_Programme> ExistProgram(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(ExistProgram), LastError);
                return OperateResult<Test_Programme>.Failed(null, LastError);
            }

            try
            {
                Test_Programme items = Sqlite.Select<Test_Programme>().Where(x => x.ProjectID == id).ToOne();

                return OperateResult<Test_Programme>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                return OperateResult<Test_Programme>.Excepted(null, ex);
            }
        }


        /// <summary>
        /// 插入试验流程
        /// </summary>
        /// <param name="function">试验流程</param>
        /// <returns></returns>
        public OperateResult InsertTestProgram(Test_Programme function)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProgram), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                function.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProgram), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 插入试验流程
        /// </summary>
        /// <param name="function">试验流程</param>
        /// <returns></returns>
        public OperateResult InsertTestProccess(Test_Process function)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                function.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 获取流程名字
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetProcessName(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Process>().Where(x => x.ProjectID == id && x.IsDeleted == false).ToList(x => x.FunctionName);

                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取流程描述
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetProcessDescription(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Process>().Where(x => x.ProjectID == id && x.IsDeleted == false).ToList(x => x.Description);

                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取流程名称和描述
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult<Dictionary<string, string>> GetProcessNaemAndDescription(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<Dictionary<string, string>>.Failed(null, LastError);
            }

            try
            {
                var items = Sqlite.Select<Test_Process>().Where(x => x.ProjectID == id).ToDictionary(x => x.FunctionName, x => x.Description);

                return OperateResult<Dictionary<string, string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult<Dictionary<string, string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取流程步骤
        /// </summary>
        /// <param name="funcname">流程名称</param>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<ModTestProcess>> GetFlowsByNameID(string funcname, int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<List<ModTestProcess>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Process>().Where(x => x.ProjectID == id && x.FunctionName == funcname).ToOne(x => x.Test_Flows) ?? new List<ModTestProcess>();

                return OperateResult<List<ModTestProcess>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<List<ModTestProcess>>.Excepted(null, ex);
            }
        }

        public OperateResult<Dictionary<string, List<ModTestProcess>>> GetFlowsByNameID(int id, List<string> funcnames)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, List<ModTestProcess>>>.Failed(null, LastError);
            }

            if (funcnames == null)
            {
                LastError = $"流程方法名称不可为空";
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, List<ModTestProcess>>>.Failed(null, LastError);
            }

            if (funcnames.Count < 1)
            {
                LastError = $"未获取需检索流程方法名称";
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, List<ModTestProcess>>>.Failed(null, LastError);
            }

            try
            {
                string sqlStr =
                $"SELECT\n" +
                $"    *\n" +
                $"FROM\n" +
                $"    Test_Process\n" +
                $"WHERE\n" +
                $"   ProjectID = {id}\n" +
                $"AND\n";

                for (int i = 0; i < funcnames.Count; i++)
                {
                    if (i == funcnames.Count - 1)
                    {
                        sqlStr += $"    FunctionName = '{funcnames[i]}'";
                        break;
                    }
                    sqlStr += $"    FunctionName = '{funcnames[i]}'\n" + "OR\n";
                }

                var list = Sqlite.Select<Test_Process>().WithSql(sqlStr).ToList();

                if (list.Count < 1)
                {
                    LastError = $"相关流程不存在，请确认完成相关配置";
                    Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                    return OperateResult<Dictionary<string, List<ModTestProcess>>>.Failed(null, LastError);
                }

                Dictionary<string, List<ModTestProcess>> data = new Dictionary<string, List<ModTestProcess>>();

                for (int i = 0; i < list.Count; i++)
                {
                    data.Add(list[i].FunctionName, list[i].Test_Flows);
                }

                return OperateResult<Dictionary<string, List<ModTestProcess>>>.Succeed(data);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, List<ModTestProcess>>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 更新当前流程试验步骤
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="funcname">流程名称</param>
        /// <param name="processes">试验步骤</param>
        /// <returns></returns>
        public OperateResult UpdateProcess(int id, string funcname, List<ModTestProcess> processes)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {

                int row = Sqlite.Update<Test_Process>()
                            .Where(x => x.ProjectID == id && x.FunctionName == funcname)
                            .Set(X => X.Test_Flows, processes)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"测试流程 【{funcname}】 未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 删除当前流程试验步骤
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult DeleteProcess(int id, string funcname)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(DeleteProcess), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                int row = Sqlite.Update<Test_Process>()
                            .Where(x => x.ProjectID == id && x.FunctionName == funcname)
                            .Set(x => x.IsDeleted, true)
                            .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"测试流程【{funcname}】未实际删除";
                    Logger.Error(nameof(DBOperate), nameof(DeleteProcess), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(DeleteProcess), LastError);
                return OperateResult.Excepted(ex);
            }
        }
        #endregion

        #region 项目方案操作
        /// <summary>
        /// 插入试验方案
        /// </summary>
        /// <param name="function">试验方案</param>
        /// <returns></returns>
        public OperateResult InsertTestProgramme(Test_Programme function)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProgramme), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                function.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProgramme), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新当前流程试验步骤
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="funcname">流程名称</param>
        /// <param name="processes">试验步骤</param>
        /// <returns></returns>
        public OperateResult UpdateProgramme(Test_Programme function)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                if (Sqlite.Select<Test_Programme>().Count() < 1)
                {
                    return InsertTestProgramme(function);
                }

                int row = Sqlite.Update<Test_Programme>()
                            .Where(x => x.ProjectID == function.ProjectID)
                            .Set(x => x.Test_FlowNames, function.Test_FlowNames)
                            .Set(x => x.UpdateOperator, function.UpdateOperator)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .ExecuteAffrows();
                if (row < 1)
                {
                    LastError = $"测试方案 未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 获取试验方案
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<Test_Programme> GetProgramme(int id)
        {

            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Test_Programme>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Programme>().Where(x => x.ProjectID == id).ToOne() ?? new Test_Programme();

                return OperateResult<Test_Programme>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetFlowsByNameID), LastError);
                return OperateResult<Test_Programme>.Excepted(null, ex);
            }
        }
        #endregion

        #region 结果条目操作
        /// <summary>
        /// 插入试验结果条目
        /// </summary>
        /// <param name="info">试验结果条目</param>
        /// <returns></returns>
        public OperateResult<string> InserTestItem(Test_ItemInfo info)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult<string>.Failed(string.Empty, LastError);
            }

            try
            {
                if (info == null)
                {
                    LastError = $"试验待存储条目不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<string>.Failed(string.Empty, LastError);
                }

                //string itemtableName = nameof(Test_ItemInfo)+
                //if (Sqlite.DbFirst.ExistsTable(tableName) == false)
                //    Sqlite.CodeFirst.SyncStructure(typeof(Test_ItemInfo), tableName);

                info.Insert();

                if (info.Id < 1)
                {
                    LastError = $"试验条目未实际发生存储";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<string>.Failed(string.Empty, LastError);
                }
                string datatableName = $"{info.ProductSN}_{info.CreateTime.Ticks}";
                ////创建数据库表
                //if (Sqlite.DbFirst.ExistsTable(datatableName) == false)
                //    Sqlite.CodeFirst.SyncStructure(typeof(Test_DataInfo), datatableName);

                return OperateResult<string>.Succeed(datatableName);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult<string>.Excepted(string.Empty, ex);
            }
        }

        /// <summary>
        /// 更新试验结果条目
        /// </summary>
        /// <param name="info">更新试验结果条目</param>
        /// <returns></returns>
        public OperateResult UpdateTetsItem(Test_ItemInfo info)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                if (info == null)
                {
                    LastError = $"试验待存储条目不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult.Failed(LastError);
                }

                if (!info.Update())
                {
                    LastError = $"试验条目未实际发生更新";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 项目条目查新（测试版本）
        /// </summary>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> GetTestItem()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }
            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ItemInfo> items = Sqlite.Select<Test_ItemInfo>().ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目列表
        /// </summary>
        /// <param name="proid">项目ID</param>
        /// <param name="starttime">试验开始时间</param>
        /// <param name="endtime">试验结束时间</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> GetTestItems(int proid, DateTime starttime, DateTime endtime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ItemInfo> items = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == proid && x.CreateTime > starttime && x.CreateTime < endtime)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目列表
        /// </summary>
        /// <param name="productsn">产品编号</param>
        /// <param name="starttime">试验开始时间</param>
        /// <param name="endtime">试验结束时间</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> GetTestItems(string productsn, DateTime starttime, DateTime endtime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ItemInfo> items = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProductSN == productsn && x.CreateTime > starttime && x.CreateTime < endtime)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目列表
        /// </summary>
        /// <param name="proid">项目ID</param>
        /// <param name="productsn">产品编号</param>
        /// <param name="starttime">试验开始时间</param>
        /// <param name="endtime">试验结束时间</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> GetTestItems(int proid, string productsn, DateTime starttime, DateTime endtime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ItemInfo> items = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == proid && x.ProductSN == productsn && x.CreateTime > starttime && x.CreateTime < endtime)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestItem), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目（通过条目ID和试验开始时间）
        /// </summary>
        /// <param name="id">条目ID</param>
        /// <param name="starttime">试验开始时间</param>
        /// <returns></returns>
        public OperateResult<Test_ItemInfo> GetTestItemByIDAndTime(int id, DateTime starttime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestItemByIDAndTime), LastError);
                return OperateResult<Test_ItemInfo>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                Test_ItemInfo item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.Id == id && x.CreateTime == starttime)
                    .ToOne() ?? new Test_ItemInfo();

                return OperateResult<Test_ItemInfo>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestItemByIDAndTime), LastError);
                return OperateResult<Test_ItemInfo>.Excepted(null, ex);
            }
        }


        /// <summary>
        /// 获取测试最早开始时间
        /// </summary>
        /// <returns></returns>
        public OperateResult<DateTime> GetTestRangeStartTime()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestRangeStartTime), LastError);
                return OperateResult<DateTime>.Failed(DateTime.MinValue, LastError);
            }
            try
            {
                Test_ItemInfo item = Sqlite.Select<Test_ItemInfo>().Limit(1).ToOne();
                return OperateResult<DateTime>.Succeed(item.CreateTime);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestRangeStartTime), LastError);
                return OperateResult<DateTime>.Excepted(DateTime.MinValue, ex);
            }
        }

        /// <summary>
        /// 获取测试最晚结束时间
        /// </summary>
        /// <returns></returns>
        public OperateResult<DateTime> GetTestRangeEndTime()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestRangeEndTime), LastError);
                return OperateResult<DateTime>.Failed(DateTime.MinValue, LastError);
            }
            try
            {
                Test_ItemInfo item = Sqlite.Select<Test_ItemInfo>()
                    .OrderBy(x => x.Id)
                    .Limit(1)
                    .ToOne();
                return OperateResult<DateTime>.Succeed(item.CreateTime);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestRangeEndTime), LastError);
                return OperateResult<DateTime>.Excepted(DateTime.MinValue, ex);
            }
        }
        #endregion

        #region 试验数据操作

        /// <summary>
        /// 插入试验数据
        /// </summary>
        /// <param name="info">试验数据</param>
        /// <returns></returns>
        public OperateResult InserTestData(Test_DataInfo info)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                if (info == null)
                {
                    LastError = $"试验待存储条目不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
                    return OperateResult.Failed(LastError);
                }

                info.Insert();

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
                return OperateResult.Excepted(ex);
            }
        }


        public OperateResult<List<Test_DataInfo>> GetTestData(int itemid, DateTime StratTime, DateTime StopTime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<List<Test_DataInfo>>.Failed(null, LastError);
            }

            try
            {
                //var ufos = Sqlite.GetGuidRepository<Test_DataInfo>(null, oldname => tablename);
                var items = Sqlite
                    .Select<Test_DataInfo>()
                    .Where(x => x.TestItemID == itemid && x.CreateTime >= StratTime && StopTime >= x.CreateTime).ToList();
                //var items = Sqlite.Select<Test_DataInfo>().Where(x => x.TestItemID == itemid).ToList();

                return OperateResult<List<Test_DataInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<List<Test_DataInfo>>.Excepted(null, ex);
            }
        }
        /// <summary>
        /// 获取分页测试数据
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="StratTime"></param>
        /// <param name="StopTime"></param>
        /// <returns></returns>
        public OperateResult<List<Test_DataInfo>> GetLimitTestData(int itemid, DateTime StratTime, DateTime StopTime, long page)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<List<Test_DataInfo>>.Failed(null, LastError);
            }

            try
            {
                const int count = 1000;
                //var ufos = Sqlite.GetGuidRepository<Test_DataInfo>(null, oldname => tablename);
                var items = Sqlite
                    .Select<Test_DataInfo>()
                    .Where(x => x.TestItemID == itemid && x.CreateTime >= StratTime && StopTime >= x.CreateTime)
                    .OrderBy(x => x.CreateTime)
                    .Limit(count)
                    .Offset((int)(count * (page - 1))).ToList();
                //var items = Sqlite.Select<Test_DataInfo>().Where(x => x.TestItemID == itemid).ToList();

                return OperateResult<List<Test_DataInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<List<Test_DataInfo>>.Excepted(null, ex);
            }
        }

        public OperateResult<long> GetTestDataCount(int itemid, DateTime StratTime, DateTime StopTime)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<long>.Failed(0, LastError);
            }

            try
            {
                //var ufos = Sqlite.GetGuidRepository<Test_DataInfo>(null, oldname => tablename);
                long count = Sqlite
                    .Select<Test_DataInfo>()
                    .Where(x => x.TestItemID == itemid && x.CreateTime >= StratTime && StopTime >= x.CreateTime).ToList().Count;
                //var items = Sqlite.Select<Test_DataInfo>().Where(x => x.TestItemID == itemid).ToList();

                return OperateResult<long>.Succeed(count);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<long>.Excepted(0, ex);
            }
        }



        #endregion

        public void SetError(string error) => LastError = error;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public DBOperate() : this(null)
        {
        }



        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public DBOperate(ILogger logger)
        {
            Logger = logger ?? SuperDHHLoggerManager.DBLogger;
        }

        #endregion

        #region 析构方法

        ~DBOperate()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                Sqlite?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
