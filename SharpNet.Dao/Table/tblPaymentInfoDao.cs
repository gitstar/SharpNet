using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblPaymentInfoDao : DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblPaymentInfoDao() : base("GameDB")
        {

        }

        public virtual tblPaymentInfo GetChargeInfoByID(int userid)
        {
            sqlQuery = string.Format("WHERE userId = N'{0}'", userid);
            tblPaymentInfo value = GetTableData<tblPaymentInfo>(sqlQuery);

            return value;
        }

        public virtual bool SetChargeInfo(tblPaymentInfo tblpaymentInfo)
        {
            UpsertTableData<tblPaymentInfo>(tblpaymentInfo);

            return true;
        }
    }
}
