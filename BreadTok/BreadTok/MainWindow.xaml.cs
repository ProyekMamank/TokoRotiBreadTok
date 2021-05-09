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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Oracle.DataAccess.Client;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string loggedUserID;
        public MainWindow(string id)
        {
            InitializeComponent();

            loggedUserID = id;
            loadDaftarPesanan();
        }

        List<HTrans> htranses = new List<HTrans>();
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MessageHandler.confirmYesNo("Are you sure you want to logout?"))
            {
                this.Close();
            }
        }

        private void PackIcon_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void loadDaftarPesanan()
        {
            OracleCommand cmd = new OracleCommand("SELECT H.NOMOR_NOTA, INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD MONTH YYYY')), H.TOTAL, K.NAMA, P.NAMA, H.METODE_PEMBAYARAN, " +
                                                    "(CASE WHEN H.STATUS = 0 THEN 'Belum Bayar' " +
                                                    "       WHEN H.STATUS = 1 THEN 'Request Bayar' " +
                                                    "       WHEN H.STATUS = 2 THEN 'Sudah Bayar' " +
                                                    "       WHEN H.STATUS = 3 THEN 'Dibatalkan' " +
                                                    "END) AS STATUS " +
                                                    "FROM H_TRANS H, PELANGGAN P, KARYAWAN K " +
                                                    "WHERE H.FK_KARYAWAN = K.ID AND H.FK_PELANGGAN = P.ID " +
                                                    "ORDER BY H.NOMOR_NOTA", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();

            htranses = new List<HTrans>();
            while (reader.Read()){
                htranses.Add(new HTrans()
                {
                    nomor_nota = reader.GetValue(0).ToString(),
                    tanggal_trans = reader.GetValue(1).ToString(),
                    total = Convert.ToInt32(reader.GetValue(2).ToString()),
                    id_karyawan = reader.GetValue(3).ToString(),
                    id_pelanggan = reader.GetValue(4).ToString(),
                    metode_pembayaran = reader.GetValue(5).ToString(),
                    status = reader.GetValue(6).ToString()
                });
            }
            reader.Close();

            dtGridPesanan.ItemsSource = htranses;

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }

        private void BtnDetailHTrans_Click(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowPesanan wp = new WindowPesanan(ID.ToString());
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wp.ShowDialog();

            loadDaftarPesanan();
            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }
    }
}
