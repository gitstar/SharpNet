using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblNoticeDao : DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblNoticeDao() : base("GameDB")
        {

        }

        public virtual IList<tblNotice> GetNoticeInfoByID(int id)
        {
            IList<tblNotice> value = GetTableDataList<tblNotice>(new { ID = id });

            return value;
        }

        public virtual IList<tblNotice> GetAllNoticeInfo()
        {
            sqlQuery = string.Format("WHERE UseType = 'Y'");

            IList<tblNotice> valueList = GetTableDataList<tblNotice>(sqlQuery);

            return valueList;
        }

        public virtual IList<tblNotice> GetNoticeInfoByTitle(string title)
        {
            sqlQuery = string.Format("WHERE Title = N'{0}' AND UseType = 'Y'", title);

            IList<tblNotice> valueList = GetTableDataList<tblNotice>(sqlQuery);

            return valueList;
        }

        public virtual IList<tblNotice> GetNoticeInfoByDate(DateTime noticeDate)
        {
            sqlQuery = string.Format("WHERE StartDate <= '{0}' AND EndDate >= '{0}' AND  UseType = 'Y'", noticeDate);

            IList<tblNotice> value = GetTableDataList<tblNotice>(sqlQuery);

            return value;
        }

        public virtual bool SetNoticeInfo(tblNotice tbliteminfo)
        {
            UpsertTableData<tblNotice>(tbliteminfo);

            return true;
        }

        public virtual bool DeleteNoticeInfo(tblNotice tbliteminfo)
        {
            DeleteTableData<tblNotice>(tbliteminfo);

            return true;
        }
    }
}
