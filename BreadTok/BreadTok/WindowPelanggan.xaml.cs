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
        List<UserVoucher> vouchers;
        List<HTrans> histories;
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
            loadRoti();
            loadCart();
            loadVoucher();
            loadHistory();
        }
        class UserVoucher
        {
            public string ID { get; set; }
            public string kode { get; set; }
            public string jenis { get; set; }
            public string nominal { get; set; }
            public int realNominal { get; set; }

            public override string ToString()
            {
                return $"{kode} ({nominal})";
            }
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

        private void loadRoti()
        {
            rotis = new List<Roti>();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = "select ID,NAMA,DESKRIPSI,HARGA,STOK,STATUS,JENIS_ROTI,FK_RESEP from roti where status > 0";
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rotis.Add(new Roti()
                {
                    id_roti = reader.GetValue(0).ToString(),
                    nama_roti = reader.GetValue(1).ToString(),
                    deskripsi_roti = reader.GetValue(2).ToString(),
                    harga_roti = Convert.ToInt32(reader.GetValue(3).ToString()),
                    stok_roti = Convert.ToInt32(reader.GetValue(4).ToString()),
                    status_roti = reader.GetValue(5).ToString(),
                    fk_jenisroti = reader.GetValue(6).ToString(),
                    fk_resep = reader.GetValue(7).ToString()
                });
            }
            reader.Close();

            dgRoti.ItemsSource = rotis;
        }
        private void loadCart()
        {
            DataTable dt = cart.getDataTable();
            dgCart.ItemsSource = dt.DefaultView;
            lbTotal.Text = cart.getFormattedTotal();
            lbPotongan.Text = cart.getFormattedPotongan();
            lbGrandTotal.Text = cart.getFormattedGrandTotal();
        }
        private void clearCart()
        {
            cart.clear();
            loadCart();
        }
        private void loadVoucher()
        {
            vouchers = new List<UserVoucher>();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = "select UV.ID, V.NAMA, V.JENIS, V.POTONGAN " +
                "from VOUCHER V " +
                "join USER_VOUCHER UV on V.ID = UV.FK_VOUCHER " +
                "join PELANGGAN P on P.ID = UV.FK_PELANGGAN " +
                "where UV.STATUS > 0 AND UV.FK_PELANGGAN = :1 AND UV.STATUS > 0";
            cmd.Parameters.Add(":1", loggedUserID);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string nom = reader.GetValue(3).ToString();
                string realNom = nom;
                if (reader.GetValue(2).ToString() == "DISKON")
                {
                    nom += "%";
                }
                else
                {
                    nom = "Rp " + nom;
                }

                vouchers.Add(new UserVoucher()
                {
                    ID = reader.GetValue(0).ToString(),
                    kode = reader.GetValue(1).ToString(),
                    jenis = reader.GetValue(2).ToString(),
                    nominal = nom,
                    realNominal = Convert.ToInt32(realNom)
                });
            }
            reader.Close();

            // DataGrid List Voucher
            dgVoucher.ItemsSource = null;
            dgVoucher.ItemsSource = vouchers;

            // Combo box voucher saat checkout
            cbVoucher.ItemsSource = vouchers;
            cbVoucher.SelectedValuePath = "ID";

        }
        private void loadHistory()
        {
            histories = new List<HTrans>();
            OracleCommand cmdH = new OracleCommand();
            cmdH.Connection = App.conn;
            cmdH.CommandText =
                @"select
                       H.NOMOR_NOTA,
                       INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD FmMONTH YYYY')),
                       H.TOTAL,
                       nvl(K.NAMA,'-'),
                       P.NAMA,
                       H.METODE_PEMBAYARAN,
                       nvl(V.NAMA,'-'),
                       (CASE WHEN H.STATUS = 0 THEN 'Belum Bayar'
                           WHEN H.STATUS = 1 THEN 'Request Bayar'
                           WHEN H.STATUS = 2 THEN 'Sudah Bayar'
                           WHEN H.STATUS = 3 THEN 'Dibatalkan'
                           END),
                       (select nvl(sum(D.QUANTITY),0) as jml
                            from D_TRANS D
                            where D.NOMOR_NOTA = H.NOMOR_NOTA
                        )
                from H_TRANS H
                join PELANGGAN P on P.ID = H.FK_PELANGGAN
                left join USER_VOUCHER UV on UV.ID = H.FK_USER_VOUCHER
                left join VOUCHER V on V.ID = UV.FK_VOUCHER
                left join KARYAWAN K on K.ID = H.FK_KARYAWAN
                where H.FK_PELANGGAN = :id_pelanggan
                order by H.TANGGAL_TRANS desc"
            ;
            cmdH.Parameters.Add(":id_pelanggan", loggedUserID);
            OracleDataReader reader = cmdH.ExecuteReader();
            while (reader.Read())
            {
                histories.Add(new HTrans()
                {
                    nomor_nota = reader.GetValue(0).ToString(),
                    tanggal_trans = reader.GetValue(1).ToString(),
                    total = Convert.ToInt32(reader.GetValue(2).ToString()),
                    nama_karyawan = reader.GetValue(3).ToString(),
                    nama_pelanggan = reader.GetValue(4).ToString(),
                    metode_pembayaran = reader.GetValue(5).ToString(),
                    id_voucher = reader.GetValue(6).ToString(),
                    status = reader.GetValue(7).ToString(),
                    countRoti = reader.GetValue(8).ToString(),
                    action = reader.GetValue(0).ToString()
                });
            }

            dgHistory.ItemsSource = null;
            dgHistory.ItemsSource = histories;

            foreach (HTrans ht in histories)
            {
                List<DTrans> dt = new List<DTrans>();

                OracleCommand cmdD = new OracleCommand();
                cmdD.Connection = App.conn;
                cmdD.CommandText =
                    @"select
                           DT.NOMOR_NOTA,
                           DT.FK_ROTI,
                           R.NAMA,
                           DT.QUANTITY,
                           DT.HARGA,
                           DT.SUBTOTAL
                    from D_TRANS DT
                    join ROTI R on DT.FK_ROTI = R.ID
                    where DT.NOMOR_NOTA = :nonota";
                cmdD.Parameters.Add(":nonota", ht.nomor_nota);
                OracleDataReader readerD = cmdD.ExecuteReader();
                while (readerD.Read())
                {
                    dt.Add(new DTrans() { 
                        nomor_nota = readerD.GetValue(0).ToString(),
                        id_roti = readerD.GetValue(1).ToString(),
                        nama_roti = readerD.GetValue(2).ToString(),
                        quantity = Convert.ToInt32(readerD.GetValue(3).ToString()),
                        harga = Convert.ToInt32(readerD.GetValue(4).ToString()),
                        subtotal = Convert.ToInt32(readerD.GetValue(5).ToString())
                    });
                }
                ht.dtrans = dt;
            }
        }
        private void overlayOn()
        {
            overlay.Visibility = Visibility.Visible;
            overlay.Width = this.ActualWidth;
            overlay.Height = this.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }
        private void overlayOff()
        {
            overlay.Visibility = Visibility.Hidden;
            overlay.Width = this.ActualWidth;
            overlay.Height = this.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
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
            if (dgVoucher.Columns.Count > 0)
            {
                dgVoucher.Columns[0].Visibility = Visibility.Hidden;
                dgVoucher.Columns[4].Visibility = Visibility.Hidden;
            }
        }

        private void dgCart_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgCart.Columns.Count > 0) 
            { 
                dgCart.Columns[0].Visibility = Visibility.Hidden;
            }
        }

        private void btOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cbMetode.SelectedIndex != -1 && cart.getCartItemCount() > 0)
            {
                Console.WriteLine(cbVoucher.SelectedValue);
                using (OracleTransaction trans = App.conn.BeginTransaction())
                {
                    try
                    {
                        OracleCommand cmdNoNota = new OracleCommand();
                        cmdNoNota.Connection = App.conn;
                        cmdNoNota.CommandText = "NOMOR_NOTA_autogen";
                        cmdNoNota.CommandType = CommandType.StoredProcedure;
                        cmdNoNota.Parameters.Add(new OracleParameter()
                        {
                            Direction = ParameterDirection.ReturnValue,
                            ParameterName = "nonota",
                            OracleDbType = OracleDbType.Varchar2,
                            Size = 15
                        });

                        cmdNoNota.ExecuteNonQuery();
                        string nonota = cmdNoNota.Parameters["nonota"].Value.ToString();

                        // H_TRANS
                        OracleCommand cmdH = new OracleCommand();
                        cmdH.Connection = App.conn;
                        cmdH.CommandText = "insert into H_TRANS (NOMOR_NOTA,TOTAL,FK_PELANGGAN,METODE_PEMBAYARAN,FK_USER_VOUCHER) " +
                            "values (:nonota, :1, :2, :3, :fkvoucher)";
                        cmdH.Parameters.Add(":nonota", nonota);
                        cmdH.Parameters.Add(":1", cart.getGrandTotal());
                        cmdH.Parameters.Add(":2", loggedUserID);
                        cmdH.Parameters.Add(":3", cbMetode.SelectedItem.ToString());
                        cmdH.Parameters.Add(":fkvoucher", cbVoucher.SelectedValue);
                        Console.WriteLine(cbMetode.SelectedItem.ToString());
                        cmdH.ExecuteNonQuery();

                        // D_TRANS
                        foreach (DataRow dr in cart.getDataTable().Rows)
                        {
                            OracleCommand cmdD = new OracleCommand();
                            cmdD.Connection = App.conn;
                            cmdD.CommandText = "insert into D_TRANS (NOMOR_NOTA,FK_ROTI, QUANTITY, SUBTOTAL) " +
                                "values (:nonota, :roti, :qty, :sbt)";
                            cmdD.Parameters.Add(":nonota", nonota);
                            cmdD.Parameters.Add(":roti", dr["ID"]);
                            cmdD.Parameters.Add(":qty", dr["Qty"]);
                            cmdD.Parameters.Add(":sbt", dr["Subtotal"]);
                            cmdD.ExecuteNonQuery();
                        }

                        if (cbVoucher.SelectedValue != null)
                        {
                            OracleCommand cmdUV = new OracleCommand();
                            cmdUV.Connection = App.conn;
                            cmdUV.CommandText = $"update user_voucher set status=0 where id='{cbVoucher.SelectedValue}'";
                            cmdUV.ExecuteNonQuery();
                        }

                        trans.Commit();
                        clearCart();
                        loadRoti();
                        loadVoucher();
                        loadHistory();
                        MessageHandler.messageSuccess("Order");
                    }
                    catch (OracleException ex)
                    {
                        trans.Rollback();
                        if (ex.Number == 20001)
                        {
                            MessageBox.Show("Stok tidak mencukupi!");
                        }
                        else
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            else
            {
                if (cart.getCartItemCount() <= 0) MessageBox.Show("No item in cart! Order failed.");
                else MessageBox.Show("Please Choose a Payment Method!");
            }
            
        }

        private void btClearVoucher_Click(object sender, RoutedEventArgs e)
        {
            cbVoucher.SelectedIndex = -1;
        }

        private void cbVoucher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVoucher.SelectedIndex != -1)
            {
                UserVoucher selUV = vouchers[cbVoucher.SelectedIndex];
                if (selUV.jenis == "DISKON")
                {
                    cart.setPotongan(selUV.realNominal);
                }
                else
                {
                    // Potongan
                    cart.setPotongan(-selUV.realNominal);
                }
            }
            else
            {
                cart.setPotongan(0);
            }
            loadCart();
        }

        private void dgHistory_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgHistory.Columns.Count > 0)
            {
                //public string nomor_nota { get; set; }
                //public string tanggal_trans { get; set; }
                //public int total { get; set; }
                //public string id_karyawan { get; set; }
                //public string id_pelanggan { get; set; }
                //public string nama_karyawan { get; set; }
                //public string nama_pelanggan { get; set; }
                //public string metode_pembayaran { get; set; }
                //public string id_voucher { get; set; }
                //public string status { get; set; }
                //public string countRoti { get; set; }
                //public List<DTrans> dtrans { get; set; }
                dgHistory.Columns[0].Header = "Nomor Nota";
                dgHistory.Columns[1].Header = "Tanggal Transaksi";
                dgHistory.Columns[2].Header = "Total";
                dgHistory.Columns[3].Header = "ID Karyawan";
                dgHistory.Columns[4].Header = "ID Pelanggan";
                dgHistory.Columns[5].Header = "Nama Karyawan";
                dgHistory.Columns[6].Header = "Nama Pelanggan";
                dgHistory.Columns[7].Header = "Metode";
                dgHistory.Columns[8].Header = "Kode Voucher";
                dgHistory.Columns[9].Header = "Status";
                dgHistory.Columns[10].Header = "Jumlah Roti";
                dgHistory.Columns[11].Header = "DTRANS";
                dgHistory.Columns[12].Header = "Action";


                dgHistory.Columns[3].Visibility = Visibility.Hidden;
                dgHistory.Columns[4].Visibility = Visibility.Hidden;
                dgHistory.Columns[6].Visibility = Visibility.Hidden;
                dgHistory.Columns[8].Visibility = Visibility.Hidden;
                dgHistory.Columns[11].Visibility = Visibility.Hidden;
            }
        }

        private void dgHistory_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "action")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btDetailHistory));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("action"));
                buttonFactory.SetValue(Button.ContentProperty, "Detail");
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
        }
        private void btDetailHistory(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(((Button)sender).CommandParameter.ToString());
            HTrans selHT = null;
            foreach (HTrans h in histories)
            {
                if (h.nomor_nota == ((Button)sender).CommandParameter.ToString())
                {
                    selHT = h;
                    break;
                }
            }
            WindowDetailHistory dh = new WindowDetailHistory(selHT);
            overlayOn();
            dh.ShowDialog();
            overlayOff();
        }
    }
}
