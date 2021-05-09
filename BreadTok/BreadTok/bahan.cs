using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadTok
{
    class bahan
    {
        DataTable dt;

        public DataTable loadData()
        {
            dt = new DataTable();
            dt.Columns.Add("MERK");
            dt.Columns.Add("JENIS BAHAN");
            dt.Columns.Add("STOCK");
            dt.Columns.Add("SUPPLIER");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from bahan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.GetValue(1).ToString();

                OracleCommand cmd2 = new OracleCommand();
                cmd2.CommandText = $"select nama_jenis from jenis_bahan where ID = {Convert.ToInt32(reader.GetValue(3))}";
                cmd2.Connection = App.conn;
                dr[1] = cmd2.ExecuteScalar().ToString();

                dr[2] = reader.GetValue(2).ToString() + " gram";

                OracleCommand cmd3 = new OracleCommand();
                cmd3.CommandText = $"select nama from supplier where ID = {Convert.ToInt32(reader.GetValue(4))}";
                cmd3.Connection = App.conn;
                dr[3] = cmd3.ExecuteScalar().ToString();
                dt.Rows.Add(dr);
            }
            reader.Close();

            return dt;
        }

        public DataTable searchData()
        {
            return dt;
        }
    }
}
