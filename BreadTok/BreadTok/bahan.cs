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
            dt.Columns.Add("Merk");
            dt.Columns.Add("Jenis Bahan");
            dt.Columns.Add("Stok");
            dt.Columns.Add("Harga");
            dt.Columns.Add("Detail");
            dt.Columns.Add("Update");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from bahan where status > 0";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.GetValue(2).ToString();

                OracleCommand cmd2 = new OracleCommand();
                cmd2.CommandText = $"select nama_jenis from jenis_bahan where ID = {Convert.ToInt32(reader.GetValue(6))}";
                cmd2.Connection = App.conn;
                dr[1] = cmd2.ExecuteScalar().ToString();

                dr[2] = reader.GetValue(3).ToString() + " " + reader.GetValue(5).ToString();

                dr[3] = Convert.ToInt32(reader.GetValue(4));
                

                dr[4] = reader.GetValue(0).ToString();
                dr[5] = reader.GetValue(0).ToString();
                dt.Rows.Add(dr);
            }
            reader.Close();

            return dt;
        }

    }
}
