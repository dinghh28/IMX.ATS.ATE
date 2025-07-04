using FreeSql;
using IMX.ATE.Common;
using IMX.Common;
using IMX.DB.Model;
using IMX.Logger;
using Newtonsoft.Json;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using DataType = FreeSql.DataType;

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

        /// <summary>
        /// 用户信息
        /// </summary>
        public string UpdateOperator { get; set; } = string.Empty;
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
                var items = Sqlite.Select<Test_ProjectInfo>().Distinct().ToList(x=>x.ProjectName);
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
        /// 获取所有项目编码
        /// </summary>
        /// <returns></returns>
        public OperateResult<Dictionary<string, int>> GetProjectSNs() 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetProjectSNs), LastError);
                return OperateResult<Dictionary<string, int>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_ProjectInfo>().Distinct().ToDictionary(x => x.ProjectSN,x=>x.Id);
                return OperateResult<Dictionary<string,int>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetProjectSNs), LastError);
                return OperateResult<Dictionary<string, int>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取项目名称及其对应ID字典
        /// </summary>
        /// <returns></returns>
        public OperateResult<Dictionary<string, int>> GetProjectName_Dic() 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<Dictionary<string, int>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_ProjectInfo>()
                    .OrderBy(x=>x.Id)
                    .Distinct()
                    .ToDictionary(x=>x.ProjectName, x=>x.Id);
                return OperateResult<Dictionary<string, int>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetProjectNames), LastError);
                return OperateResult<Dictionary<string, int>>.Excepted(null, ex);
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
                Logger.Error(nameof(DBOperate), nameof(GetProjectInfo_ByName), LastError);
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
                Logger.Error(nameof(DBOperate), nameof(GetProjectInfo_ByName), LastError);
                return OperateResult<Test_ProjectInfo>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取项目详细信息(通过项目ID)
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<Test_ProjectInfo> GetProjectInfo_ByID(int id) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetProjectInfo_ByID), LastError);
                return OperateResult<Test_ProjectInfo>.Failed(null, LastError);
            }

            try
            {
                var items = Test_ProjectInfo.Find(id);

                return OperateResult<Test_ProjectInfo>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetProjectInfo_ByID), LastError);
                return OperateResult<Test_ProjectInfo>.Excepted(null, ex);
            }
        }

        #region 开关机流程
        /// <summary>
        /// 更新开关机流程
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="processes">流程</param>
        /// <param name="IsOpen">是否为开机流程</param>
        /// <returns></returns>
        public OperateResult UpdataedTestFlow(int id, List<ModTestProcess> processes, bool IsOpen)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdataedTestFlow), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                int row = 0;
                if (IsOpen)
                {
                    row = Sqlite.Update<Test_ProjectInfo>()
                       .Where(x => x.Id == id)
                       .Set(x => x.Test_OpenFlows, processes)
                       .Set(x => x.UpdateTime, DateTime.Now)
                       .ExecuteAffrows();
                    if (row < 1)
                    {
                        LastError = "开机流程未发生实际变更";
                        Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                        return OperateResult.Failed(LastError);
                    }
                }
                else
                {
                    row = Sqlite.Update<Test_ProjectInfo>()
                       .Where(x => x.Id == id)
                       .Set(x => x.Test_ShutFlows, processes)
                       .Set(x => x.UpdateTime, DateTime.Now)
                       .ExecuteAffrows();
                    if (row < 1)
                    {
                        LastError = "关机流程未发生实际变更";
                        Logger.Error(nameof(DBOperate), nameof(UpdateProgram), LastError);
                        return OperateResult.Failed(LastError);
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdataedTestFlow), LastError);
                return OperateResult.Excepted(ex);
            }

            return OperateResult.Succeed();
        }
        #endregion

        #region DBC配置联动
        /// <summary>
        /// 设置DBC下发状态
        /// </summary>
        /// <param name="dbcid">DBC配置ID</param>
        /// <param name="cansend">允许发送标志</param>
        /// <returns></returns>
        public OperateResult SetDBCSendState(int dbcid,bool cansend) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var affrows = Sqlite.Update<Test_ProjectInfo>()
                    .Set(x=>x.CanSendDBC, cansend)
                    .Where(x=>x.DBCConfigID == dbcid)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC配置未与项目同步，请注意产品指令下发模板配置内容";
                    Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 设置DBC下发状态
        /// </summary>
        /// <param name="id">项目id</param>
        /// <param name="cansend">允许发送标志</param>
        /// <returns></returns>
        public OperateResult SetDBCSendState_Singleton(int id, bool cansend) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                Test_ProjectInfo.Find(id).CanSendDBC = cansend;

                var affrows = Sqlite.Update<Test_ProjectInfo>()
                    .Set(x => x.CanSendDBC, cansend)
                    .Where(x => x.Id == id)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC配置未与项目同步，请注意产品指令下发模板配置内容";
                    Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SetDBCSendState), LastError);
                return OperateResult.Excepted(ex);
            }
        }
        #endregion

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
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
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
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新DBC配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">配置名称</param>
        /// <param name="describe">配置描述</param>
        /// <param name="filename">DBC文件名</param>
        /// <param name="fileid">DBC文件ID</param>
        /// <param name="enableusesend">下发信号可用使能</param>
        /// <param name="enableusereceive">上报信号可用使能</param>
        /// <returns></returns>
        public OperateResult UpdateDBCConfig(int id, string name, string describe, string filename, int fileid, Electricity electricity, bool enableusesend, bool enableusereceive)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                var affrows = Sqlite.Update<Test_DBCConfig>(id)
                    .Set(x=>x.EnableUse, true)
                    .Set(x => x.EnableUseReceive, enableusereceive)
                    .Set(x => x.EnableUseSend, enableusesend)
                    .Set(x => x.ConfigName, name)
                    .Set(x => x.Describe, describe)
                    .Set(x => x.DBCFileName, filename)
                    .Set(x => x.DBCFileID, fileid)
                    .Set(x => x.Electricity, electricity)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC配置信息未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                    return OperateResult.Failed();
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }

        }

        /// <summary>
        /// 更新DBC配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">配置名称</param>
        /// <param name="describe">配置描述</param>
        /// <param name="enableusereceive">上报信号可用使能</param>
        /// <returns></returns>
        public OperateResult UpdateDBCConfig(int id, string name, string describe, Electricity electricity, bool enableusereceive)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                return OperateResult.Failed(LastError);
            }
            try
            {
                var affrows = Sqlite.Update<Test_DBCConfig>(id)
                    .Set(x => x.EnableUseReceive, enableusereceive)
                    .Set(x => x.ConfigName, name)
                    .Set(x => x.Describe, describe)
                    .Set(x => x.Electricity, electricity)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC配置信息未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                    return OperateResult.Failed();
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCConfig), LastError);
                return OperateResult.Excepted(ex);
            }

        }

        /// <summary>
        /// 更新上报信号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="signals">上报信号配置列表</param>
        /// <returns></returns>
        public OperateResult UpdateReceiveSignals(int id, List<Test_DBCInfo> signals)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateReceiveSignals), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var affrows = Sqlite.Update<Test_DBCConfig>(id)
                    .Set(x => x.EnableUseReceive, true)
                    .Set(x => x.Test_DBCReceiveSignals, signals)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC上报信号未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateReceiveSignals), LastError);
                    return OperateResult.Failed();
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateReceiveSignals), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新下发信号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="signals">下发信号列表</param>
        /// <param name="messages">下发消息列表</param>
        /// <returns></returns>
        public OperateResult UpdateSendSignals(int id, List<Test_DBCInfo> signals, List<Test_DBCMessageInfo> messages)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateSendSignals), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var affrows = Sqlite.Update<Test_DBCConfig>(id)
                    .Set(x => x.EnableUseSend, true)
                    .Set(x => x.Test_DBCSendSignals, signals)
                    .Set(x => x.Test_DBCSendMessages, messages)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC下发信号未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateSendSignals), LastError);
                    return OperateResult.Failed();
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateSendSignals), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新DBC使用状态（全置false）
        /// </summary>
        /// <param name="id">对应DBC文件ID</param>
        /// <returns></returns>
        public OperateResult UpdateDBCEnableState_False(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCEnableState_False), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var affrows = Sqlite.Update<Test_DBCConfig>().Where(x => x.DBCFileID == id)
                    .Set(x => x.EnableUse, false)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"DBC使用状态未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateDBCEnableState_False), LastError);
                    return OperateResult.Failed();
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateDBCEnableState_False), LastError);
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
        /// 获取DBC配置
        /// </summary>
        /// <param name="id">DBC配置ID</param>
        /// <returns></returns>
        public OperateResult<Test_DBCConfig> GetDBCConfig_ByID(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByID), LastError);
                return OperateResult<Test_DBCConfig>.Failed(null, LastError);
            }

            try
            {
                var items  =  Test_DBCConfig.Find(id);
                //var items = Sqlite.Select<Test_DBCConfig>().Where(x => x.ProjectID == id).ToOne();
                if (items == null)
                {
                    return OperateResult<Test_DBCConfig>.Succeed(new Test_DBCConfig());
                }
                return OperateResult<Test_DBCConfig>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByID), LastError);
                return OperateResult<Test_DBCConfig>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 当前DBC配置使用状态判断
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult<bool> DBCConfigCanUse(int id) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByID), LastError);
                return OperateResult<bool>.Failed(false, LastError);
            }

            try
            {
                var items = Test_DBCConfig.Find(id);
                if (items == null)
                {
                    return OperateResult<bool>.Succeed(false);
                }


                //var items = Sqlite.Select<Test_DBCConfig>().Where(x => x.DBCFileID == id).ToOne(x=>x.EnableUse&&x.EnableUseReceive&&x.EnableUseSend);

                return OperateResult<bool>.Succeed(items.EnableUse && items.EnableUseReceive && items.EnableUseSend);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetDBCConfig_ByID), LastError);
                return OperateResult<bool>.Excepted(false, ex);
            }
        }

        /// <summary>
        /// 获取所有DBC配置信息
        /// </summary>
        /// <returns></returns>
        public OperateResult<List<Test_DBCConfig>> SelectedDBCConfig_All() 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCConfig_All), LastError);
                return OperateResult<List<Test_DBCConfig>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_DBCConfig>().ToList();
                return OperateResult<List<Test_DBCConfig>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCConfig_All), LastError);
                return OperateResult<List<Test_DBCConfig>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取DBC配置信息
        /// </summary>
        /// <param name="electricity">配置供电类型</param>
        /// <returns></returns>
        public OperateResult<List<Test_DBCConfig>> SelectedDBCConfig(Electricity electricity)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCConfig), LastError);
                return OperateResult<List<Test_DBCConfig>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_DBCConfig>()
                    .Where(x=>x.Electricity == electricity)
                    //.Where(x=>x.EnableUse)SS
                    .ToList();
                return OperateResult<List<Test_DBCConfig>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCConfig), LastError);
                return OperateResult<List<Test_DBCConfig>>.Excepted(null, ex);
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

        #region DBC文件和配置联合操作
        public OperateResult<Test_DBCConfig> SelectedDBCByDBCID(int id) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCByDBCID), LastError);
                return OperateResult<Test_DBCConfig>.Failed(null, LastError);
            }

            try
            {
                //var items = Test_DBCConfig.Find(id);
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
                Logger.Error(nameof(DBOperate), nameof(SelectedDBCByDBCID), LastError);
                return OperateResult<Test_DBCConfig>.Excepted(null, ex);
            }
        }
        #endregion

        #region 自定义帧配置操作
        /// <summary>
        /// 获取配置名称
        /// </summary>
        /// <param name="id">对应DBC配置ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetCustomConfigName (int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetCustomConfigName), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }

            try
            {
                //var item =  Test_CustomMessageInfo.Find(id);

                var items = Sqlite.Select<Test_CustomMessageInfo>().Where(x => x.DBCConfigID == id).ToList(x=>x.ConfigName);

                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetCustomConfigName), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取自定义配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperateResult<List<Test_CustomMessageInfo>> GetCustomConfig(int id) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetCustomConfigName), LastError);
                return OperateResult<List<Test_CustomMessageInfo>>.Failed(null, LastError);
            }

            try
            {
                //var item =  Test_CustomMessageInfo.Find(id);

                var items = Sqlite.Select<Test_CustomMessageInfo>().Where(x => x.DBCConfigID == id).ToList();

                return OperateResult<List<Test_CustomMessageInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetCustomConfigName), LastError);
                return OperateResult<List<Test_CustomMessageInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取对应消息配置信息
        /// </summary>
        /// <param name="id">配置自id</param>
        /// <returns></returns>
        public OperateResult<List<Test_CustomMessage>> GetCustomMessages(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetCustomMessages), LastError);
                return OperateResult<List<Test_CustomMessage>>.Failed(null, LastError);
            }

            try
            {
                //var item =  Test_CustomMessageInfo.Find(id);

                var items = Sqlite.Select<Test_CustomMessageInfo>().Where(x => x.Id == id).ToOne(x => x.CustomMessages);

                return OperateResult<List<Test_CustomMessage>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetCustomMessages), LastError);
                return OperateResult<List<Test_CustomMessage>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取对应消息配置信息
        /// </summary>
        /// <param name="id">对应DBC配置ID</param>
        /// <param name="name">消息配置名称</param>
        /// <returns></returns>
        public OperateResult<List<Test_CustomMessage>> GetCustomMessages(int id, string name) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetCustomMessages), LastError);
                return OperateResult<List<Test_CustomMessage>>.Failed(null, LastError);
            }

            try
            {
                //var item =  Test_CustomMessageInfo.Find(id);

                var items = Sqlite.Select<Test_CustomMessageInfo>()
                    .Where(x => x.DBCConfigID == id && x.ConfigName == name)
                    .ToOne(x => x.CustomMessages);

                return OperateResult<List<Test_CustomMessage>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetCustomMessages), LastError);
                return OperateResult<List<Test_CustomMessage>>.Excepted(null, ex);
            }
        }


        /// <summary>
        /// 插入DBC配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public OperateResult InsertCustomConfig(Test_CustomMessageInfo config)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertCustomConfig), LastError);
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
                Logger.Error(nameof(DBOperate), nameof(InsertCustomConfig), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        /// <summary>
        /// 更新自定义配置
        /// </summary>
        /// <returns></returns>
        public OperateResult UpdateCustomMessage(int id, List<Test_CustomMessage> messages) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(UpdateCustomMessage), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                var affrows = Sqlite.Update<Test_CustomMessageInfo>(id)
                    .Set(x => x.CustomMessages, messages)
                    .Set(x => x.UpdateOperator, UpdateOperator)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .ExecuteAffrows();

                if (affrows < 1)
                {
                    LastError = $"自定义帧配置未发生实际变化";
                    Logger.Error(nameof(DBOperate), nameof(UpdateCustomMessage), LastError);
                    return OperateResult.Failed(LastError);
                }
                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(UpdateCustomMessage), LastError);
                return OperateResult.Excepted(ex);
            }
        }
        #endregion

        #region 项目测试项操作

        /// <summary>
        /// 插入测试项
        /// </summary>
        /// <param name="function">测试项</param>
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
                function.UpdateOperator = UpdateOperator;
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
                var items = Sqlite.Select<Test_Process>()
                    .Where(x => x.ProjectID == id && !x.IsDeleted)
                    .ToDictionary(x => x.FunctionName, x => x.Description);

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

        /// <summary>
        /// 获取试验流程
        /// </summary>
        /// <param name="id"></param>
        /// <param name="funcnames"></param>
        /// <returns></returns>
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
                $"    ProjectID = {id}\n" +
                $"    AND\n"+
                $"    (\n";

                for (int i = 0; i < funcnames.Count; i++)
                {
                    if (i == funcnames.Count - 1)
                    {
                        sqlStr += $"        FunctionName = '{funcnames[i]}'\n"
                            +$"   )";
                        break;
                    }
                    sqlStr += $"        FunctionName = '{funcnames[i]}'\n" + "      OR\n";
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
        /// 获取试验流程
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="funcnames"></param>
        /// <returns></returns>
        public OperateResult<Dictionary<string, Test_Process>> SelectFlowsByNameID(int id, List<string> funcnames)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, Test_Process>>.Failed(null, LastError);
            }

            if (funcnames == null)
            {
                LastError = $"流程方法名称不可为空";
                Logger.Error(nameof(DBOperate), nameof(SelectFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, Test_Process>>.Failed(null, LastError);
            }

            if (funcnames.Count < 1)
            {
                LastError = $"未获取需检索流程方法名称";
                Logger.Error(nameof(DBOperate), nameof(SelectFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, Test_Process>>.Failed(null, LastError);
            }

            try
            {
                string sqlStr =
                $"SELECT\n" +
                $"    *\n" +
                $"FROM\n" +
                $"    Test_Process\n" +
                $"WHERE\n" +
                $"    ProjectID = {id}\n" +
                $"    AND\n" +
                $"    (\n";

                for (int i = 0; i < funcnames.Count; i++)
                {
                    if (i == funcnames.Count - 1)
                    {
                        sqlStr += $"        FunctionName = '{funcnames[i]}'\n"
                            + $"   )";
                        break;
                    }
                    sqlStr += $"        FunctionName = '{funcnames[i]}'\n" + "      OR\n";
                }

                List<Test_Process> list = Sqlite.Select<Test_Process>().WithSql(sqlStr).ToList();

                if (list.Count < 1)
                {
                    LastError = $"相关流程不存在，请确认完成相关配置";
                    Logger.Error(nameof(DBOperate), nameof(SelectFlowsByNameID), LastError);
                    return OperateResult<Dictionary<string, Test_Process>>.Failed(null, LastError);
                }

                Dictionary<string, Test_Process> data = new Dictionary<string, Test_Process>();

                for (int i = 0; i < list.Count; i++)
                {
                    data.Add(list[i].FunctionName, list[i]);
                }

                return OperateResult<Dictionary<string, Test_Process>>.Succeed(data);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectFlowsByNameID), LastError);
                return OperateResult<Dictionary<string, Test_Process>>.Excepted(null, ex);
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
                            .Set(x => x.UpdateOperator, UpdateOperator)
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

#if DEBUG
        /// <summary>
        /// 试验项插入功能调试
        /// </summary>
        /// <returns></returns>
        public OperateResult Test_InsertTestProccess()
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                Test_Process function = new Test_Process
                {
                    Test_Flows = new List<ModTestProcess>(),
                    //SaveDatas = new Test_ProcessSaveData(),
                };

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
        /// 更新测试项存储数据(仅调试修正可用)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="funcname"></param>
        /// <param name="eupreaddata"></param>
        /// <param name="eupsetdata"></param>
        /// <param name="proreaddata"></param>
        /// <param name="prosetdata"></param>
        /// <param name="usecalculate"></param>
        /// <param name="calculatedata"></param>
        /// <param name="usecostomread"></param>
        /// <param name="costomreaddata"></param>
        /// <returns></returns>
        public OperateResult UpdateTestProccessSaveData(
            int id,
            string funcname,
            List<ModTestDataInfo> eupreaddata,
            List<ModTestDataInfo> eupsetdata,
            List<ModTestDataInfo> proreaddata,
            List<ModTestDataInfo> prosetdata,
            bool usecalculate,
            List<ModTestDataInfo> calculatedata,
            bool usecostomread,
            List<ModTestDataInfo> costomreaddata
            )
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult.Failed(LastError);
            }

            try
            {
                int row = Sqlite.Update<Test_Process>()
                            .Where(x => x.ProjectID == id && x.FunctionName == funcname)
                            .Set(x => x.Test_ReadData_Euq, eupreaddata)
                            .Set(x => x.Test_SetData_Euq, eupsetdata)
                            .Set(x => x.Test_ReadData_Pro, proreaddata)
                            .Set(x => x.Test_SetData_Pro, prosetdata)
                            .Set(x => x.UseCalculateData, usecalculate)
                            .Set(x => x.Test_CalculateData, calculatedata)
                            .Set(x => x.UseCustomData, usecostomread)
                            .Set(x => x.Test_CustomData, costomreaddata)
                            .Set(x => x.UpdateTime, DateTime.Now)
                            .Set(x => x.UpdateOperator, UpdateOperator)
                            .ExecuteAffrows();

                if (row < 1)
                {
                    LastError = $"测试项 【{funcname}】 存储数据 未实际发生变更";
                    Logger.Error(nameof(DBOperate), nameof(UpdateProcess), LastError);
                    return OperateResult.Failed(LastError);
                }

                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InsertTestProccess), LastError);
                return OperateResult.Excepted(ex);
            }
        }
