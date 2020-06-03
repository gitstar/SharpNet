using SharpNet.DB.Dapper;
using SharpNet.DB.Table;
using System.Collections.Generic;

namespace SharpNet.Dao.Table
{
    public class tblUserInfoDao : DapperFactoryBase
    {
        string sqlQuery = string.Empty;
        /// </summary>
        public tblUserInfoDao() : base("GameDB")
        {

        }

        public virtual tbl_users GetUserInfoByID(int userid)
        {
            tbl_users value = GetTableData<tbl_users>(new { userId = userid });

            return value;
        }

        public virtual IList<tbl_users> GetAllUserInfo()
        {
            IList<tbl_users> valueList = GetTableDataList<tbl_users>();

            return valueList;
        }


        public virtual tbl_users GetUserInfoByName(string nickName)
        {
            sqlQuery = string.Format("WHERE nickname = N'{0}' AND isDeleted = '0'", nickName);

            tbl_users value = GetTableData<tbl_users>(sqlQuery);

            return value;
        }

        public virtual tbl_users GetUserInfo(string userNickName, string pswd)
        {
            sqlQuery = string.Format("WHERE nickname = N'{0}' AND password = '{1}' AND isDeleted = '0' ", userNickName,pswd);

            tbl_users value = GetTableData<tbl_users>(sqlQuery);

            return value;
        }

        public virtual tbl_users GetUserInfoByNickName(string userNickName)
        {
            sqlQuery = string.Format("WHERE UserNickName = N'{0}' AND isDeleted = '0'", userNickName);

            tbl_users value = GetTableData<tbl_users>(sqlQuery);

            return value;
        }

        public virtual bool CheckUserInfo(string nickName)
        {
            sqlQuery = string.Format("WHERE nickname = N'{0}' AND isDeleted = '0'", nickName);
            tbl_users value = GetTableData<tbl_users>(sqlQuery);

            if (value != null)
                return true;
            else
                return false;
        }

        public virtual bool SetUserInfo(tbl_users tblUserInfo)
        {
          //  UpsertTableData<tbl_users>(tblUserInfo);

            UpdateTableData<tbl_users>(tblUserInfo);

            return true;
        }

        public virtual bool DeleteUserInfo(tbl_users tbluserinfo)
        {
            DeleteTableData<tbl_users>(tbluserinfo);

            return true;
        }
    }
}
