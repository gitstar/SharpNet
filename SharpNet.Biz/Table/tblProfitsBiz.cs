
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNet.Biz.Table
{
    public class tblProfitsBiz
    {
        tblProfitsDao phDao = new tblProfitsDao();
        /// </summary>
        public tblProfitsBiz()
        {

        }
        public virtual bool SetPlayerHistory(tblProfits tblprofits)
        {
            return phDao.SetPlayerHistory(tblprofits);
        }

    }
}
