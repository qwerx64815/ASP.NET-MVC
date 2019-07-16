using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HypermarketList
{
    public class Sysdb
    {
        private string cn;
        private string sqlcmd;
        private string TableName;

        public string Sqlconn()
        {
            string conn = "Data Source=127.0.0.1;Initial Catalog=Hypermarket;" +
                "Persist Security Info=True;User ID=sa;Password=sa123";
            return conn;
        }

        public Sysdb()
        {
            this.cn = Sqlconn();
        }

        public Sysdb(string Tname)
        {
            this.cn = Sqlconn();
            this.TableName = Tname;
        }
        public Sysdb(string user_sqlcmd, string Tname)
        {
            this.cn = Sqlconn();
            this.sqlcmd = user_sqlcmd;
            this.TableName = Tname;
        }

        public DataSet SelectDB()
        {
            SqlDataAdapter MyAdapter = new SqlDataAdapter(this.sqlcmd, this.cn);
            DataSet vDataSet = new DataSet();
            try { MyAdapter.Fill(vDataSet, this.TableName); }
            catch (Exception) { }
            return vDataSet;
        }
    }
}