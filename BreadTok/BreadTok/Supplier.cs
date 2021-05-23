using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;

namespace BreadTok
{
    class Supplier
    {
        public string getColumnByID(string columnName, int id)
        {
            OracleCommand cmd = new OracleCommand($"SELECT {columnName.ToUpper()} FROM SUPPLIER WHERE ID = {id}", App.conn);
            return cmd.ExecuteScalar().ToString();
        }
        public string getColumnByKode(string columnName, string kode)
        {
            OracleCommand cmd = new OracleCommand($"SELECT {columnName.ToUpper()} FROM SUPPLIER WHERE KODE = '{kode}'", App.conn);
            return cmd.ExecuteScalar().ToString();
        }

        public DataTable GetDataTable(string columns)
        {
            DataTable listSupplier = new DataTable();
            OracleCommand cmd = new OracleCommand($"SELECT {columns} FROM SUPPLIER", App.conn);
            cmd.ExecuteReader();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(listSupplier);
            return listSupplier;
        }
    }
}
