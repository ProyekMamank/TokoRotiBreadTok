﻿using System;
using System.Collections.Generic;
using System.Data;
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
        public int total { get; set; }
        DataTable dt;
        public Cart()
        {
            rotis = new List<Roti>();
            jml = new List<int>();
            total = 0;
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
        public DataTable loadToDatagrid()
        {
            dt = new DataTable();
            dt.Columns.Add("Nama Roti");
            dt.Columns.Add("Harga Roti");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Subtotal");
            dt.Columns.Add("Action");

            // TODO : Tidak tampil di datagrid

            for (int i = 0; i < rotis.Count; i++)
            {
                Roti r = rotis[i];
                DataRow dr = dt.NewRow();
                //dr["Nama Roti"] = r.nama_roti;
                //dr["Harga Roti"] = r.harga_roti;
                //dr["Qty"] = jml[i];
                //dr["Subtotal"] = jml[i] * r.harga_roti;
                //dr["Action"] = r.id_roti;
                //dr[0] = r.nama_roti;
                //dr[1] = r.harga_roti;
                //dr[2] = jml[i];
                //dr[3] = jml[i] * r.harga_roti;
                //dr[4] = r.id_roti;
                dr["Nama Roti"] = r.nama_roti;
                dr["Harga Roti"] = r.harga_roti;
                dr["Qty"] = jml[i];
                dr["Subtotal"] = jml[i] * r.harga_roti;
                dr["Action"] = r.id_roti;

                dt.Rows.Add(dr);
            }

            return dt;
        }
        public string getTotal()
        {
            return "Total : Rp " + total;
        }
        public void clear()
        {
            rotis.Clear();
            jml.Clear();
            total = 0;
        }
    }
}
