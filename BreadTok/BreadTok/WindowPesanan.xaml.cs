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
        string id_karyawan;
        List<DTrans> dtranses;
        public WindowPesanan(string nomor_nota, string id_karyawan)
        {
            InitializeComponent();
            this.nomor_nota = nomor_nota;
            lblNomorNota.Content = this.nomor_nota;
            this.id_karyawan = id_karyawan;

            loadHeaderTrans();
        }

        private void loadHeaderTrans()
        {
            OracleCommand cmd = new OracleCommand("SELECT H.NOMOR_NOTA, INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD MONTH YYYY')), H.TOTAL, NVL(H.FK_KARYAWAN, ''), P.NAMA, H.METODE_PEMBAYARAN, NVL(H.FK_USER_VOUCHER, ''), " +
                                                    "(CASE WHEN H.STATUS = 0 THEN 'Belum Bayar' " +
                                                    "       WHEN H.STATUS = 1 THEN 'Request Bayar' " +
                                                    "       WHEN H.STATUS = 2 THEN 'Sudah Bayar' " +
                                                    "       WHEN H.STATUS = 3 THEN 'Dibatalkan' " +
                                                    "END) AS STATUS " +
                                                    "FROM H_TRANS H, PELANGGAN P " +
                                                    "WHERE H.FK_PELANGGAN = P.ID AND H.NOMOR_NOTA = '" + nomor_nota + "' ", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lblNomorNota.Content = reader.GetValue(0).ToString();
                lblTanggalTrans.Content = reader.GetValue(1).ToString();
                
                if (reader.GetValue(3).ToString() == "")
                {
                    lblPegawai.Content = "-";
                }
                else
                {
                    OracleCommand cmd2 = new OracleCommand("SELECT NAMA FROM KARYAWAN WHERE ID = '" + reader.GetValue(3).ToString() + "'", App.conn);
                    lblPegawai.Content = cmd2.ExecuteScalar().ToString();
                }
                
                lblPelanggan.Content = reader.GetValue(4).ToString();
                lblMetodePembayaran.Content = reader.GetValue(5).ToString();

                if (reader.GetValue(6).ToString() == "")
                {
                    lblKodeVoucher.Content = "-";
                }
                else
                {
                    OracleCommand cmd2 = new OracleCommand("SELECT V.NAMA " +
                        "FROM USER_VOUCHER UV, VOUCHER V " +
                        "WHERE UV.FK_VOUCHER = V.ID AND " +
                        "UV.ID = '" + reader.GetValue(6).ToString() + "'", App.conn);
                    lblKodeVoucher.Content = cmd2.ExecuteScalar().ToString(); ;
                }

                lblStatus.Content = reader.GetValue(7).ToString();
                

                lblHargaTotal.Content = Convert.ToInt32(reader.GetValue(2).ToString()).ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
            }
            reader.Close();

            buttonActionHandler();

            loadDetailTrans();
        }

        private void buttonActionHandler()
        {
            if (lblStatus.Content.Equals("Belum Bayar"))
            {
                btnKonfirmasi.Visibility = Visibility.Hidden;
                btnBatalkan.Visibility = Visibility.Visible;
                btnBatalkan.Margin = new Thickness(300, 428, 0, 0);
            }
            else if (lblStatus.Content.Equals("Request Bayar"))
            {
                btnKonfirmasi.Visibility = Visibility.Visible;
                btnBatalkan.Visibility = Visibility.Visible;
                btnBatalkan.Margin = new Thickness(425, 428, 0, 0);
            }
            else if (lblStatus.Content.Equals("Sudah Bayar") || lblStatus.Content.Equals("Dibatalkan"))
            {
                btnKonfirmasi.Visibility = Visibility.Hidden;
                btnBatalkan.Visibility = Visibility.Hidden;
            }
        }

        private void loadDetailTrans()
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

        private void BtnBatalkan_Click(object sender, RoutedEventArgs e)
        {
            if (MessageHandler.confirmYesNo("Apakah anda ingin membatalkan pesanan ini?"))
            {
                OracleCommand cmd = new OracleCommand("UPDATE H_TRANS SET STATUS = :1, FK_KARYAWAN = :2 WHERE NOMOR_NOTA = :3 ", App.conn);
                cmd.Parameters.Add(":1", 3);
                cmd.Parameters.Add(":2", id_karyawan);
                cmd.Parameters.Add(":3", lblNomorNota.Content);
                cmd.ExecuteNonQuery();

                MessageHandler.messageSuccess("Cancel Transaksi");
                loadHeaderTrans();
            }
        }

        private void BtnKonfirmasi_Click(object sender, RoutedEventArgs e)
        {
            if (MessageHandler.confirmYesNo("Apakah anda ingin konfirmasi pesanan ini?"))
            {
                OracleCommand cmd = new OracleCommand("UPDATE H_TRANS SET STATUS = :1, FK_KARYAWAN = :2 WHERE NOMOR_NOTA = :3 ", App.conn);
                cmd.Parameters.Add(":1", 2);
                cmd.Parameters.Add(":2", id_karyawan);
                cmd.Parameters.Add(":3", lblNomorNota.Content);
                cmd.ExecuteNonQuery();

                MessageHandler.messageSuccess("Konfirmasi Pembayaran Transaksi");
                loadHeaderTrans();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
