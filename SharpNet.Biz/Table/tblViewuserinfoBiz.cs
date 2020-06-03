
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNet.Biz.Table
{
    public class tblViewuserinfoBiz
    {
        tblViewuserinfoDao phDao = new tblViewuserinfoDao();
        /// </summary>
        public tblViewuserinfoBiz()
        {

        }
        public virtual tblViewuserinfo GetUserInfoByID(int userid)
        {
            return phDao.GetUserInfoByID(userid);
        }

    }
}
