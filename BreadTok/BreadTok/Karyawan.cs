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
            dt.Columns.Add("NAMA");
            dt.Columns.Add("USERNAME");
            dt.Columns.Add("PASSWORD");
            dt.Columns.Add("EMAIL");
            dt.Columns.Add("JABATAN");
            dt.Columns.Add("STATUS");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from karyawan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.GetValue(4).ToString();

                dr[1] = reader.GetValue(2).ToString();

                dr[2] = reader.GetValue(3).ToString();

                dr[3] = reader.GetValue(7).ToString();

                OracleCommand cmd2 = new OracleCommand();
                cmd2.CommandText = $"select nama_jabatan from jabatan where ID = {Convert.ToInt32(reader.GetValue(11))}";
                cmd2.Connection = App.conn;
                dr[4] = cmd2.ExecuteScalar().ToString();

                string status = "";
                if(reader.GetValue(10).ToString() == "1")
                {
                    status = "ACTIVE";
                }
                else
                {
                    status = "SUSPENDED";
                }

                dr[5] = status;
                dt.Rows.Add(dr);
            }
            reader.Close();

            return dt;
        }
    }
}
