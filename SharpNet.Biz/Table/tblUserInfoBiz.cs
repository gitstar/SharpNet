
using SharpNet.Core.Security;
using SharpNet.Dao.Table;
using SharpNet.DB.Table;
using System.Collections.Generic;

namespace SharpNet.Biz.Table
{
    public class tblUserInfoBiz
    {
        tblUserInfoDao uiDao = new tblUserInfoDao();
        /// </summary>
        public tblUserInfoBiz() 
        {

        }

        public virtual tbl_users GetUserInfoByID(int userid)
        {
            return uiDao.GetUserInfoByID(userid);
        }

        public virtual IList<tbl_users> GetAllUserInfo()
        {
            return uiDao.GetAllUserInfo();
        }

        public virtual tbl_users GetUserInfoByName(string userName)
        {
            return uiDao.GetUserInfoByName(userName);
        }

        public virtual tbl_users GetUserInfo(string userNickName, string pswd)
        {
            MD5Crypt md5 = new MD5Crypt();
            string encpswd = md5.EncryptMessage(pswd);
            return uiDao.GetUserInfo(userNickName,encpswd);
        }

        public virtual tbl_users GetUserInfoByNickName(string userNickName)
        {
            return uiDao.GetUserInfoByNickName(userNickName);
        }

        public virtual bool CheckUserInfo(string nickName)
        {
            return uiDao.CheckUserInfo(nickName);
        }

        public virtual bool SetUserInfo(tbl_users tbluserinfo)
        {
            return uiDao.SetUserInfo(tbluserinfo);
        }

        public virtual bool DeleteUserInfo(tbl_users tbluserinfo)
        {
            return uiDao.DeleteUserInfo(tbluserinfo);
        }
    }
}
