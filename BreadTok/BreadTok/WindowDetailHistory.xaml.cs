using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for WindowDetailHistory.xaml
    /// </summary>
    public partial class WindowDetailHistory : Window
    {
        HTrans ht;
        public WindowDetailHistory(HTrans ht)
        {
            InitializeComponent();
            this.ht = ht;
            init();
        }
        private void init()
        {
            lbNomorNota.Content = ht.nomor_nota;
            lbTanggalTrans.Content = ht.tanggal_trans;
            lbKodeVoucher.Content = ht.id_voucher;
            lbStatus.Content = ht.status;
            lbPegawai.Content = ht.nama_karyawan;
            lbPelanggan.Content = ht.nama_pelanggan;
            lbMetodePembayaran.Content = ht.metode_pembayaran;
            lbHargaTotal.Content = ht.total.ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
            loadDtrans();
        }
        private void loadDtrans()
        {
            dgDetailHistory.ItemsSource = null;
            dgDetailHistory.ItemsSource = ht.dtrans;
            Console.WriteLine(ht.dtrans.Count);
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgDetailHistory_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgDetailHistory.Columns.Count > 0)
            {
                //public string nomor_nota { get; set; }
                //public string id_roti { get; set; }
                //public string nama_roti { get; set; }
                //public int quantity { get; set; }
                //public int harga { get; set; }
                //public int subtotal { get; set; }
                dgDetailHistory.Columns[0].Header = "Nomor Nota";
                dgDetailHistory.Columns[1].Header = "ID Roti";
                dgDetailHistory.Columns[2].Header = "Nama Roti";
                dgDetailHistory.Columns[3].Header = "Quantity";
                dgDetailHistory.Columns[4].Header = "Harga";
                dgDetailHistory.Columns[5].Header = "Subtotal";

                dgDetailHistory.Columns[0].Visibility = Visibility.Hidden;
                dgDetailHistory.Columns[1].Visibility = Visibility.Hidden;
            }
        }

        private void btOrder_Click(object sender, RoutedEventArgs e)
        {
            // Cetak struk
            WindowCetakStruk cs = new WindowCetakStruk(ht.nomor_nota, this);
            cs.ShowDialog();
        }
    }
}
