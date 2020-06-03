using Dapper;
using Microsoft.CSharp.RuntimeBinder;
using MySql.Data.MySqlClient;
using SharpNet.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.DB.Dapper
{
    public class DapperFactoryBase
    {
        private string _connectionName;
        private string[] _exceptFields = { "UpdateDate", "CreateDate" };

        //CultureInfo culture = new CultureInfo("en-US");
        public static string serverIP = string.Empty;

        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }
        public DapperFactoryBase(string connectionName)
        {
            ConnectionName = connectionName;
        }
        protected IDbConnection OpenConnection()
        {
            string conStr = string.Empty;

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (settings.ProviderName.Equals(typeof(OleDbConnection).Namespace))
                return new OleDbConnection(settings.ConnectionString);

            if (settings.ProviderName.Equals(typeof(MySqlConnection).Namespace))
            {
                // OrmConfiguration.DefaultDialect = SqlDialect.MySql;
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);

                if (!string.IsNullOrEmpty(serverIP))
                    conStr = settings.ConnectionString.Replace("localhost", serverIP);
                else
                    conStr = settings.ConnectionString;

                return new MySqlConnection(conStr);
            }

            // MS-SQL
            if (!string.IsNullOrEmpty(serverIP))
                conStr = settings.ConnectionString.Replace("PGB-PC", serverIP);
            else
                conStr = settings.ConnectionString;
            return new SqlConnection(conStr);
        }

        #region History

        public IDictionary<string, object> SelectHistoryData(string query)
        {
            using (IDbConnection conn = OpenConnection())
            {
                return (IDictionary<string, object>)conn.Query(query).FirstOrDefault();
            }
        }

        public int InsertHistoryData(string query)
        {
            using (IDbConnection conn = OpenConnection())
            {
                return conn.Execute(query);
            }
        }

        #endregion

        #region sp process

        protected string CreateStoredProcedureSQL<T>(params object[] spParams)
        {
            StringBuilder sb = new StringBuilder();

            var curtype = typeof(T);

            sb.AppendFormat("EXEC {0} ", GetTableName(curtype));

            if (spParams == null)
                return string.Empty;

            if (spParams != null)
            {
                for (int i = 0; i < spParams.Length; i++)
                {
                    if (spParams[i] == null)
                        continue;

                    if (spParams[i].GetType() == typeof(DateTime))
                    {
                        DateTime tmpDate = spParams[i].ConvertTo<DateTime>();

                        if (tmpDate.Year > 1 && tmpDate.Year < 9999)
                        {
                            // string value = spParams[i].ToDateString();

                            string value = tmpDate.ToShortString();
                            sb.AppendFormat("'{0}'", value);
                        }
                    }
                    else if (spParams[i].GetType() == typeof(DateTime?))
                    {
                        DateTime tmpDate = spParams[i].ConvertTo<DateTime>();

                        //if (tmpDate == ServiceContext.Current.NullDate)
                        //    sb.AppendFormat("NULL");
                        //else
                        {
                            string value = tmpDate.ToShortString();
                            sb.AppendFormat("'{0}'", value);
                        }
                    }
                    else if (spParams[i].GetType() == typeof(String))
                    {
                        string tmpVal = spParams[i].ToString();

                        //if (tmpVal == null || tmpVal == ServiceContext.Current.NullString)
                        //    sb.AppendFormat("NULL");
                        ////else if (tmpVal == " ")
                        ////    sb.AppendFormat("''");
                        //else
                        sb.AppendFormat("'{0}'", spParams[i]);
                    }
                    else
                        sb.AppendFormat("'{0}'", spParams[i]);

                    if (i < spParams.Length - 1)
                        sb.Append(", ");
                }

                if (sb.ToString().EndsWith(", "))
                    sb.Remove(sb.Length - 2, 2);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sp 실행결과를 얻는다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="ts">TimeSpan 형식의 시간제한 설정</param>
        /// <param name="spParams">파라메터리스트</param>
        /// <returns>sp 실행결과의 리스트를 돌려준다</returns>
        public IList<T> GetSpDataTimeout<T>(TimeSpan ts, params object[] spParams)
        {
            return GetSpDataTimeout<T>((int)ts.TotalMilliseconds, spParams);
        }

        /// <summary>
        /// Sp 실행결과를 얻는다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="timeout">미리초시간제한 설정 </param>
        /// <param name="spParams">파라메터리스트</param>
        /// <returns>sp 실행결과의 리스트를 돌려준다</returns>
        public IList<T> GetSpDataTimeout<T>(int timeout, params object[] spParams)
        {
            string strSQL = CreateStoredProcedureSQL<T>(spParams);

            using (IDbConnection conn = OpenConnection())
            {
                IList<T> spRetunObject = conn.Query<T>(strSQL, commandTimeout: timeout).ToList();

                return spRetunObject;
            }
        }

        /// <summary>
        /// Sp 실행결과를 얻는다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="spParams">파라메터리스트</param>
        /// <returns>sp 실행결과의 리스트를 돌려준다</returns>
        public IList<T> GetSpData<T>(params object[] spParams)
        {
            string strSQL = CreateStoredProcedureSQL<T>(spParams);

            using (IDbConnection conn = OpenConnection())
            {
                IList<T> spRetunObject = conn.Query<T>(strSQL).ToList();

                return spRetunObject;
            }
        }

        /// <summary>
        /// Sp 를 리용한 Save처리를 진행한다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="spParams">파라메터리스트</param>
        /// <returns>성공 이면 true/ 실패이면 rollback exception 발생</returns>
        public bool SaveSpData<T>(params object[] spParams)
        {
            string strSQL = CreateStoredProcedureSQL<T>(spParams);

            using (IDbConnection conn = OpenConnection())
            {
                conn.Query<T>(strSQL).ToList();
            }

            return true;
        }


        #endregion

        #region SetTable

        /// <summary>
        /// 정적 Query 를 리용한 테이블조작(Update/Insert/Delete)을 진행한다
        /// </summary>
        /// <param name="sqlQuery">전체 SQL Query</param>
        /// <returns>성공이면 true</returns>
        public bool SetTableQueryResult(string sqlQuery)
        {
            using (IDbConnection conn = OpenConnection())
            {
                conn.Execute(sqlQuery);

                return true;
            }
        }

        #endregion

        #region GetTableData

        /// <summary>
        /// 정적 Full Query 를 리용한 Table Data를  호출시 사용
        /// </summary>
        /// <typeparam name="T">테블 클라스</typeparam>
        /// <param name="sqlQuery">정적 Full Sql</param>
        /// <returns>Query 의 Return List</returns>
        protected IList<T> GetTableDataQueryList<T>(string sqlQuery)
        {
            IList<T> data = null;

            using (IDbConnection conn = OpenConnection())
            {
                data = conn.Query<T>(sqlQuery).ToList();

                return data;
            }
        }

        /// <summary>
        /// Condition 에 맞는 Table Data 얻어온다
        /// </summary>
        /// <typeparam name="T">테블 클라스</typeparam>
        /// <param name="whereCondition">Where 문으로 시작되는 Query</param>
        /// <returns></returns>
        public IList<T> GetTableDataList<T>(string whereCondition = null)
        {
            IList<T> data = null;

            try
            {
                using (IDbConnection conn = OpenConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    if (string.IsNullOrEmpty(whereCondition))
                        data = conn.GetList<T>().ToList();
                    else
                        data = conn.GetList<T>(whereCondition).ToList();

                    return data;
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("GetTableDataList : ", ex.ToString(), "DapperFactoryBase");
                return null;
            }
        }

        public T GetTableData<T>(string whereCondition)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var data = conn.GetList<T>(whereCondition).FirstOrDefault();

                return data;
            }
        }


        /// <summary>
        /// 단순 조건문에 맞는 Table data List를 호출
        /// </summary>
        /// <typeparam name="T">테블 클라스</typeparam>
        /// <param name="Condition">단순 Query ex: new {Class = '0001'}</param>  //new {Class = '0001'}
        /// <returns></returns>
        public IList<T> GetTableDataList<T>(object Condition)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var data = conn.GetList<T>(Condition).ToList();

                return data;
            }
        }

        /// <summary>
        /// 단순 조건문에 맞는 Table data를 호출
        /// </summary>
        /// <typeparam name="T">테블 클라스</typeparam>
        /// <param name="Condition">단순 Query ex: new {Class = '0001'}</param>  //new {Class = '0001'}
        /// <returns></returns>
        public T GetTableData<T>(object Condition)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var data = conn.GetList<T>(Condition).FirstOrDefault();

                return data;
            }
        }

        /// <summary>
        /// 단순 조건문에 맞는 Table data를 호출  using FastCRUD
        /// </summary>
        /// <typeparam name="T">테블 클라스</typeparam>
        /// <param name="whereCondition">단순 Query ex: new Asset {Class = '0001'}</param>
        /// dbConnection.Get(new Asset {Id = 10});
        /// <returns></returns>
        public T GetTableData<T>(T whereCondition)
        {
            using (IDbConnection conn = OpenConnection())
            {
                var data = conn.Get<T>(whereCondition);

                return data;
            }
        }

        #endregion

        #region UpdateTable

        /// <summary>
        /// Table에 대한 Update처리를 위한 함수
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="updateObject">테이블오브젝트</param>
        /// <returns>성공이면 true</returns>
        public bool UpdateTableData<T>(Object updateObject)
        {
            string sqlQuery = string.Empty;

            //  updateObject = SetUserID(updateObject);


            try
            {
                using (IDbConnection conn = OpenConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    sqlQuery = CreateUpdateSQL<T>(updateObject);



                    var result = conn.Query<T>(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("UpdateTableData Error :", ex.ToString());

                return false;
            }

            return true;
        }

        /// <summary>
        /// Update를 위한 SQL 문을 생성한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateObject"></param>
        /// <returns></returns>
        private string CreateUpdateSQL<T>(Object updateObject)
        {
            var sb = new StringBuilder();

            // update
            //sb.Append("UPDATE ");
            //sb.Append(GetTableName(updateObject));

            //sb.Append(" SET ");
            //BuildUpdateSet(updateObject, sb);

            //sb.Append(" WHERE ");
            //BuildWhereSet(sb, updateObject);

            //return sb.ToString();

            return updateObject.BuildUpdateQuery();
        }

        #endregion

        #region InsertTable

        /// <summary>
        /// Table 에대한 Insert 처리를 진행한다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="insertObject">오브젝트</param>
        /// <returns></returns>
        public bool InsertTableData<T>(Object insertObject)
        {
            string sqlQuery = string.Empty;

            // insertObject = SetUserID(insertObject);

            try
            {
                using (IDbConnection conn = OpenConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    sqlQuery = CreateInsertSQL<T>(insertObject);



                    var result = conn.Query<T>(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("InsertTableData  ERROR :", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Insert처리를 위한 SQL 문을 생성한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="insertObject"></param>
        /// <returns></returns>
        private string CreateInsertSQL<T>(Object insertObject)
        {
            var sb = new StringBuilder();

            // update
            /*
            sb.Append("INSERT INTO ");
            sb.Append(GetTableName(insertObject));

            sb.Append(Environment.NewLine + " ( ");
            BuildInsertParameters(insertObject, sb);
            sb.Append(") ");

            sb.Append(Environment.NewLine + "VALUES");
            sb.Append(" (");
            BuildInsertValues(insertObject, sb);
            sb.Append(")");

            return sb.ToString();
            */

            return insertObject.BuildInsertQuery();
        }

        #endregion

        #region DeleteTable
        /// <summary>
        /// Table에 대한 Delete처리를 진행한다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="delObject">테블오브젝트</param>
        /// <returns></returns>
        public bool DeleteTableData<T>(object delObject)
        {
            string sqlQuery = string.Empty;

            using (IDbConnection conn = OpenConnection())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                sqlQuery = CreateDeleteSQL(delObject);

                var result = conn.Query<T>(sqlQuery);
            }

            return true;
        }

        /// <summary>
        /// Delete 처리를 위한 SQL문을 구성한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delObject"></param>
        /// <returns></returns>
        private string CreateDeleteSQL(object delObject)
        {
            /*
            var sb = new StringBuilder();

            // delete
            sb.Append("DELETE ");           
            sb.Append(GetTableName(delObject));
            sb.Append(" WHERE ");

            BuildDeleteSet(delObject, sb);
          
            return sb.ToString();
            */

            return delObject.BuildDeleteQuery();
        }

        #endregion

        #region UpsertTableData

        /// <summary>
        /// Table에 대한 Upsert 처리를 진행한다.
        /// </summary>
        /// <typeparam name="T">클라스</typeparam>
        /// <param name="setObject">테블오브젝트</param>
        /// <param name="logTableName">로그테블에 대한 쓰기조작을 동시에 진행할때 로그테블이름을 밝힌다.</param>
        /// <returns></returns>        
        public bool UpsertTableData<T>(Object setObject, string logTableName = null)
        {
            string sqlQuery = string.Empty;

            try
            {
                using (IDbConnection conn = OpenConnection())
                {
                    sqlQuery = CreateMergeSQL<T>(setObject, logTableName);

                    int result = conn.Execute(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("UpsertTableData ERROR :", ex.ToString());

                return false;
            }

            return true;
        }

        public bool UpsertTableDataList<T>(IList<T> list, string logTableName = null)
        {
            using (IDbConnection conn = OpenConnection())
            {
                list.ForEach(x =>
                {
                    try
                    {
                        string query = CreateMergeSQL<T>(x, logTableName);
                        conn.Execute(query);
                    }
                    catch { }
                });
                return true;
            }
        }

        //Merge process with dapper.
        private string CreateMergeSQL<T>(Object mergeObject, string logTableName = null)
        {
            string strMergeSql = string.Empty;

            //  mergeObject = SetUserID(mergeObject);

            var sb = new StringBuilder();

            // log table header.
            if (!String.IsNullOrEmpty(logTableName))
            {
                sb.AppendFormat("INSERT INTO {0}", logTableName);
                sb.Append(Environment.NewLine);
                BuildLogTableHeaderParametes(sb, mergeObject);
                sb.Append(Environment.NewLine);
            }

            // Using SQL
            sb.Append("MERGE ");
            BuildUsingParameters(sb, mergeObject);
            sb.Append(Environment.NewLine + " WHEN MATCHED THEN ");

            //sb.Append(Environment.NewLine + " UPDATE SET ");
            //BuildUpdateSet(mergeObject, sb);

            sb.Append(mergeObject.BuildUpdateQuery(true));


            sb.Append(Environment.NewLine + " WHEN NOT MATCHED THEN ");

            //sb.AppendFormat(Environment.NewLine + "INSERT (");
            //BuildInsertParameters(mergeObject, sb);
            //sb.Append(") ");
            //sb.Append(Environment.NewLine + "VALUES");
            //sb.Append(" (");
            //BuildInsertValues(mergeObject, sb);
            //sb.Append(")");

            sb.Append(mergeObject.BuildInsertQuery(true));

            if (String.IsNullOrEmpty(logTableName))
                sb.Append(";");
            else
            {
                //sb.Append(";");

                sb.Append(Environment.NewLine + "OUTPUT ");
                BuildLogTableBottomParametes(sb, mergeObject);
            }

            return sb.ToString();
        }

        ///// <summary>
        ///// CreateUserID,UpdateUserID 추가한다.
        ///// </summary>
        ///// <param name="mergeObject"></param>
        ///// <returns></returns>
        //private object SetUserID(object mergeObject)
        //{
        //    var propertyInfos = GetScaffoldableProperties(mergeObject).ToArray();

        //    // Set value for CreateUserId, UpdateUserID.
        //    for (var i = 0; i < propertyInfos.Count(); i++)
        //    {
        //        var property = propertyInfos.ElementAt(i);

        //        string columnName = GetColumnName(property);

        //        var colValue = property.GetValue(mergeObject, null);

        //        if (CheckExceptField(columnName))
        //            continue;

        //        if (columnName.ToLower() == "createuserid" || columnName.ToLower() == "updateuserid")
        //            property.SetValue(mergeObject, UserID);
        //    }

        //    return mergeObject;

        //}

        #endregion

        #region Dapper Class Engine

        private bool CheckExceptField(string field)
        {
            var list = from c in _exceptFields
                       where c.ToLower().Equals(field.ToLower())
                       select c;


            if (list.Count() > 0)
                return true;
            else
                return false;
        }

        private void BuildLogTableHeaderParametes(StringBuilder sb, object mergeObject)
        {
            var propertyInfos = GetScaffoldableProperties(mergeObject).ToArray();

            // Select ....
            sb.Append("SELECT ");

            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var property = propertyInfos.ElementAt(i);

                string columnName = GetColumnName(property);

                sb.AppendFormat("{0}", columnName);

                if (i < propertyInfos.Count() - 1)
                    sb.Append(", ");
            }

            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

            // FROM
            sb.Append(Environment.NewLine + "FROM ( ");
        }

        private void BuildLogTableBottomParametes(StringBuilder sb, object mergeObject)
        {
            StringBuilder sbColumns = new StringBuilder();

            var propertyInfos = GetScaffoldableProperties(mergeObject).ToArray();

            //sb.Append("Inserted.*");
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var property = propertyInfos.ElementAt(i);

                string columnName = GetColumnName(property);

                sb.AppendFormat("Inserted.{0}", columnName);
                sbColumns.AppendFormat("{0}, ", columnName);

                if (i < propertyInfos.Count() - 1)
                    sb.Append(", ");
            }

            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

            // AS 
            sb.Append(" )");
            sb.Append(Environment.NewLine + " AS Changes ( ");

            // append real column name again.
            sb.Append(sbColumns);

            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

            sb.Append(" );");
        }


        //build select clause based on list of properties
        private void BuildUsingParameters(StringBuilder sb, object mergeObject)
        {
            var tableName = GetTableName(mergeObject);
            sb.Append(tableName);

            var propertyInfos = GetScaffoldableProperties(mergeObject).ToArray();

            // using ....
            sb.Append(Environment.NewLine + " USING (VALUES (");
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var property = propertyInfos.ElementAt(i);

                string columnName = GetColumnName(property);
                if (CheckExceptField(columnName))
                    continue;

                var colValue = property.GetValue(mergeObject, null);
                if (colValue == null)
                    continue;

                // except auto increasement 
                //if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
                //{
                //    var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
                //    if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                //        continue;
                //}

                if (property.PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)colValue;

                    if (date.Year > 1 && date.Year < 9999)
                        // sb.AppendFormat(culture, "'{0:yyyy-MM-dd HH:mm:ss}'", colValue);
                        sb.AppendFormat("'{0}'", date.ToShortString());
                    else
                        continue;
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    DateTime? proDate = (DateTime?)colValue;

                    //if (proDate == ServiceContext.Current.NullDate)
                    //    sb.AppendFormat("NULL");
                    //else
                    //sb.AppendFormat(culture, "'{0:yyyy-MM-dd HH:mm:ss}'", colValue);
                    sb.AppendFormat("'{0}'", proDate.ConvertTo<DateTime>().ToShortString());
                }
                else if (property.PropertyType == typeof(String))
                {
                    string strTmp = colValue.ToString();

                    //if (strTmp == ServiceContext.Current.NullString)
                    //    sb.AppendFormat("NULL");
                    //else
                    sb.AppendFormat("N'{0}'", colValue); //nvarchar
                }
                else
                    sb.AppendFormat("'{0}'", colValue);

                if (i < propertyInfos.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

            sb.Append(")) ");

            // as ....
            string alias = HasUpperCase(tableName);
            sb.Append(Environment.NewLine + "AS " + alias + " (");
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var property = propertyInfos.ElementAt(i);
                string columnName = GetColumnName(property);
                if (CheckExceptField(columnName))
                    continue;

                var colValue = property.GetValue(mergeObject, null);
                if (colValue == null)
                    continue;

                // auto id 
                //if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
                //{
                //    var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
                //    if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                //        continue;
                //}

                if (property.PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)colValue;

                    if (date.Year == 1)
                        continue;
                }

                sb.Append(columnName);
                //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "ColumnAttribute") != null)
                    sb.Append(" as " + propertyInfos.ElementAt(i).Name);
                if (i < propertyInfos.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
            sb.Append(") ");

            // on...
            sb.Append(Environment.NewLine + "ON ");
            var idProps = GetIdProperties(mergeObject).ToList();

            for (int i = 0; i < idProps.Count(); i++)
            {
                var colValue = idProps[i].GetValue(mergeObject, null);
                if (colValue == null)
                    continue;

                sb.AppendFormat("{0}.{1} = {2}.{1}", tableName, GetColumnName(idProps[i]), alias);
                if (i < idProps.Count() - 1)
                    sb.Append(" AND ");
            }

            if (sb.ToString().EndsWith(" AND "))
                sb.Remove(sb.Length - 4, 4);

        }

        ////build update statement based on list on an entity
        //private void BuildUpdateSet(object entityToUpdate, StringBuilder sb)
        //{
        //    var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();           

        //    for (var i = 0; i < nonIdProps.Length; i++)
        //    {
        //        var property = nonIdProps.ElementAt(i);

        //        // sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
        //        string columnName = GetColumnName(property);
        //        if (CheckExceptField(columnName))
        //            continue;

        //        if (columnName.ToLower().Equals("createuserid"))
        //            continue;

        //        var colValue = property.GetValue(entityToUpdate, null);
        //        if (colValue == null)
        //            continue;


        //        // auto id
        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
        //        {
        //            var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
        //            if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
        //                continue;
        //        }

        //        if (property.PropertyType == typeof(DateTime))
        //        {
        //            DateTime date = (DateTime)colValue;

        //            if (date.Year > 1 && date.Year < 9999)
        //                //sb.AppendFormat(culture, "{0} = '{1:yyyy-MM-dd HH:mm:ss}'", columnName, colValue);
        //                sb.AppendFormat("{0} = '{1}'", columnName, date.ToShortString());
        //            else
        //                continue;
        //        }
        //        else if (property.PropertyType == typeof(DateTime?))
        //        {

        //            DateTime? proDate = (DateTime?)colValue;

        //            //if (proDate == ServiceContext.Current.NullDate)
        //            //    sb.AppendFormat("{0} = NULL", columnName);
        //            //else
        //                // sb.AppendFormat(culture, "{0} = '{1:yyyy-MM-dd HH:mm:ss}'", columnName, colValue);
        //                sb.AppendFormat("{0} = '{1}'", columnName, proDate.ConvertTo<DateTime>().ToShortString());
        //        }
        //        else if (property.PropertyType == typeof(String))
        //        {
        //            string strTmp = colValue.ToString();

        //            //if (strTmp == ServiceContext.Current.NullString)
        //            //    sb.AppendFormat("{0} = NULL", columnName);
        //            //else
        //                sb.AppendFormat("{0} = '{1}'", columnName, colValue);
        //        }
        //        else
        //            sb.AppendFormat("{0} = '{1}'", columnName, colValue);


        //        if (i < nonIdProps.Length - 1)
        //            sb.AppendFormat(", ");

        //        //if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "UpdateDate")) bUpdateField = true;
        //    }
        //    if (sb.ToString().EndsWith(", "))
        //        sb.Remove(sb.Length - 2, 2);          
        //}


        //private void BuildDeleteSet(object entityToDelete, StringBuilder sb)
        //{
        //    var props = GetScaffoldableProperties(entityToDelete).ToArray();

        //    for (var i = 0; i < props.Length; i++)
        //    {
        //        var property = props.ElementAt(i);

        //        // sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
        //        string columnName = GetColumnName(property);
        //        if (CheckExceptField(columnName))
        //            continue;

        //        var colValue = property.GetValue(entityToDelete, null);
        //        if (colValue == null)
        //            continue;

        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
        //        {
        //            var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
        //            if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity && colValue.ToString() == "0")
        //                continue;
        //        }

        //        if (property.PropertyType == typeof(DateTime))
        //        {
        //            DateTime date = (DateTime)colValue;

        //            if (date.Year > 1 && date.Year < 9999)
        //                //sb.AppendFormat(culture, "{0} = '{1:yyyy-MM-dd HH:mm:ss}'", columnName, colValue);
        //                sb.AppendFormat("{0} = '{1}'", columnName, colValue.ToDateString());
        //            else
        //                continue;
        //        }
        //        else if (property.PropertyType == typeof(DateTime?))
        //        {
        //            if (colValue == null)
        //                continue;

        //            DateTime? proDate = (DateTime?)colValue;

        //            if (proDate.Value.Year == 9999 && proDate.Value.Month == 12 && proDate.Value.Day == 31)
        //                sb.AppendFormat("{0} = NULL", columnName);
        //            else
        //                //sb.AppendFormat(culture, "{0} = '{1:yyyy-MM-dd HH:mm:ss}'", columnName, colValue);
        //                sb.AppendFormat("{0} = '{1}'", columnName, colValue.ToDateString());
        //        }
        //        else if (property.PropertyType == typeof(String))
        //        {
        //            string strTmp = colValue.ToString();

        //            //if (strTmp == ServiceContext.Current.NullString)
        //            //    sb.AppendFormat("{0} = NULL", columnName);
        //            //else
        //                sb.AppendFormat("{0} = '{1}'", columnName, colValue);
        //        }
        //        else
        //            sb.AppendFormat("{0} = '{1}'", columnName, colValue);

        //        if (i < props.Length - 1)
        //            sb.AppendFormat(" AND ");
        //    }
        //    if (sb.ToString().EndsWith("AND "))
        //        sb.Remove(sb.Length - 4, 4);
        //}

        ////build where clause based on list of properties
        //private void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object sourceEntity)
        //{
        //    var propertyInfos = idProps.ToArray();
        //    for (var i = 0; i < propertyInfos.Count(); i++)
        //    {
        //        //match up generic properties to source entity properties to allow fetching of the column attribute
        //        //the anonymous object used for search doesn't have the custom attributes attached to them so this allows us to build the correct where clause
        //        //by converting the model type to the database column name via the column attribute
        //        var propertyToUse = propertyInfos.ElementAt(i);
        //        var sourceProperties = GetScaffoldableProperties(sourceEntity).ToArray();
        //        for (var x = 0; x < sourceProperties.Count(); x++)
        //        {
        //            if (sourceProperties.ElementAt(x).Name == propertyInfos.ElementAt(i).Name)
        //            {
        //                propertyToUse = sourceProperties.ElementAt(x);
        //            }
        //        }

        //        sb.AppendFormat("{0} = @{1}", GetColumnName(propertyToUse), propertyInfos.ElementAt(i).Name);
        //        if (i < propertyInfos.Count() - 1)
        //            sb.AppendFormat(" AND ");
        //    }

        //    if (sb.ToString().EndsWith("AND "))
        //        sb.Remove(sb.Length - 4, 4);
        //}

        //private void BuildWhereSet(StringBuilder sb, object sourceEntity)
        //{
        //    // get pk columns.
        //    var propertyInfos = GetIdProperties(sourceEntity).ToArray();

        //    for (var i = 0; i < propertyInfos.Count(); i++)
        //    {
        //        var propertyToUse = propertyInfos.ElementAt(i);
        //        var sourceProperties = GetScaffoldableProperties(sourceEntity).ToArray();
        //        for (var x = 0; x < sourceProperties.Count(); x++)
        //        {
        //            if (sourceProperties.ElementAt(x).Name == propertyInfos.ElementAt(i).Name)
        //            {
        //                propertyToUse = sourceProperties.ElementAt(x);
        //            }
        //        }

        //        var colValue = propertyToUse.GetValue(sourceEntity, null);

        //        if (colValue == null)
        //            continue;

        //        sb.AppendFormat("{0} = '{1}'", GetColumnName(propertyToUse), colValue);

        //        if (i < propertyInfos.Count() - 1)
        //            sb.AppendFormat(" AND ");
        //    }

        //    if (sb.ToString().EndsWith("AND "))
        //        sb.Remove(sb.Length - 4, 4);
        //}

        ////build insert parameters which include all properties in the class that are not marked with the Editable(false) attribute,
        ////are not marked with the [Key] attribute, and are not named Id
        //private void BuildInsertParameters(object entityToInsert, StringBuilder sb)
        //{
        //    var props = GetScaffoldableProperties(entityToInsert).ToArray();

        //    for (var i = 0; i < props.Count(); i++)
        //    {
        //        var property = props.ElementAt(i);
        //        string columnName = GetColumnName(property);
        //        if (CheckExceptField(columnName))
        //            continue;

        //        var colValue = property.GetValue(entityToInsert, null);

        //        if (colValue == null)
        //            continue;

        //        //if (property.PropertyType != typeof(Guid) && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")) continue;
        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "ReadOnlyAttribute" && IsReadOnly(property))) continue;

        //        //auto id
        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
        //        {
        //            var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
        //            if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
        //                continue;
        //        }

        //        sb.Append(columnName);
        //        if (i < props.Count() - 1)
        //            sb.Append(", ");
        //    }
        //    if (sb.ToString().EndsWith(", "))
        //        sb.Remove(sb.Length - 2, 2);
        //}


        ////build insert values which include all properties in the class that are not marked with the Editable(false) attribute,
        ////are not marked with the [Key] attribute, and are not named Id
        //private void BuildInsertValues(object entityToInsert, StringBuilder sb)
        //{
        //    var props = GetScaffoldableProperties(entityToInsert).ToArray();
        //    for (var i = 0; i < props.Count(); i++)
        //    {
        //        var property = props.ElementAt(i);

        //        string columnName = GetColumnName(property);
        //        if (CheckExceptField(columnName))
        //            continue;

        //        var colValue = property.GetValue(entityToInsert, null);
        //        if (colValue == null)
        //            continue;

        //        //if (property.PropertyType != typeof(Guid) && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")) continue;
        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "ReadOnlyAttribute" && IsReadOnly(property))) continue;

        //        //auto id
        //        if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
        //        {
        //            var info = property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
        //            if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
        //                continue;
        //        }

        //        if (property.PropertyType == typeof(DateTime))
        //        {
        //            DateTime date = (DateTime)colValue;

        //            if (date.Year > 1)
        //            // sb.AppendFormat(culture, "'{0:yyyy-MM-dd HH:mm:ss}'", colValue);
        //            {
        //                sb.AppendFormat("'{0}'", date.ToShortString());
        //            }
        //            else
        //                sb.AppendFormat("GetDate()");
        //        }
        //        else if (property.PropertyType == typeof(DateTime?))
        //        {

        //            DateTime? proDate = (DateTime?)colValue;

        //            //if (proDate == ServiceContext.Current.NullDate)
        //            //    sb.AppendFormat("NULL");
        //            //else
        //                // sb.AppendFormat(culture, "'{0:yyyy-MM-dd HH:mm:ss}'", colValue);
        //                sb.AppendFormat("'{0}'", proDate.ConvertTo<DateTime>().ToShortString());
        //        }
        //        else if (property.PropertyType == typeof(String))
        //        {
        //            string strTmp = colValue.ToString();

        //            //if (strTmp == ServiceContext.Current.NullString)
        //            //    sb.AppendFormat("NULL");
        //            //else
        //                sb.AppendFormat("'{0}'", colValue); //nvarchar
        //        }
        //        else
        //            sb.AppendFormat("'{0}'", colValue);

        //        if (i < props.Count() - 1)
        //            sb.Append(", ");
        //    }
        //    if (sb.ToString().EndsWith(", "))
        //        sb.Remove(sb.Length - 2, 2);

        //}


        ////Get all properties that are NOT named Id, DO NOT have the Key attribute, and are not marked ReadOnly
        //private IEnumerable<PropertyInfo> GetUpdateableProperties(object entity)
        //{
        //    var updateableProperties = GetScaffoldableProperties(entity);
        //    //remove ones with ID
        //    updateableProperties = updateableProperties.Where(p => p.Name != "Id");
        //    //remove ones with key attribute
        //    updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute") == false);
        //    //remove ones that are readonly
        //    updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == "ReadOnlyAttribute") && IsReadOnly(p)) == false);

        //    return updateableProperties;
        //}




        //Get all properties that are not decorated with the Editable(false) attribute
        private IEnumerable<PropertyInfo> GetScaffoldableProperties(object entity)
        {
            var props = entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "EditableAttribute" && !IsEditable(p)) == false);
            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        //Get all properties in an entity
        public IEnumerable<PropertyInfo> GetAllProperties(object entity)
        {
            if (entity == null) entity = new { };
            return entity.GetType().GetProperties();
        }


        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimick EditableAttribute in System.ComponentModel.DataAnnotations 
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == "EditableAttribute");
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }


        /// <summary>
        //  Get all properties that are named Id or have the Key attribute
        //  For Inserts and updates we have a whole entity so this method is used
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            var type = entity.GetType();
            return GetIdProperties(type);
        }


        /// <summary>
        //  Get all properties that are named Id or have the Key attribute
        //  For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name == "Id");
        }




        /// <summary>
        /// Gets the table name for this entity
        //  For Inserts and updates we have a whole entity so this method is used
        //  Uses class name by default and overrides if the class has a Table attribute
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string GetTableName(object entity)
        {
            var type = entity.GetType();
            return GetTableName(type);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        protected string GetTableName(Type type)
        {
            //var tableName = String.Format("[{0}]", type.Name);
            var tableName = Encapsulate(type.Name);

            var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic;
            if (tableattr != null)
            {
                //tableName = String.Format("[{0}]", tableattr.Name);
                tableName = Encapsulate(tableattr.Name);
                try
                {
                    if (!String.IsNullOrEmpty(tableattr.Schema))
                    {
                        //tableName = String.Format("[{0}].[{1}]", tableattr.Schema, tableattr.Name);
                        string schemaName = Encapsulate(tableattr.Schema);
                        tableName = String.Format("{0}.{1}", schemaName, tableName);
                    }
                }
                catch (RuntimeBinderException)
                {
                    //Schema doesn't exist on this attribute.
                }
            }

            return tableName;
        }

        /// <summary>
        /// ModelEntity의 Column 명을 얻는다.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private string GetColumnName(PropertyInfo propertyInfo)
        {
            string colName = propertyInfo.Name;

            if (colName.Substring(0, 1) == "_")
                colName = colName.Substring(1, colName.Length - 1);

            var columnName = Encapsulate(colName);

            var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "ColumnAttribute") as dynamic;
            if (columnattr != null)
            {
                columnName = Encapsulate(columnattr.Name);
                // Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
            }
            return columnName;
        }

        private string Encapsulate(string databaseword)
        {
            return string.Format("{0}", databaseword);
        }

        ////Determine if the Attribute has an IsReadOnly key and return its boolean state
        ////fake the funk and try to mimick ReadOnlyAttribute in System.ComponentModel 
        ////This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        //private bool IsReadOnly(PropertyInfo pi)
        //{
        //    var attributes = pi.GetCustomAttributes(false);
        //    if (attributes.Length > 0)
        //    {
        //        dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == "ReadOnlyAttribute");
        //        if (write != null)
        //        {
        //            return write.IsReadOnly;
        //        }
        //    }
        //    return false;
        //}

        //private Guid SequentialGuid()
        //{
        //    var tempGuid = Guid.NewGuid();
        //    var bytes = tempGuid.ToByteArray();
        //    var time = Setting.Current.SystemDate;
        //    bytes[3] = (byte)time.Year;
        //    bytes[2] = (byte)time.Month;
        //    bytes[1] = (byte)time.Day;
        //    bytes[0] = (byte)time.Hour;
        //    bytes[5] = (byte)time.Minute;
        //    bytes[4] = (byte)time.Second;
        //    return new Guid(bytes);
        //}

        private string HasUpperCase(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            //data.Select(x => Char.IsUpper(x) ? String.Concat(" ", x) : x.ToString());
            var result = string.Concat(str.Select(c => char.IsUpper(c) ? c.ToString() : "")).TrimStart();
            return result.ToString().ToLower();
        }

        #endregion
    }
}
