using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadTok
{
    class Karyawan
    {
        DataTable dt;

        public DataTable loadData()
        {
            dt = new DataTable();
            dt.Columns.Add("Nama");
            dt.Columns.Add("Username");
            dt.Columns.Add("Status");
            dt.Columns.Add("Detail");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from karyawan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.GetValue(4).ToString();

                dr[1] = reader.GetValue(2).ToString();

                string status = "";
                if(reader.GetValue(10).ToString() == "1")
                {
                    status = "ACTIVE";
                }
                else
                {
                    status = "SUSPENDED";
                }

                dr[2] = status;

                dr[3] = reader.GetValue(0).ToString();
                dt.Rows.Add(dr);
            }
            reader.Close();

            return dt;
        }
    }
}
