using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblViewuserinfoDao : DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblViewuserinfoDao() : base("GameDB")
        {

        }

        public virtual tblViewuserinfo GetUserInfoByID(int userid)
        {
            sqlQuery = string.Format("WHERE userId = N'{0}'", userid);

            tblViewuserinfo value = GetTableData<tblViewuserinfo>(sqlQuery);

            return value;
        }
    }
}
