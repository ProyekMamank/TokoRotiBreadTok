using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BreadTok
{
    class Cart
    {
        public List<Roti> rotis { get; set; }
        public List<int> jml { get; set; }
        private int total { get; set; }
        private int potongan { get; set; }
        DataTable dtDisp, dtReal;
        OracleDataAdapter da;
        OracleCommandBuilder builder;
        public Cart()
        {
            rotis = new List<Roti>();
            jml = new List<int>();
            total = 0;
            loadDTrans();
        }
        private void loadDTrans()
        {
            dtReal = new DataTable();
            da = new OracleDataAdapter("select * from d_trans where 1=2", App.conn);
            builder = new OracleCommandBuilder(da);
            da.Fill(dtReal);
        }
        public void addToCart(Roti r, int qty)
        {
            int idx = -1;
            for (int i = 0; i < rotis.Count; i++)
            {
                if (rotis[i].Equals(r))
                {
                    idx = i;
                    break;
                }
            }
            
            if (idx != -1)
            {
                jml[idx] += qty;
                total += rotis[idx].harga_roti * qty;
            }
            else
            {
                rotis.Add(r);
                jml.Add(qty);
                total += r.harga_roti * qty;
            }
        }
        public void removeFromCart(string id)
        {
            int idx = -1;
            for (int i = 0; i < rotis.Count; i++)
            {
                if (rotis[i].id_roti == id)
                {
                    idx = i;
                    break;
                }
            }
            if (idx != -1)
            {
                total -= rotis[idx].harga_roti * jml[idx];
                rotis.RemoveAt(idx);
                jml.RemoveAt(idx);
            }
        }
        public DataTable getDataTable()
        {
            dtDisp = new DataTable();
            dtDisp.Columns.Add("ID");
            dtDisp.Columns.Add("Nama Roti");
            dtDisp.Columns.Add("Harga Roti");
            dtDisp.Columns.Add("Qty");
            dtDisp.Columns.Add("Subtotal");
            dtDisp.Columns.Add("Action");

            // TODO : Tidak tampil di datagrid

            for (int i = 0; i < rotis.Count; i++)
            {
                Roti r = rotis[i];
                DataRow dr = dtDisp.NewRow();
                dr["ID"] = r.id_roti;
                dr["Nama Roti"] = r.nama_roti;
                dr["Harga Roti"] = r.harga_roti;
                dr["Qty"] = jml[i];
                dr["Subtotal"] = jml[i] * r.harga_roti;
                dr["Action"] = r.id_roti;

                dtDisp.Rows.Add(dr);
            }

            return dtDisp;
        }
        public string getFormattedTotal()
        {
            return getTotal().ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
        }
        public string getFormattedGrandTotal()
        {
            return getGrandTotal().ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
        }
        public string getFormattedPotongan()
        {
            int realPot = potongan <= 0 ? -potongan : (int)(total * (potongan / 100.0));
            return realPot.ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
        }
        public int getCartItemCount()
        {
            return rotis.Count;
        }
        public void clear()
        {
            rotis.Clear();
            jml.Clear();
            total = 0;
        }
        public int getTotal()
        {
            return total;
        }
        public int getGrandTotal()
        {
            int realPot = potongan <= 0 ? -potongan : (int)(total * (potongan / 100.0));
            return Math.Max(total - realPot,0);
        }
        public void setPotongan(int pot)
        {
            this.potongan = pot;
        }
        public void makeTransaction(string nonota)
        {
            for (int i = 0; i < rotis.Count; i++)
            {
                Roti r = rotis[i];
                int currJml = jml[i];
                DataRow dr = dtReal.NewRow();
                dr[0] = nonota;
                dr[1] = r.id_roti;
                dr[2] = currJml;
                dr[3] = r.harga_roti;
                dr[4] = r.harga_roti * currJml;

                dtReal.Rows.Add(dr);
            }
            da.Update(dtReal);
        }
    }
}
