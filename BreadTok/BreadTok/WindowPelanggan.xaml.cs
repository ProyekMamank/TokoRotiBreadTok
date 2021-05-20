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
using System.Windows.Shapes;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for WindowPelanggan.xaml
    /// </summary>
    public partial class WindowPelanggan : Window
    {
        string loggedUserID;
        List<Roti> rotis;
        Cart cart;
        DataTable dtVoucher;

        public WindowPelanggan(string id)
        {
            InitializeComponent();
            loggedUserID = id;
            init();
        }
        private void init()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = $"select nama from pelanggan where id = '{loggedUserID}'";
            string nama = cmd.ExecuteScalar().ToString();
            lbWelcome.Text = $"Selamat Datang, {nama}!";
            cart = new Cart();
            dgRoti.ColumnWidth = DataGridLength.Auto;
            loadData();
            loadCart();
            loadVoucher();
        }

        private void btLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageHandler.confirmYesNo("Are you sure you want to logout?"))
            {
                this.Close();
            }

        }

        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void loadData()
        {
            rotis = new List<Roti>();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = "select * from roti where status > 0";
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rotis.Add(new Roti()
                {
                    id_roti = reader.GetValue(0).ToString(),
                    kode_roti = reader.GetValue(1).ToString(),
                    nama_roti = reader.GetValue(2).ToString(),
                    deskripsi_roti = reader.GetValue(3).ToString(),
                    harga_roti = Convert.ToInt32(reader.GetValue(4).ToString()),
                    stok_roti = Convert.ToInt32(reader.GetValue(5).ToString()),
                    status_roti = reader.GetValue(6).ToString(),
                    fk_jenisroti = reader.GetValue(7).ToString(),
                    fk_resep = reader.GetValue(8).ToString()
                });
            }
            reader.Close();

            dgRoti.ItemsSource = rotis;
        }
        private void loadCart()
        {
            DataTable dt = cart.loadToDatagrid();
            dgCart.ItemsSource = dt.DefaultView;
            lbTotal.Text = cart.getTotal();
        }
        private void clearCart()
        {
            cart.clear();
            loadCart();
        }
        private void loadVoucher()
        {
            dtVoucher = new DataTable();
            dtVoucher.Columns.Add("ID");
            dtVoucher.Columns.Add("Kode Voucher");
            dtVoucher.Columns.Add("Jenis Voucher");
            dtVoucher.Columns.Add("Nominal");
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = "select UV.ID, V.NAMA, V.JENIS, V.POTONGAN " +
                "from VOUCHER V " +
                "join USER_VOUCHER UV on V.ID = UV.FK_VOUCHER " +
                "join PELANGGAN P on P.ID = UV.FK_PELANGGAN " +
                "where UV.STATUS > 0 AND UV.FK_PELANGGAN = :1";
            cmd.Parameters.Add(":1", loggedUserID);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DataRow dr = dtVoucher.NewRow();
                dr["ID"] = reader.GetValue(0).ToString();
                dr["Kode Voucher"] = reader.GetValue(1).ToString();
                dr["Jenis Voucher"] = reader.GetValue(2).ToString();
                string nom = reader.GetValue(3).ToString();
                if (reader.GetValue(2).ToString() == "DISKON")
                {
                    nom += "%";
                }
                else
                {
                    nom = "Rp " + nom;
                }
                dr["Nominal"] = nom;
                dtVoucher.Rows.Add(dr);
            }
            reader.Close();

            dgVoucher.ItemsSource = null;
            dgVoucher.ItemsSource = dtVoucher.DefaultView;
            Console.WriteLine(dtVoucher.Columns.Count);
            Console.WriteLine(dgVoucher.Columns.Count);

            // Combo box voucher saat checkout
            cbVoucher.ItemsSource = dtVoucher.DefaultView;
            cbVoucher.SelectedValuePath = "ID";
            cbVoucher.DisplayMemberPath = "Kode Voucher";
        }

        private void btAddToCart_Click(object sender, RoutedEventArgs e)
        {
            foreach (Roti r in rotis)
            {
                if (r.id_roti == ((Button)sender).CommandParameter.ToString())
                {
                    cart.addToCart(r, 1);
                }
            }
            loadCart();
        }
        private void btRemoveFromCart(object sender, RoutedEventArgs e)
        {
            cart.removeFromCart(((Button)sender).CommandParameter.ToString());
            loadCart();
        }

        private void dgCart_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Action")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btRemoveFromCart));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("Action"));
                buttonFactory.SetValue(Button.ContentProperty, "Remove");
                buttonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Colors.Red));
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            clearCart();
        }

        private void dgVoucher_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgVoucher.Columns.Count > 0) dgVoucher.Columns[0].Visibility = Visibility.Hidden;
        }
    }
}
