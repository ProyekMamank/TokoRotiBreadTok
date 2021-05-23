using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadTok
{
    class Roti
    {
        public string id_roti { get; set; }
        public string nama_roti { get; set; }
        public string deskripsi_roti { get; set; }
        public int harga_roti { get; set; }
        public int stok_roti { get; set; }
        public string status_roti { get; set; }
        public string fk_jenisroti { get; set; }
        public string fk_resep { get; set; }
        public DataTable fillResep(string columns, string whereClause, DataTable data)
        {
            OracleCommand cmd = new OracleCommand($"SELECT {columns} FROM D_RESEP D JOIN BAHAN B ON B.ID = D.ID_BAHAN JOIN JENIS_BAHAN JB ON B.JENIS_BAHAN = JB.ID {whereClause}", App.conn);
            cmd.ExecuteReader();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(data);
            return data;
        }
        public DataTable fillDataTable(string columns, string whereClause, DataTable data)
        {
            OracleCommand cmd = new OracleCommand($"SELECT {columns} FROM ROTI R JOIN JENIS_ROTI JR ON R.JENIS_ROTI = JR.ID JOIN H_RESEP HR ON HR.ID = R.FK_RESEP {whereClause}", App.conn);
            cmd.ExecuteReader();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(data);
            return data;
        }

        public DataTable fillDataJenisRoti(string columns, string whereClause, DataTable data)
        {
            OracleCommand cmd = new OracleCommand($"SELECT {columns} FROM JENIS_ROTI {whereClause}", App.conn);
            cmd.ExecuteReader();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(data);
            return data;
        }
    }
}
