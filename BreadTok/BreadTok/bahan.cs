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
            dt.Columns.Add("HARGA");
            dt.Columns.Add("SUPPLIER");
            dt.Columns.Add("ACTION");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from bahan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.GetValue(1).ToString();

                OracleCommand cmd2 = new OracleCommand();
                cmd2.CommandText = $"select nama_jenis from jenis_bahan where ID = {Convert.ToInt32(reader.GetValue(5))}";
                cmd2.Connection = App.conn;
                dr[1] = cmd2.ExecuteScalar().ToString();

                dr[2] = reader.GetValue(2).ToString() + " " + reader.GetValue(4).ToString();

                dr[3] = Convert.ToInt32(reader.GetValue(3));

                OracleCommand cmd3 = new OracleCommand();
                cmd3.CommandText = $"select nama from supplier where ID = {Convert.ToInt32(reader.GetValue(6))}";
                cmd3.Connection = App.conn;
                dr[4] = cmd3.ExecuteScalar().ToString();

                dr[5] = reader.GetValue(0).ToString();
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
