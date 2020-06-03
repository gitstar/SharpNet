using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharpNet.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Parse
{
    public class NPoiExcel
    {
        public string fileName { get; set; }
        public NPoiExcel(string fName = null)
        {
            if (!fName.IsEmptyOrWhiteSpace())
                fileName = fName;
        }


        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn = false)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            IWorkbook workbook = null;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                workbook.Close();

                return data;
            }
            catch (Exception ex)
            {
                SharpLog.Error("ExcelToDataTable", ex.ToString());
                return null;
            }
        }


        public int CreateDataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            IWorkbook workbook = null;

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook();
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook();

                try
                {
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }

                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    workbook.Write(fs); //写入到excel

                }
                catch (Exception ex)
                {
                    SharpLog.Error("DataTableToExcel", ex.ToString());
                    return -1;
                }
            }

            workbook.Close();

            return count;
        }

        public int InsertDataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            IWorkbook workbook = null;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);

                //for (int i = 0; i < wb.Count; i++)
                //{
                //    comboBox1.Items.Add(wb.GetSheetAt(i).SheetName);
                //}
            }


            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            {
                //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                //    workbook = new XSSFWorkbook();
                //else if (fileName.IndexOf(".xls") > 0) // 2003版本
                //    workbook = new HSSFWorkbook();

                try
                {
                    if (workbook != null)
                    {
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null)
                            sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1 + sheet.LastRowNum;
                    }
                    else
                    {
                        count = sheet.LastRowNum;
                    }

                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    workbook.Write(fs); //写入到excel

                }
                catch (Exception ex)
                {
                    SharpLog.Error("InsertDataTableToExcel", ex.ToString());
                    return -1;
                }
            }

            workbook.Close();
            return count;
        }

        public bool UpdateExcel(DataTable data, string sheetName = null)
        {
            ISheet sheet = null;
            IWorkbook workbook = null;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
            }
            if (workbook == null)
                return false;

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    if (!sheetName.IsEmptyOrWhiteSpace())
                        sheet = workbook.GetSheet(sheetName);
                    else
                        sheet = workbook.GetSheetAt(0);

                    int rowCount = sheet.LastRowNum;
                    for (int i = 0; i < data.Rows.Count; ++i)
                    {
                        int idIndex = data.Rows[i][0].ConvertTo<Int32>();

                        IRow row = sheet.GetRow(idIndex + 1);
                        ICell icell = row.CreateCell(2);
                        icell.SetCellValue(data.Rows[i][1].ToString());
                    }
                    workbook.Write(fs); //写入到excel

                }
                catch (Exception ex)
                {
                    SharpLog.Error("InsertDataTableToExcel", ex.ToString());
                    return false;
                }
                finally
                {
                    workbook.Close();
                }
            }

            return true;
        }
    }
}
