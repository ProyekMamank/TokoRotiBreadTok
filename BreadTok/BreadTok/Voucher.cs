using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;

namespace BreadTok
{
    class Voucher
    {
        DataTable dt;

        public DataTable loadData()
        {
            dt = new DataTable();
            dt.Columns.Add("Nama");
            dt.Columns.Add("Jenis");
            dt.Columns.Add("Potongan");

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from voucher";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();

                dr[0] = reader.GetValue(1).ToString();

                dr[1] = reader.GetValue(2).ToString();
                
                if (reader.GetValue(2).ToString() == "POTONGAN")
                {
                    dr[2] = "Rp " + reader.GetValue(3).ToString();
                }
                else
                {
                    dr[2] = reader.GetValue(3).ToString() + " %";
                }
                
                
                dt.Rows.Add(dr);
            }
            reader.Close();

            return dt;
        }
    }
}