#endif

        #endregion

        #region 项目方案操作

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
                Logger.Error(nameof(DBOperate), nameof(GetProgrammeName), LastError);
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
                Logger.Error(nameof(DBOperate), nameof(GetProgrammeName), LastError);
                return OperateResult<Test_Programme>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验阶段名字
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetRunProgrammeNames(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetRunProgrammeNames), LastError);
                return OperateResult<List<string>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_Programme>().Where(x => x.ProjectID == id).ToOne(x => x.Test_FlowNames);

                return OperateResult<List<string>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(GetRunProgrammeNames), LastError);
                return OperateResult<List<string>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验急停流程
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public OperateResult<List<string>> GetOffProgrammeName(int id)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetOffProgrammeName), LastError);
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
                Logger.Error(nameof(DBOperate), nameof(GetOffProgrammeName), LastError);
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
        #endregion

        #region 试验结果项目信息条目操作
        /// <summary>
        /// 插入试验结果项目信息条目
        /// </summary>
        /// <param name="info">json字符串</param>
        /// <returns></returns>
        public OperateResult<long> InserTestProjectItem(string info) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InserTestProjectItem), LastError);
                return OperateResult<long>.Failed(-1, LastError);
            }
            try
            {
                if (info == null)
                {
                    LastError = $"试验待存储结果项目信息条目不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestProjectItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                //string itemtableName = nameof(Test_ItemInfo)+
                //if (Sqlite.DbFirst.ExistsTable(tableName) == false)
                //    Sqlite.CodeFirst.SyncStructure(typeof(Test_ItemInfo), tableName);

                var item = JsonConvert.DeserializeObject<Test_ProjectItemInfo>(info);

                if (item == null)
                {
                    LastError = $"试验结果结果项目信息条目数据格式异常:\r\n{info}";
                    Logger.Fatal(nameof(DBOperate), nameof(InserTestProjectItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }


                //item.Insert();
                long id = Sqlite.Insert(item).ExecuteIdentity();
                if (id < 1)
                {
                    LastError = $"试验条目未实际发生存储";
                    Logger.Error(nameof(DBOperate), nameof(InserTestProjectItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                return OperateResult<long>.Succeed(id);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestProjectItem), LastError);
                return OperateResult<long>.Excepted(-1, ex);
            }
        }

        #region 全条件检索
        /// <summary>
        /// 条件检索试验结果项目信息条目
        /// </summary>
        /// <param name="projectid">项目ID</param>
        /// <param name="productsn">产品SN</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="startpage">开始页数</param>
        /// <param name="pagesize">单页数量</param>
        /// <returns></returns>
        public OperateResult<List<Test_ProjectItemInfo>> SelectTestProjectItem(int projectid, string productsn, DateTime start, DateTime end, int startpage, int pagesize)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ProjectItemInfo> items = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x => x.ProjectID == projectid && x.ProductSN == productsn && x.CreateTime.Between(start,end))
                    .Page(startpage, pagesize)
                    .ToList();

                return OperateResult<List<Test_ProjectItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 条件检索试验结果项目信息条目数量
        /// </summary>
        /// <param name="projectid">项目ID</param>
        /// <param name="productsn">产品SN</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public OperateResult<long> SelectTestProjectItem_Count(int projectid, string productsn, DateTime start, DateTime end) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Failed(-1, LastError);
            }

            try
            {                
                long itemscount = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x => x.ProjectID == projectid && x.ProductSN == productsn && x.CreateTime.Between(start, end))
                    .Count();

                return OperateResult<long>.Succeed(itemscount);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Excepted(-1, ex);
            }
        }
        #endregion

        #region 仅项目ID
        /// <summary>
        /// 条件检索试验结果项目信息条目(仅项目ID)
        /// </summary>
        /// <param name="projectid">项目ID</param
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="startpage">开始页数</param>
        /// <param name="pagesize">单页数量</param>
        /// <returns></returns>
        public OperateResult<List<Test_ProjectItemInfo>> SelectTestProjectItem(int projectid, DateTime start, DateTime end, int startpage, int pagesize)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ProjectItemInfo> items = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x => x.ProjectID == projectid && x.CreateTime.Between(start, end))
                    .Page(startpage, pagesize)
                    .ToList();

                return OperateResult<List<Test_ProjectItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Excepted(null, ex);
            }

        }

        /// <summary>
        /// 条件检索试验结果项目信息条目数量
        /// </summary>
        /// <param name="projectid">项目ID</param>
        /// <param name="productsn">产品SN</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public OperateResult<long> SelectTestProjectItem_Count(int projectid, DateTime start, DateTime end)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Failed(-1, LastError);
            }

            try
            {
                long itemscount = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x => x.ProjectID == projectid && x.CreateTime.Between(start, end))
                    .Count();

                return OperateResult<long>.Succeed(itemscount);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Excepted(-1, ex);
            }
        }
        #endregion

        #region 仅产品编码
        /// <summary>
        /// 条件检索试验结果项目信息条目（仅产品编码）
        /// </summary>
        /// <param name="productsn">产品编码</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="startpage">开始页数</param>
        /// <param name="pagesize">单页数量</param>
        /// <returns></returns>
        public OperateResult<List<Test_ProjectItemInfo>> SelectTestProjectItem(string productsn, DateTime start, DateTime end, int startpage, int pagesize)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                List<Test_ProjectItemInfo> items = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x =>  x.CreateTime.Between(start, end) && x.ProductSN.Contains(productsn))
                    .Page(startpage, pagesize)
                    .ToList();

                return OperateResult<List<Test_ProjectItemInfo>>.Succeed(items);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem), LastError);
                return OperateResult<List<Test_ProjectItemInfo>>.Excepted(null, ex);
            }

        }
        
        /// <summary>
        /// 条件检索试验结果项目信息条目数量
        /// </summary>
        /// <param name="projectid">项目ID</param>
        /// <param name="productsn">产品SN</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public OperateResult<long> SelectTestProjectItem_Count(string productsn, DateTime start, DateTime end)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Failed(-1, LastError);
            }

            try
            {
                long itemscount = Sqlite.Select<Test_ProjectItemInfo>()
                    .Where(x => x.CreateTime.Between(start, end) && x.ProductSN.Contains(productsn))
                    .Count();

                return OperateResult<long>.Succeed(itemscount);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestProjectItem_Count), LastError);
                return OperateResult<long>.Excepted(-1, ex);
            }
        }
        #endregion

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
        /// 插入试验结果条目
        /// </summary>
        /// <param name="info">试验结果条目Json字符串</param>
        /// <returns></returns>
        public OperateResult<long> InserTestItem(string info,long projectid) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult<long>.Failed(-1, LastError);
            }
            try
            {
                if (info == null)
                {
                    LastError = $"试验待存储条目不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                //string itemtableName = nameof(Test_ItemInfo)+
                //if (Sqlite.DbFirst.ExistsTable(tableName) == false)
                //    Sqlite.CodeFirst.SyncStructure(typeof(Test_ItemInfo), tableName);

                var item = JsonConvert.DeserializeObject<Test_ItemInfo>(info);
               
                if (item == null)
                {
                    LastError = $"试验结果条目数据格式异常:\r\n{info}";
                    Logger.Fatal(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                item.ProjectID = (int)projectid;

                long id =  Sqlite.Insert(item).ExecuteIdentity();

                if (id < 1)
                {
                    LastError = $"试验条目未实际发生存储";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                return OperateResult<long>.Succeed(id);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                return OperateResult<long>.Excepted(-1, ex);
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

        #region 获取试验条目
        /// <summary>
        /// 获取试验条目
        /// </summary>
        /// <param name="id">试验项目信息ID</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> SelectTestItems(long id) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                var item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == id)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目（分页查询）
        /// </summary>
        /// <param name="id">试验项目信息ID</param>
        /// <param name="startpage"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> SelectTestItems(long id,int startpage, int pagesize)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                var item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == id)
                    .Page(startpage,pagesize)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目（测试项名称模糊查询）
        /// </summary>
        /// <param name="id">试验项目信息ID</param>
        /// <param name="flowname">测试项名称</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> SelectTestItems(long id, string flowname) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                var item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == id && x.FlowName.Contains(flowname))
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目（试验结果查询）
        /// </summary>
        /// <param name="id">试验项目信息ID</param>
        /// <param name="state">试验结果</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> SelectTestItems(long id, ResultState state) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                var item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == id && x.Result == state)
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }

        /// <summary>
        /// 获取试验条目（试验结果+名称查询）
        /// </summary>
        /// <param name="id">试验项目信息ID</param>
        /// <param name="state">试验结果</param>
        /// <returns></returns>
        public OperateResult<List<Test_ItemInfo>> SelectTestItems(long id, string flowname, ResultState state)
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Failed(null, LastError);
            }

            try
            {
                //Sqlite.CodeFirst.GetTableByEntity(typeof(Test_ItemInfo)).AsTableImpl.SetDefaultAllTables(value => value.Take(3).ToArray());
                var item = Sqlite.Select<Test_ItemInfo>()
                    .Where(x => x.ProjectID == id && x.Result == state && x.FlowName.Contains(flowname))
                    .ToList();

                return OperateResult<List<Test_ItemInfo>>.Succeed(item);
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(SelectTestItems), LastError);
                return OperateResult<List<Test_ItemInfo>>.Excepted(null, ex);
            }
        }
        #endregion

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
                    LastError = $"试验待存储数据不可为空";
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

        /// <summary>
        /// 插入试验数据
        /// </summary>
        /// <param name="info">试验数据Json字符串</param>
        /// <param name="id">测试项存储ID</param>
        /// <returns></returns>
        public OperateResult InserTestData(string info, long id) 
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
                    LastError = $"试验待存储数据不可为空";
                    Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
                    return OperateResult.Failed(LastError);
                }

                Test_DataInfo item = JsonConvert.DeserializeObject<Test_DataInfo>(info);

                if (item == null)
                {
                    LastError = $"[试验数据]数据格式异常:\r\n{info}";
                    Logger.Fatal(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }

                item.TestItemID = id;

                int rows = Sqlite.Insert(item).ExecuteAffrows();

                if (rows < 1)
                {
                    LastError = $"试验数据未实际发生存储";
                    Logger.Error(nameof(DBOperate), nameof(InserTestItem), LastError);
                    return OperateResult<long>.Failed(-1, LastError);
                }


                return OperateResult.Succeed();
            }
            catch (Exception ex)
            {
                LastError = ex.GetMessage();
                Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
                return OperateResult.Excepted(ex);
            }
        }

        ///// <summary>
        ///// 插入试验项目
        ///// </summary>
        ///// <param name="info">试验数据Json字符串</param>
        ///// <returns></returns>
        //public OperateResult InserTestData(string info) 
        //{
        //    if (!IsInitOK)
        //    {
        //        LastError = $"数据库未初始化";
        //        Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
        //        return OperateResult.Failed(LastError);
        //    }

        //    try
        //    {
        //        if (string.IsNullOrEmpty(info))
        //        {
        //            LastError = $"试验待存储数据不可为空";
        //            Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
        //            return OperateResult.Failed(LastError);
        //        }

        //        var data = JsonConvert.DeserializeObject<Test_DataInfo>(info);

        //        if (data == null)
        //        {

        //        }
        //        int row = Sqlite.Insert(data).ExecuteAffrows();

        //        if (row < 1) 
        //        {
        //            LastError = $"试验数据未实际发生存储";
        //            Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
        //            return OperateResult.Failed( LastError);
        //        }

        //        return OperateResult.Succeed();
        //    }
        //    catch (Exception ex)
        //    {
        //        LastError = ex.GetMessage();
        //        Logger.Error(nameof(DBOperate), nameof(InserTestData), LastError);
        //        return OperateResult.Excepted(ex);
        //    }
        //}


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
                    .Where(x => x.TestItemID == itemid && x.CreateTime >= StratTime && StopTime.AddMinutes(1) >= x.CreateTime)
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

        /// <summary>
        /// 试验数据分页查询
        /// </summary>
        /// <param name="itemid">试验条目ID</param>
        /// <param name="StratTime">开始时间</param>
        /// <param name="StopTime">结束时间</param>
        /// <param name="startpage">查询页数</param>
        /// <param name="pagesize">单页行数</param>
        /// <returns></returns>
        public OperateResult<List<Test_DataInfo>> GetLimitTestData(long itemid, DateTime StratTime, DateTime StopTime, int startpage, int pagesize) 
        {
            if (!IsInitOK)
            {
                LastError = $"数据库未初始化";
                Logger.Error(nameof(DBOperate), nameof(GetTestData), LastError);
                return OperateResult<List<Test_DataInfo>>.Failed(null, LastError);
            }
            try
            {
                var items = Sqlite.Select<Test_DataInfo>()
                    .Where(x => x.TestItemID == itemid &&x.CreateTime.BetweenEnd(StratTime, StopTime))
                    .Page(startpage, pagesize)
                    .ToList();
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
        /// 获取试验数据数量
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="StratTime"></param>
        /// <param name="StopTime"></param>
        /// <returns></returns>
        public OperateResult<long> GetTestDataCount(long itemid, DateTime StratTime, DateTime StopTime)
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
                    .Where(x => x.TestItemID == itemid && x.CreateTime.Between(StratTime,StopTime)).Count();
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
