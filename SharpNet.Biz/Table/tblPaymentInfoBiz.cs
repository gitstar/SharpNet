
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System.Collections.Generic;

namespace SharpNet.Biz.Table
{
    public class tblPaymentInfoBiz
    {
        tblPaymentInfoDao ciDao = new tblPaymentInfoDao();

        /// </summary>
        public tblPaymentInfoBiz() 
        {

        }
        public virtual bool SetItemChargeInfo(tblPaymentInfo tblpaymentInfo)
        {
            ciDao.SetChargeInfo(tblpaymentInfo);

            return true;
        }
    }
}
