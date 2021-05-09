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
using System.Data;
using Oracle.DataAccess.Client;
using System.Globalization;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for WindowPesanan.xaml
    /// </summary>
    public partial class WindowPesanan : Window
    {
        string nomor_nota;
        List<DTrans> dtranses;
        public WindowPesanan(string nomor_nota)
        {
            InitializeComponent();
            this.nomor_nota = nomor_nota;
            lblNomorNota.Content = this.nomor_nota;

            OracleCommand cmd = new OracleCommand("SELECT H.NOMOR_NOTA, INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD MONTH YYYY')), H.TOTAL, K.NAMA, P.NAMA, H.METODE_PEMBAYARAN, " +
                                                    "(CASE WHEN H.STATUS = 1 THEN 'Pending' " +
                                                    "       WHEN H.STATUS = 2 THEN 'Accepted' " +
                                                    "END) AS STATUS " +
                                                    "FROM H_TRANS H, PELANGGAN P, KARYAWAN K " +
                                                    "WHERE H.FK_KARYAWAN = K.ID AND H.FK_PELANGGAN = P.ID AND H.NOMOR_NOTA = '" + nomor_nota + "' ", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                lblNomorNota.Content = reader.GetValue(0).ToString();
                lblTanggalTrans.Content = reader.GetValue(1).ToString();
                lblPegawai.Content = reader.GetValue(3).ToString();
                lblPelanggan.Content = reader.GetValue(4).ToString();
                lblMetodePembayaran.Content = reader.GetValue(5).ToString();
                lblStatus.Content = reader.GetValue(6).ToString();
                
                lblHargaTotal.Content = Convert.ToInt32(reader.GetValue(2).ToString()).ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
            }
            reader.Close();

            loadDetailDTrans();
        }

        private void loadDetailDTrans()
        {
            dtranses = new List<DTrans>();
            OracleCommand cmd = new OracleCommand("SELECT D.NOMOR_NOTA, R.NAMA, D.QUANTITY, D.HARGA, D.SUBTOTAL " +
                                                "FROM D_TRANS D, ROTI R " +
                                                "WHERE D.FK_ROTI = R.ID AND D.NOMOR_NOTA = '" + nomor_nota + "' ", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                dtranses.Add(new DTrans()
                {
                    nomor_nota = reader.GetValue(0).ToString(),
                    id_roti = reader.GetValue(1).ToString(),
                    quantity = Convert.ToInt32(reader.GetValue(2).ToString()),
                    harga = Convert.ToInt32(reader.GetValue(3).ToString()),
                    subtotal = Convert.ToInt32(reader.GetValue(4).ToString())
                });
            }
            reader.Close();

            dtGridDetailPesanan.ItemsSource = dtranses;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
