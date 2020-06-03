
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;

namespace SharpNet.Biz.Table
{
    public class tblNoticeBiz 
    {
        tblNoticeDao nDao = new tblNoticeDao();
        /// </summary>
        public tblNoticeBiz()
        {

        }

        public virtual IList<tblNotice> GetNoticeInfoByID(int id)
        {
            return nDao.GetNoticeInfoByID(id);
        }

        public virtual IList<tblNotice> GetAllNoticeInfo()
        {
            return nDao.GetAllNoticeInfo();
        }
        public virtual IList<tblNotice> GetNoticeInfoByTitle(string title)
        {
            return nDao.GetNoticeInfoByTitle(title);
        }

        public virtual IList<tblNotice> GetNoticeInfoByDate(DateTime noticeDate)
        {
            return nDao.GetNoticeInfoByDate(noticeDate);
        }

        public virtual bool SetNoticeInfo(tblNotice tbliteminfo)
        {
            return nDao.SetNoticeInfo(tbliteminfo);
        }

        public virtual bool DeleteNoticeInfo(tblNotice tbliteminfo)
        {
            return nDao.DeleteNoticeInfo(tbliteminfo);
        }
    }
}
