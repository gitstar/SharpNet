
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;

namespace SharpNet.Biz.Table
{
    public class tblLoginHistoryBiz
    {
        tblLoginHistoryDao lhDao = new tblLoginHistoryDao();
        /// </summary>
        public tblLoginHistoryBiz() 
        {

        }

        public virtual IList<tblLoginHistory> GetLoginHistoryByUserID(int userid)
        {
            return lhDao.GetLoginHistoryByUserID(userid);
        }

        public virtual IList<tblLoginHistory> GetLogintHistoryByDate(DateTime joinDate)
        {
            return lhDao.GetLogintHistoryByDate(joinDate);
        }

        public virtual bool SetLoginHistory(tblLoginHistory tblloginhistory)
        {
            return lhDao.SetLoginHistory(tblloginhistory);
        }

    }
}
