using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for WindowResep.xaml
    /// </summary>
    public partial class WindowResep : Window
    {
        string id;
        List<isiDaftarBahan> daftarbahan;

        class isiDaftarBahan
        {
            public string jenis_bahan { get; set; }
            public string kode_bahan { get; set; }
            public string merk_bahan { get; set; }
            public string jumlah_bahan { get; set; }
        }
        public WindowResep(string id)
        {
            InitializeComponent();
            this.id = id;
            loadResep();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void loadResep()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select nama from h_resep where id='{id}'";
            cmd.Connection = App.conn;
            lblNama.Content = cmd.ExecuteScalar().ToString();

            daftarbahan = new List<isiDaftarBahan>();
            cmd = new OracleCommand();
            cmd.CommandText = $"select * from d_resep where id_h_resep='{id}'";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string jenis = "";
                string kode = "";
                string merk = "";
                string jumlah = "";

                cmd.CommandText = $"select * from bahan where id={reader.GetValue(1).ToString()}";
                cmd.Connection = App.conn;
                OracleDataReader reader2 = cmd.ExecuteReader();
                while (reader2.Read())
                {
                    cmd.CommandText = $"select nama_jenis from jenis_bahan where id='{reader2.GetValue(6).ToString()}'";
                    cmd.Connection = App.conn;
                    jenis = cmd.ExecuteScalar().ToString();
                    kode = reader2.GetValue(1).ToString();
                    merk = reader2.GetValue(2).ToString();
                    jumlah = reader.GetValue(2).ToString() + " " + reader2.GetValue(5).ToString();
                }
                reader2.Close();

                daftarbahan.Add(new isiDaftarBahan
                {
                    jenis_bahan = jenis,
                    kode_bahan = kode,
                    merk_bahan = merk,
                    jumlah_bahan = jumlah,
                });
            }
            reader.Close();
            dgDaftarBahan.ItemsSource = daftarbahan;
        }
    }
}
