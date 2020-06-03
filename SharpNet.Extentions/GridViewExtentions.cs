using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class GridViewExtentions
{

    public static bool SaveGridToXml(this DataGridView dg, string xmlfilepath, string tableName = null, string dsName = null)
    {
        try
        {
            string path = xmlfilepath; // "C:\\XMLFile1.xml";
            DataSet ds = new DataSet();

            if (dsName.IsNotEmptyOrWhiteSpace())
                ds.DataSetName = dsName;

            DataTable dt = new DataTable();
            if (tableName.IsNotEmptyOrWhiteSpace())
                dt.TableName = tableName;

            //Adding columns to datatable
            foreach (DataGridViewColumn col in dg.Columns)
            {
                dt.Columns.Add(col.DataPropertyName, col.ValueType);
            }
            //adding new rows
            foreach (DataGridViewRow row in dg.Rows)
            {
                // 빈렬은 보관하지 않는다.
                bool isValue = false;
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].Value != null)
                    {
                        isValue = true;
                        break;
                    }
                }

                if (!isValue)
                    continue;

                // 렬을 dt 에 넣기동작
                DataRow row1 = dt.NewRow();
                for (int i = 0; i < dg.ColumnCount; i++)
                    //if value exists add that value else add Null for that field
                    row1[i] = (row.Cells[i].Value == null ? DBNull.Value : row.Cells[i].Value);
                dt.Rows.Add(row1);
            }
            //Copying from datatable to dataset
            ds.Tables.Add(dt);
            //writing new values to XML
            ds.WriteXml(path);
            return true;
        }
        catch
        {
            //SharpLog.Error("SaveGridToXml", ex.ToString());
            return false;
        }
    }

    public static void XmlToGrid(this DataGridView dg, string xmlFileName, string tableName = null)
    {
        DataSet ds = new DataSet();
        //Reading XML file and copying to dataset
        ds.ReadXml(xmlFileName);
        dg.DataSource = ds;

        if (tableName.IsEmptyOrWhiteSpace())
            dg.DataMember = "table1";
        else
            dg.DataMember = tableName;
    }

    public static T ToObject<T>(this DataRow dataRow)  where T : new()
    {
        T item = new T();
        foreach (DataColumn column in dataRow.Table.Columns)
        {
            PropertyInfo property = item.GetType().GetProperty(column.ColumnName);

            if (property != null && dataRow[column] != DBNull.Value)
            {
                object result = Convert.ChangeType(dataRow[column], property.PropertyType);
                property.SetValue(item, result, null);
            }
        }

        return item;
    }

    /// <summary>
    /// Converts a DataTable to a list with generic objects
    /// </summary>
    /// <typeparam name="T">Generic object</typeparam>
    /// <param name="table">DataTable</param>
    /// <returns>List with generic objects</returns>
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }

}