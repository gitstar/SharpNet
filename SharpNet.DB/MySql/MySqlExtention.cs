using MySql.Data.MySqlClient;
using System;
using System.Data;
using SharpNet.Log;

namespace SharpNet.DB.MySql
{
    public class MySqlExtention
    {
        public static MySqlConnection _conn;
        public MySqlExtention(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (_conn == null)
                {
                    conn = new MySqlConnection(connectionString);
                }

                OpenDB();

            }  
        }

        public MySqlConnection conn
        {
            get
            {
                return _conn;
            }
            set
            {
                _conn = value;
            }
        }

        public void OpenDB()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
            }
            catch (Exception ex)
            {
               SharpLog.Error("OpenDB", ex.ToString());
               throw new Exception(ex.ToString());
            }           
        }

        public void CloseDB()
        {
            try
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            catch (Exception ex)
            {
                SharpLog.Error("CloseDB", ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public bool MySqlQueryExecute(string strQuery)
        {
            try
            {
                OpenDB();
                MySqlCommand mySqlCommand = new MySqlCommand(strQuery, conn);
                mySqlCommand.Prepare();
                mySqlCommand.ExecuteNonQuery();
                
                return true;
            }
            catch (Exception ex)
            {
                SharpLog.Error("MySqlQueryExecute", ex.ToString(), strQuery);
                return false;
            }
        }

        public MySqlDataReader MySqlDataRead(string sql)
        {
            try
            {
                OpenDB();
                MySqlCommand mySqlCommand1 = new MySqlCommand(sql, conn);
                mySqlCommand1.Prepare();
                MySqlDataReader mySqlDataReader1 = mySqlCommand1.ExecuteReader();               
                return mySqlDataReader1;
            }
            catch (Exception ex)
            {
                SharpLog.Error("MySqlDataRead", ex.ToString(), sql);
                return null;
            }
        }

        public DataSet MySqlQueryRead(string strQuery)
        {
           
            try
            {
                DataSet dataSet = new DataSet();

                OpenDB();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(strQuery, conn);
                mySqlDataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                SharpLog.Error("MySqlQueryRead", ex.ToString(), strQuery);
                return null;
            }
        }

        public DataTable MySqlCallProcedure(string sQuery)
        {
            return this.MySqlQueryRead(sQuery).Tables[0];
        }

        /// <summary>
        ///  mySqlCommand.Parameters.Add("@Image", MySqlDbType.Blob);
       ///  mySqlCommand.Parameters["@Image"].Value = (object)ImageData;
        /// </summary>
        /// <param name="strFunctionName"></param>
        /// <returns></returns>
        public object MySqlQueryExecuteScalar(string strFunctionName)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(strFunctionName, conn);
            mySqlCommand.CommandType = CommandType.StoredProcedure;
            //mySqlCommand.Parameters.Add(new MySqlParameter("inReasonCode", param[0]));
            //mySqlCommand.Parameters.Add("@Image", MySqlDbType.Blob);
            //mySqlCommand.Parameters["@Image"].Value = (object)ImageData;

            OpenDB();
            return mySqlCommand.ExecuteScalar();
        }

        public int MySqlHandleImage(string CmdString, byte[] ImageData)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(CmdString, conn);
            mySqlCommand.Parameters.Add("@Image", MySqlDbType.Blob);
            mySqlCommand.Parameters["@Image"].Value = (object)ImageData;
            return mySqlCommand.ExecuteNonQuery();
        }
    }
}
