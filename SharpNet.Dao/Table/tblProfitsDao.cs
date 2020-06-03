using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblProfitsDao : DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblProfitsDao() : base("GameDB")
        {

        }      
        public virtual bool SetPlayerHistory(tblProfits tblprofits)
        {

            bool result = UpsertTableData<tblProfits>(tblprofits);

            return true;
        }

    }
}
