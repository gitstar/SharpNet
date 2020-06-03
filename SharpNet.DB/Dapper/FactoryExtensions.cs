using DbExtensions;
using SharpNet.Set;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.DB.Dapper
{
    public static class FactoryExtensions
    {
        private static string[] _excludeFields = { "UpdateDate", "CreateDate" };

        public static string BuildInsertQuery(this object obj, bool isMergeQuery = false)
        {
            var query = new SqlBuilder();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> properties = obj.GetProperties();

            properties.SetProperty(obj, "createdate", Setting.Current.SystemDate);
            properties.SetProperty(obj, "updatedate", Setting.Current.SystemDate);

            foreach (PropertyInfo p in properties)
            {
                object value = p.GetValue(obj);

                string name = p.Name;

                if (value == null)
                    continue;

                if (p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "ReadOnlyAttribute" && p.IsReadOnly()))
                    continue;

                if (p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
                {
                    var info = p.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
                    if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                        continue;
                }

                if (p.ColumnName().ToLower().Equals("createdate") || p.ColumnName().ToLower().Equals("updatedate"))
                    dic.Add(p.ColumnName(), "GETDATE()");
                else if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    DateTime date = p.GetValue(obj).ConvertTo<DateTime>();

                    bool condition = date.Year > 1 && date.Year < 9999;
                    dic.Add(p.ColumnName(), date.ToShortString());
                }
                else if (p.PropertyType.Equals(typeof(DateTime?)))
                {
                    DateTime? date = (DateTime?)p.GetValue(obj);
                    if (!date.HasValue)
                        continue;

                    if (date.Value == Setting.Current.NullDate)
                        dic.Add(p.ColumnName(), "NULL");
                    else
                        dic.Add(p.ColumnName(), date.ToShortString());
                }
                else if (p.PropertyType.Equals(typeof(string)) || p.PropertyType.Equals(typeof(char)) || p.PropertyType.Equals(typeof(Guid)))
                {
                    string data = value.ConvertTo<string>();

                    if (data == Setting.Current.NullString)
                        dic.Add(p.ColumnName(), "NULL");
                    else
                        dic.Add(p.ColumnName(), data);
                }
                else if (p.PropertyType.IsValueType)
                {
                    dic.Add(p.ColumnName(), value);
                }
                else
                    dic.Add(p.ColumnName(), p.GetValue(obj));
            }

            var queryString = SQL.INSERT_INTO(string.Format("{0} ({1})", isMergeQuery ? string.Empty : obj.GetType().GetTableName(), dic.Keys.Join(",")))
                                .VALUES(string.Empty).JoinSetValues(dic.Values.ToArray());

            if (isMergeQuery)
                queryString = queryString.Replace("INSERT INTO", "INSERT");

            return queryString;
        }

        public static string BuildUpdateQuery(this object obj, bool isMergeQuery = false)
        {
            var builder = SQL.UPDATE(isMergeQuery ? " " : obj.GetType().GetTableName());


            Dictionary<string, object> dic = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> properties = obj.GetProperties();

            PropertyInfo pInfo = properties.FirstOrDefault(c => c.Name.ToLower().Equals("updatedate"));
            if (pInfo != null)
                pInfo.SetValue(obj, Setting.Current.SystemDate);

            IEnumerable<PropertyInfo> primaryKeys = obj.GetType().GetIdProperties();

            foreach (PropertyInfo p in properties)
            {
                object value = p.GetValue(obj);

                if (value == null || p.ColumnName().ToLower().Equals("createuserid") || p.ColumnName().ToLower().Equals("createdate") || primaryKeys.Contains(p))
                    continue;

                if (p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
                {
                    var info = p.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
                    if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                        continue;
                }

                if (p.ColumnName().ToLower().Equals("updatedate"))
                {
                    builder.SET(string.Format("{0} = GETDATE()", p.ColumnName()));
                }
                else if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    DateTime date = p.GetValue(obj).ConvertTo<DateTime>();

                    bool condition = date.Year > 1 && date.Year < 9999;

                    if (!condition)
                        continue;

                    builder.SET(string.Format("{0} = '{1}'", p.ColumnName(), date.ToShortString()));
                }
                else if (p.PropertyType.Equals(typeof(DateTime?)))
                {
                    DateTime? date = (DateTime?)p.GetValue(obj);
                    if (!date.HasValue)
                        continue;

                    if (date.Value == Setting.Current.NullDate)
                        builder.SET(string.Format("{0} = NULL", p.ColumnName()));
                    else
                    {
                        DateTime nDate = date.ConvertTo<DateTime>();

                        bool condition = nDate.Year > 1 && nDate.Year < 9999;
                        if (!condition)
                            continue;

                        builder.SET(string.Format("{0} = '{1}'", p.ColumnName(), date.ToShortString()));
                    }

                }
                else if (p.PropertyType.Equals(typeof(string)) || p.PropertyType.Equals(typeof(char)) || p.PropertyType.Equals(typeof(Guid)))
                {
                    string data = value.ConvertTo<string>();

                    if (data == Setting.Current.NullString)
                        builder.SET(string.Format("{0} = NULL", p.ColumnName()));
                    else
                        builder.SET(string.Format("{0} = N'{1}'", p.ColumnName(), data));
                }
                else if (p.PropertyType.IsValueType)
                {
                    builder.SET(string.Format("{0} = {1}", p.ColumnName(), value));
                }
            }

            if (!isMergeQuery)
                builder.BuildWhereQuery(obj, obj.GetType().GetIdProperties());


            return builder.ToString();
        }

        public static string BuildDeleteQuery(this object obj)
        {
            var builder = SQL.DELETE_FROM(obj.GetType().GetTableName()).BuildWhereQuery(obj, obj.GetProperties());

            return builder.ToString();
        }

        private static SqlBuilder BuildWhereQuery(this SqlBuilder builder, object obj, IEnumerable<PropertyInfo> properties)
        {
            builder.Append(" ");
            builder.WHERE();

            foreach (PropertyInfo p in properties)
            {
                object value = p.GetValue(obj);

                if (value == null || _excludeFields.Contains(p.ColumnName()))
                    continue;

                if (p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "DatabaseGeneratedAttribute"))
                {
                    var info = p.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>();
                    if (info.Single().DatabaseGeneratedOption == DatabaseGeneratedOption.Identity && value.ConvertTo<Int32>() == 0)
                        continue;
                }

                if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    DateTime date = p.GetValue(obj).ConvertTo<DateTime>();

                    bool condition = date.Year > 1 && date.Year < 9999;
                    builder._If(condition, string.Format("{0} = '{1}'", p.ColumnName(), date.ToShortString()));
                }
                else if (p.PropertyType.Equals(typeof(DateTime?)))
                {
                    DateTime? date = (DateTime?)p.GetValue(obj);
                    if (!date.HasValue)
                        continue;

                    bool condition = date.Value.Year == 9999 && date.Value.Month == 12 && date.Value.Day == 31;
                    builder._If(!condition, string.Format("{0} = '{1}'", p.ColumnName(), date.ToShortString()));
                }
                else if (p.PropertyType.Equals(typeof(string)) || p.PropertyType.Equals(typeof(char)) || p.PropertyType.Equals(typeof(Guid)))
                {
                    builder._If(value.ConvertTo<string>().IsNotEmptyOrWhiteSpace(), string.Format("{0} = '{1}'", p.ColumnName(), value));
                }
                else if (p.PropertyType.IsValueType)
                {
                    builder._If(true, string.Format("{0} = {1}", p.ColumnName(), value));
                }
            }
            return builder;
        }

        private static bool IsEditable(this PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == "EditableAttribute");
                if (write != null)
                    return write.AllowEdit;
            }
            return false;
        }

        private static string ColumnName(this PropertyInfo propertyInfo)
        {
            string colName = propertyInfo.Name;

            if (colName.Substring(0, 1) == "_")
                colName = colName.Substring(1, colName.Length - 1);

            var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "ColumnAttribute") as dynamic;
            if (columnattr != null)
                colName = columnattr.Name;

            return colName;
        }

        private static string GetTableName(this Type type)
        {
            var tableName = type.Name;

            var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic;
            if (tableattr != null)
            {
                if (tableattr.Schema != null)
                {
                    string schemaName = tableattr.Schema;
                    tableName = string.Format("{0}.{1}", schemaName, tableattr.Name);
                }
            }

            // for mysql reeldb.
            //string preName = tableName.Substring(0, 2);
            //string fullName = tableName.Substring(2, tableName.Length - 2);
            //tableName = preName + "_" + fullName;

            return tableName + " ";
        }

        //private static void SetBulderParameter(object obj, PropertyInfo p, Action action)
        //{
        //    if (p.PropertyType.Equals(typeof(DateTime)))
        //    {
        //        DateTime date = p.GetValue(obj).ConvertTo<DateTime>();

        //        bool condition = date.Year > 1 && date.Year < 9999;
        //        action();
        //    }
        //    else if (p.PropertyType.Equals(typeof(DateTime?)))
        //    {
        //        DateTime? date = (DateTime?)p.GetValue(obj);
        //        if (!date.HasValue)
        //            return;

        //        bool condition = date.Value.Year == 9999 && date.Value.Month == 12 && date.Value.Day == 31;
        //        action();
        //    }
        //    else if (p.PropertyType.Equals(typeof(string)) || p.PropertyType.Equals(typeof(char)))
        //    {
        //        string value = p.GetValue(obj).ConvertTo<string>();
        //        if (value.IsNotEmptyOrWhiteSpace())
        //            action();
        //    }
        //    else if (p.PropertyType.IsValueType)
        //    {
        //        action();
        //    }
        //}

        private static bool IsReadOnly(this PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == "ReadOnlyAttribute");
                if (write != null)
                    return write.IsReadOnly;
            }
            return false;
        }

        private static string JoinSetValues(this SqlBuilder builder, IEnumerable<object> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object x in list)
            {
                Type type = x.GetType();
                if (x.ConvertTo<string>().Equals("GETDATE()"))
                    sb.Append(x.ToString() + ",");
                else if (type.IsValueType && !type.Equals(typeof(DateTime)) && !type.Equals(typeof(DateTime?)) && !type.Equals(typeof(Guid)))
                    sb.Append(x.ToString() + ",");
                else
                    sb.AppendFormat("N'{0}',", x);
            }

            string value = (sb.Length > 1) ? sb.Remove(sb.Length - 1, 1).ToString() : String.Empty;
            return string.Format(builder.ToString(), value);
        }

        private static IEnumerable<PropertyInfo> GetIdProperties(this Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute")).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.ColumnName() != "AutoID");
        }

        private static IEnumerable<PropertyInfo> GetProperties(this object obj)
        {
            var props = obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "EditableAttribute" && !p.IsEditable()) == false);
            return props.Where(p => p.PropertyType.IsSimpleType() || p.IsEditable());
        }

        private static void SetProperty(this IEnumerable<PropertyInfo> properties, object obj, string columnName, object value)
        {
            PropertyInfo pInfo1 = properties.FirstOrDefault(c => c.Name.ToLower().Equals(columnName.ToLower()));
            if (pInfo1 != null)
                pInfo1.SetValue(obj, value);
        }
    }
}
