using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
        private bahan b;
        private int selectedIndex;
        string loggedUserID;
        public MainWindow(string id)
        {
            InitializeComponent();
            b = new bahan();
            loggedUserID = id;
            loadData();
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

        private void loadData()
        {
            dgBahan.ItemsSource = null;
            dgBahan.ItemsSource = b.loadData().DefaultView;
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Hidden;
            panelInsertBahan.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Hidden;
            btnBack.Visibility = Visibility.Visible;
            loadCbJenisBahan();
            loadCbSupplier();
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Visible;
            panelInsertBahan.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
        }

        private void dgBahan_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ACTION")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnUpdate_Click));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("ACTION"));
                buttonFactory.SetValue(Button.ContentProperty, "UPDATE");
                buttonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Colors.Green));
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine((sender as Button).CommandParameter);
        }

        private void dgBahan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedIndex = Convert.ToInt32((((DataGrid)sender).SelectedItem as DataRowView)[5]);
        }


        private void loadCbJenisBahan()
        {
            cbJenisBahan.Items.Clear();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from jenis_bahan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbJenisBahan.Items.Add(new ComboBoxItem()
                {
                    Name = "ID" + reader.GetString(0),
                    Content = reader.GetString(1)
                });
            }
            reader.Close();
            cbJenisBahan.SelectedValuePath = "Name";
            cbJenisBahan.SelectedIndex = 0;
        }

        private void loadCbSupplier()
        {
            cbSupplier.Items.Clear();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from supplier";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbSupplier.Items.Add(new ComboBoxItem()
                {
                    Name = "ID" + reader.GetString(0),
                    Content = reader.GetString(1)
                });
            }
            reader.Close();
            cbSupplier.SelectedValuePath = "Name";
            cbSupplier.SelectedIndex = 0;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string merk = tbMerk.Text.ToUpper();
            int qty = Convert.ToInt32(tbQuantity.Text);
            int harga = Convert.ToInt32(tbHarga.Text);
            string satuan = cbSatuan.SelectedItem.ToString();
            string jenisBahan = cbJenisBahan.SelectedValue.ToString().Substring(2);
            string supplier = cbSupplier.SelectedValue.ToString().Substring(2);


            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "insert into bahan values(:1,:2,:3,:4,:5,:6,:7)";
            cmd.Connection = App.conn;
            cmd.Parameters.Add(":1", "0");
            cmd.Parameters.Add(":2", merk);
            cmd.Parameters.Add(":3", qty);
            cmd.Parameters.Add(":4", harga);
            cmd.Parameters.Add(":5", satuan);
            cmd.Parameters.Add(":6", jenisBahan);
            cmd.Parameters.Add(":7", supplier);
            cmd.ExecuteNonQuery();

            resetInsertPanel();
            loadData();

            panelMasterBahan.Visibility = Visibility.Visible;
            panelInsertBahan.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
        }

        private void resetInsertPanel()
        {
            tbMerk.Text = "";
            tbQuantity.Text = "";
            selectedIndex = -1;
        }
    }
}
