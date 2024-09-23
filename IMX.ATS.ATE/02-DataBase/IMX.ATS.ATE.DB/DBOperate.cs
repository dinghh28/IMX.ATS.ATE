using FreeSql;
using IMX.DB.Model;
using IMX.Logger;
using Super.Zoo.Framework;
using Super.Zoo.Framework.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMX.DB
{
    public class DBOperate : SingletonObject<DBOperate>, ILogRecord
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
#if DEBUG
                sqliteLazy = new Lazy<IFreeSql>(() => new FreeSqlBuilder()
                .UseMonitorCommand(cmd => Trace.WriteLine($"Sql：{cmd.CommandText}"))//监听SQL语句,Trace在输出选项卡中查看
                .UseConnectionString(DataType, ConnectionString)//DataType
                //.UseSlave(SlaveConnectionString)
                .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                //.UseAdoConnectionPool(true)
                .Build());
#else
                           sqliteLazy = new Lazy<IFreeSql>(() => new FreeSqlBuilder()
                          .UseConnectionString(DataType, ConnectionString)
                          .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                          .UseAdoConnectionPool(true)
                          //.UseSlave(SlaveConnectionString)
                          .Build());
#endif
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
                UserInfo item = Sqlite.Select<UserInfo>().Where(x => x.UserName == username && x.Password == psw).ToOne();

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

                long exrow = Sqlite.Select<UserInfo>().Where(x => x.UserName == userInfo.UserName).Count();
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
                int row = Sqlite.Update<UserInfo>(id).Set(x => x.Privilege, privilege).ExecuteAffrows();

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
    }
}
