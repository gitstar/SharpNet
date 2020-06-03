using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblLoginHistoryDao :DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblLoginHistoryDao() : base("GameDB")
        {

        }

        public virtual IList<tblLoginHistory> GetLoginHistoryByUserID(int userid)
        {
            sqlQuery = string.Format("WHERE UserID = '{0}'", userid);

            IList<tblLoginHistory> value = GetTableDataList<tblLoginHistory>(sqlQuery);

            return value;
        }

        public virtual IList<tblLoginHistory> GetLogintHistoryByDate(DateTime joinDate)
        {
            sqlQuery = string.Format("WHERE JoinDate = '{0}' AND UseType = 'Y'", joinDate);

            IList<tblLoginHistory> value = GetTableDataList<tblLoginHistory>(sqlQuery);

            return value;
        }

        public virtual bool SetLoginHistory(tblLoginHistory tblloginhistory)
        {
            InsertTableData<tblLoginHistory>(tblloginhistory);
            return true;
        }

    }
}
