using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JinianNet.JNTemplate.Demo
{

    public abstract class DbHelper
    {
        public void SetTimeoutDefault()
        {
            Timeout = 30;
        }
        private int Timeout = 30;

        public string ConnectionString { get; set; }

        public DbConnection Connection { get; protected set; }

        public DbHelper()
            : this(null)
        {

        }

        public DbHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected abstract DbConnection CreateConnection();
        protected abstract DbCommand CreateCommand();
        protected abstract DbDataAdapter CreateDataAdapter();
        protected abstract DbParameter CreateParameter();

        public int ExecuteNonQuery(string cmdText, params object[] commandParameters)
        {
            if (commandParameters != null && commandParameters.Length > 0)
            {
                cmdText = ParseCommandText(cmdText, commandParameters);
                return ExecuteNonQuery(CommandType.Text, cmdText, PrepareParameter(commandParameters));
            }
            else
            {
                return ExecuteNonQuery(CommandType.Text, cmdText, null);
            }
        }
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(this.ConnectionString, cmdType, cmdText, commandParameters);
        }

        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {

            DbCommand cmd = CreateCommand();

            using (DbConnection conn = CreateConnection())
            {
                try
                {
                    conn.ConnectionString = connectionString;
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    //this.Connection= conn;
                    return val;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();

                    }
                    conn.Dispose();
                }
            }
        }

        public int ExecuteNonQuery(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {

            DbCommand cmd = CreateCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            this.Connection = connection;
            return val;
        }

        public int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            this.Connection = trans.Connection;
            return val;
        }


        public DbDataReader ExecuteReader(string cmdText, params object[] commandParameters)
        {
            if (commandParameters != null && commandParameters.Length > 0)
            {
                cmdText = ParseCommandText(cmdText, commandParameters);
                return ExecuteReader(CommandType.Text, cmdText, PrepareParameter(commandParameters));
            }
            else
            {
                return ExecuteReader(CommandType.Text, cmdText, null);
            }
        }

        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteReader(this.ConnectionString, cmdType, cmdText, commandParameters);
        }

        public DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();
            DbConnection conn = CreateConnection();
            conn.ConnectionString = connectionString;

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                this.Connection = conn;
                return rdr;
            }
            catch
            {

                throw;
            }
            finally
            {
                //conn.Close();
                //conn.Dispose();
                //conn = null;
            }
        }

        public DbDataReader ExecuteReader(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            //this.Connection = connection;
            return rdr;
        }

        public object ExecuteScalar(string cmdText, params object[] commandParameters)
        {
            if (commandParameters != null && commandParameters.Length > 0)
            {
                cmdText = ParseCommandText(cmdText, commandParameters);
                return ExecuteScalar(CommandType.Text, cmdText, PrepareParameter(commandParameters));
            }
            else
            {
                return ExecuteScalar(CommandType.Text, cmdText, null);
            }
        }

        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(this.ConnectionString, cmdType, cmdText, commandParameters);
        }

        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();

            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    //this.Connection = connection;
                    return val;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        public object ExecuteScalar(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {

            DbCommand cmd = CreateCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            this.Connection = connection;
            return val;
        }

        public object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            this.Connection = trans.Connection;
            return val;
        }

        public DataTable ExecuteTable(string cmdText, params object[] commandParameters)
        {
            if (commandParameters != null && commandParameters.Length > 0)
            {
                cmdText = ParseCommandText(cmdText, commandParameters);
                return ExecuteTable(CommandType.Text, cmdText, PrepareParameter(commandParameters));
            }
            else
            {
                return ExecuteTable(CommandType.Text, cmdText, null);
            }
        }

        public DataTable ExecuteTable(CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            return ExecuteTable(this.ConnectionString, cmdType, cmdText, commandParameters);
        }

        public DataTable ExecuteTable(string connectionString, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();

            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    DbDataAdapter ap = CreateDataAdapter();
                    ap.SelectCommand = cmd;
                    DataSet st = new DataSet();
                    ap.Fill(st, "Result");
                    cmd.Parameters.Clear();
                    return st.Tables["Result"];
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
        }

        public DataTable ExecuteTable(DbConnection connection, CommandType cmdType, string cmdText, params DbParameter[] commandParameters)
        {

            DbCommand cmd = CreateCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            DbDataAdapter ap = CreateDataAdapter();
            ap.SelectCommand = cmd;
            DataSet st = new DataSet();
            ap.Fill(st, "Result");
            cmd.Parameters.Clear();
            this.Connection = connection;
            return st.Tables["Result"];
        }

        public int ExecuteTransaction(string[] cmdTexts)
        {
            return ExecuteTransaction(new TransactionCollection(cmdTexts));
        }

        public int ExecuteTransaction(TransactionCollection trans)
        {
            return ExecuteTransaction(this.ConnectionString, trans);
        }

        public int ExecuteTransaction(string connectionString, TransactionCollection trans)
        {

            DbCommand cmd = CreateCommand();

            using (DbConnection conn = CreateConnection())
            {
                try
                {
                    conn.ConnectionString = connectionString;
                    //PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    int val = ExecuteTransaction(conn, trans);
                    cmd.Parameters.Clear();
                    //this.Connection= conn;
                    return val;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();

                    }
                    conn.Dispose();
                }
            }
        }

        public int ExecuteTransaction(DbConnection connection, TransactionCollection trans)
        {
            int val = 0;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (DbTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < trans.Count; i++)
                    {
                        val += ExecuteNonQuery(transaction, trans[i].CommandType, trans[i].Text, trans[i].Parameter);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return val;
        }

        public DbDataReader ExecuteReaderPage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string GroupClause, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            DbConnection conn = CreateConnection();
            conn.ConnectionString = connectionString;
            try
            {
                conn.Open();
                DbCommand cmd = CreateCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, "", commandParameters);
                string Sql = GetPageSql(conn, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out  RecordCount, out  PageCount);
                if (GroupClause != null && GroupClause.Trim() != "")
                {
                    int n = Sql.ToLower().LastIndexOf(" order by ");
                    Sql = Sql.Substring(0, n) + " " + GroupClause + " " + Sql.Substring(n);
                }
                cmd.CommandText = Sql;
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                //this.Connection = conn;
                return rdr;
            }
            catch
            {
                //if (conn.State == ConnectionState.Open)
                //    conn.Close();
                throw;
            }
            finally
            {
                //if (conn.State == ConnectionState.Open)
                //    conn.Close();
                //conn.Dispose();
                //conn = null;
            }
        }
        public DbDataReader ExecuteReaderPage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string GroupClause, string OrderFields, int PageIndex, int PageSize, params DbParameter[] commandParameters)
        {
            DbConnection conn = CreateConnection();
            conn.ConnectionString = connectionString;
            try
            {
                conn.Open();
                DbCommand cmd = CreateCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, "", commandParameters);
                string Sql = GetPageSql(conn, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize);
                if (GroupClause != null && GroupClause.Trim() != "")
                {
                    int n = Sql.ToLower().LastIndexOf(" order by ");
                    Sql = Sql.Substring(0, n) + " " + GroupClause + " " + Sql.Substring(n);
                }
                cmd.CommandText = Sql;
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                //this.Connection = conn;
                return rdr;
            }
            catch
            {
                //if (conn.State == ConnectionState.Open)
                //    conn.Close();
                throw;
            }
            finally
            {
                //if (conn.State == ConnectionState.Open)
                //    conn.Close();
                //conn.Dispose();
                //conn = null;
            }
        }

        public DataTable ExecutePage(string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            return ExecutePage(this.ConnectionString, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out  RecordCount, out  PageCount, commandParameters);
        }
        public DataTable ExecutePage(string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, params DbParameter[] commandParameters)
        {
            return ExecutePage(this.ConnectionString, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, commandParameters);
        }

        public DataTable ExecutePage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    //this.Connection= connection;
                    return ExecutePage(connection, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out RecordCount, out PageCount, commandParameters);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        public DataTable ExecutePage(string connectionString, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, params DbParameter[] commandParameters)
        {
            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    //this.Connection= connection;
                    return ExecutePage(connection, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, commandParameters);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        public DataTable ExecutePage(DbConnection connection, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();
            PrepareCommand(cmd, connection, null, CommandType.Text, "", commandParameters);
            string Sql = GetPageSql(connection, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize, out  RecordCount, out  PageCount);
            cmd.CommandText = Sql;
            DbDataAdapter ap = CreateDataAdapter();
            ap.SelectCommand = cmd;
            DataSet st = new DataSet();
            ap.Fill(st, "PageResult");
            cmd.Parameters.Clear();
            this.Connection = connection;
            return st.Tables["PageResult"];
        }
        public DataTable ExecutePage(DbConnection connection, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, params DbParameter[] commandParameters)
        {
            DbCommand cmd = CreateCommand();
            PrepareCommand(cmd, connection, null, CommandType.Text, "", commandParameters);
            string Sql = GetPageSql(connection, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize);
            cmd.CommandText = Sql;
            DbDataAdapter ap = CreateDataAdapter();
            ap.SelectCommand = cmd;
            DataSet st = new DataSet();
            ap.Fill(st, "PageResult");
            cmd.Parameters.Clear();
            this.Connection = connection;
            return st.Tables["PageResult"];
        }


        public string GetPageSql(DbConnection connection, DbCommand cmd, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize, out int RecordCount, out int PageCount)
        {
            RecordCount = 0;
            PageCount = 0;
            if (PageSize <= 0)
            {
                PageSize = 10;
            }
            string SqlCount = string.Format("select count({0}) from {1}", IndexField, SqlTablesAndWhere);
            cmd.CommandText = SqlCount;
            RecordCount = Convert.ToInt32(cmd.ExecuteScalar());
            if (RecordCount % PageSize == 0)
            {
                PageCount = RecordCount / PageSize;
            }
            else
            {
                PageCount = RecordCount / PageSize + 1;
            }
            if (PageIndex > PageCount)
                PageIndex = PageCount;
            if (PageIndex < 1)
                PageIndex = 1;
            return GetPageSql(connection, cmd, SqlAllFields, SqlTablesAndWhere, IndexField, OrderFields, PageIndex, PageSize);
        }

        public abstract string GetPageSql(DbConnection connection, DbCommand cmd, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize);

        public void CloseConnection()
        {

            if (this.Connection != null)
            {
                if (this.Connection.State == ConnectionState.Open)
                {
                    this.Connection.Close();
                }
            }
        }

        private void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;
            cmd.CommandTimeout = Timeout;
            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                {
                    if (parm != null)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
            }
        }

        public DbParameter[] PrepareParameter(object[] parameters)
        {
            DbParameter[] dbparameters = new DbParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                dbparameters[i] = CreateParameter();
                if (parameters[i] != null)
                {
                    dbparameters[i].DbType = PrepareDbType(parameters[i].GetType());
                }
                dbparameters[i].ParameterName = string.Concat("@Param", i.ToString());
                dbparameters[i].Value = parameters[i];
            }
            return dbparameters;

        }

        public string ParseCommandText(string cmdText, object[] parameters)
        {
            if (parameters.Length > 0)
            {
                if (cmdText.Contains("{...}"))
                {
                    string[] array = new string[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        array[i] = string.Concat("@Param", i.ToString());
                    }
                    cmdText = cmdText.Replace("{...}", string.Join(",", array));
                }
            }
            if (cmdText.IndexOf('{') != -1)
            {
                cmdText = Regex.Replace(cmdText, @"\{(\d+)\}", "@Param$1");
            }
            return cmdText;
        }

        public DbType PrepareDbType(Type type)
        {
            switch (type.FullName)
            {
                case "System.String":
                    return DbType.String;
                case "System.Byte[]":
                    return DbType.Binary;
                case "System.Byte":
                    return DbType.Byte;
                case "System.Boolean":
                    return DbType.Boolean;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Decimal":
                    return DbType.Decimal;
                case "System.Double":
                    return DbType.Double;
                case "System.Guid":
                    return DbType.Guid;
                case "System.Int16":
                    return DbType.Int16;
                case "System.Int32":
                    return DbType.Int32;
                case "System.Int64":
                    return DbType.Int64;
                case "System.SByte":
                    return DbType.SByte;
                case "System.Single":
                    return DbType.Single;
                case "System.UInt16":
                    return DbType.UInt16;
                case "System.UInt32":
                    return DbType.UInt32;
                case "System.UInt64":
                    return DbType.UInt64;
                default:
                    return DbType.Object;
            }

        }
    }

    public class Transaction
    {
        public CommandType CommandType { get; set; }
        public string Text { get; set; }
        public DbParameter[] Parameter { get; set; }
    }

    public class TransactionCollection : List<Transaction>
    {
        public TransactionCollection()
            : base()
        {
        }

        public TransactionCollection(int capacity)
            : base(capacity)
        {
        }

        public TransactionCollection(IEnumerable<Transaction> collection)
            : base(collection)
        {

        }

        public TransactionCollection(string[] collection)
            : this(collection.Length)
        {
            this.AddRange(collection);
        }

        public void Add(string item)
        {
            this.Add(new Transaction()
            {
                CommandType = CommandType.Text,
                Text = item,
                Parameter = null
            });
        }

        public void Add(CommandType commandType, string cmdText)
        {
            this.Add(new Transaction()
            {
                CommandType = commandType,
                Text = cmdText,
                Parameter = null
            });
        }

        public void AddRange(string[] collection)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                if (!string.IsNullOrEmpty(collection[i]))
                {
                    this.Add(collection[i]);
                }
            }
        }
    }


    public class SQLiteHelper : DbHelper
    {
        public SQLiteHelper()
            : base(System.Configuration.ConfigurationManager.ConnectionStrings["connection"].ConnectionString)
        {

        }

        public SQLiteHelper(string connectionString)
            : base(connectionString)
        {

        }

        protected override System.Data.Common.DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }

        protected override System.Data.Common.DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        protected override System.Data.Common.DbDataAdapter CreateDataAdapter()
        {
            return new SQLiteDataAdapter();
        }

        protected override System.Data.Common.DbParameter CreateParameter()
        {
            return new SQLiteParameter();
        }

        public override string GetPageSql(System.Data.Common.DbConnection connection, System.Data.Common.DbCommand cmd, string SqlAllFields, string SqlTablesAndWhere, string IndexField, string OrderFields, int PageIndex, int PageSize)
        {
            if (PageSize <= 0)
            {
                PageSize = 10;
            }
            if (PageIndex < 1)
                PageIndex = 1;
            StringBuilder Sql = new StringBuilder();
            Sql.Append("select ");
            Sql.Append(SqlAllFields);
            Sql.Append(" from ");
            if (PageIndex == 1)
            {
                Sql.Append(SqlTablesAndWhere);
                Sql.Append(" ");
                Sql.Append(OrderFields);
                Sql.Append(" Limit ");
                Sql.Append(PageSize);

            }
            else
            {
                Sql.Append(SqlTablesAndWhere);
                Sql.Append(" ");
                Sql.Append(OrderFields);
                Sql.Append(" Limit ");
                Sql.Append((PageIndex - 1) * PageSize);
                Sql.Append(" , ");
                Sql.Append(PageSize);
            }
            return Sql.ToString();
        }
    }
}