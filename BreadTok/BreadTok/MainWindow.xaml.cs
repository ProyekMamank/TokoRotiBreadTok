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
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bahan b;
        private Karyawan k;
        private int selectedIdBahan;
        private int selectedIdKaryawan;
        private int selectedIdRoti;
        string loggedUserID;
        List<Roti> rotis;
        public MainWindow(string id)
        {
            InitializeComponent();
            b = new bahan();
            k = new Karyawan();
            loggedUserID = id;

            if (Convert.ToInt32(loggedUserID) > 0)
            {
                OracleCommand cmd = new OracleCommand("SELECT NAMA FROM KARYAWAN WHERE ID = '" + loggedUserID + "'", App.conn);
                lbWelcome.Text = "Selamat Datang, " + cmd.ExecuteScalar().ToString() + "!";
            }
            else
            {
                lbWelcome.Text = "Selamat Datang, Admin!";
            }

            loadDataBahan();
            loadDataKaryawan();
            loadDataRoti();
            loadDaftarPesanan();
        }

        List<HTrans> htranses = new List<HTrans>();

        // GENERAL
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

        // DAFTAR PESANAN
        private void loadDaftarPesanan()
        {
            OracleCommand cmd = new OracleCommand("SELECT H.NOMOR_NOTA, INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD MONTH YYYY')), H.TOTAL, P.NAMA, H.METODE_PEMBAYARAN, " +
                                                    "(CASE WHEN H.STATUS = 0 THEN 'Belum Bayar' " +
                                                    "       WHEN H.STATUS = 1 THEN 'Request Bayar' " +
                                                    "       WHEN H.STATUS = 2 THEN 'Sudah Bayar' " +
                                                    "       WHEN H.STATUS = 3 THEN 'Dibatalkan' " +
                                                    "END) AS STATUS " +
                                                    "FROM H_TRANS H, PELANGGAN P " +
                                                    "WHERE H.FK_PELANGGAN = P.ID " +
                                                    "ORDER BY H.NOMOR_NOTA", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();

            htranses = new List<HTrans>();
            while (reader.Read()){
                htranses.Add(new HTrans()
                {
                    nomor_nota = reader.GetValue(0).ToString(),
                    tanggal_trans = reader.GetValue(1).ToString(),
                    total = Convert.ToInt32(reader.GetValue(2).ToString()),
                    id_karyawan = "",
                    id_pelanggan = reader.GetValue(3).ToString(),
                    metode_pembayaran = reader.GetValue(4).ToString(),
                    status = reader.GetValue(5).ToString(),
                    kode_voucher = "-"
                });
            }
            reader.Close();

            dtGridPesanan.ItemsSource = htranses;

        }

        private void BtnDetailHTrans_Click(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowPesanan wp = new WindowPesanan(ID.ToString(), loggedUserID);
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

        private void loadDataBahan()
        {
            dgBahan.ItemsSource = null;
            dgBahan.ItemsSource = b.loadData().DefaultView;
        }

        private void loadDataKaryawan()
        {
            dgKaryawan.ItemsSource = null;
            dgKaryawan.ItemsSource = k.loadData().DefaultView;
        }

        private void loadDataRoti()
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

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Hidden;
            panelInsertBahan.Visibility = Visibility.Visible;
            panelUpdateBahan.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;
            btnInsert.Visibility = Visibility.Hidden;
            btnBack.Visibility = Visibility.Visible;
            loadCbJenisBahan("insert");
            loadCbSupplier("insert");
            loadImage(imgInsertbahan, "\\Resources\\ImagePlaceholder.png");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Visible;
            panelInsertBahan.Visibility = Visibility.Hidden;
            panelUpdateBahan.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Visible;
            btnInsert.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
            selectedIdBahan = -1;
        }

        private void dgBahan_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Update")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnUpdate_Click));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("Update"));
                buttonFactory.SetValue(Button.ContentProperty, "Update");
                buttonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Colors.Green));
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
            else if(e.PropertyName == "Detail")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnOpenWindowBahan));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("Detail"));
                buttonFactory.SetValue(Button.ContentProperty, "Detail");
                buttonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Colors.CadetBlue));
                buttonFactory.SetValue(Button.WidthProperty, 80.0);
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
        }
        
        private void DgBahan_Loaded(object sender, RoutedEventArgs e)
        {
            dgBahan.Columns[0].Width = DataGridLength.SizeToCells;
            dgBahan.Columns[1].Width = DataGridLength.SizeToCells;
            dgBahan.Columns[2].Width = DataGridLength.Auto;
            dgBahan.Columns[3].Width = DataGridLength.Auto;
            dgBahan.Columns[4].Width = DataGridLength.SizeToCells;
            dgBahan.Columns[5].Width = DataGridLength.SizeToCells;
        }

        private void btnOpenWindowBahan(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowBahan wp = new WindowBahan(ID.ToString());
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wp.ShowDialog();

            //loadDaftarPesanan();
            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Hidden;
            panelInsertBahan.Visibility = Visibility.Hidden;
            panelUpdateBahan.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Hidden;
            btnBack.Visibility = Visibility.Visible;
            btnInsert.Visibility = Visibility.Hidden;
            loadCbJenisBahan("update");
            loadCbSupplier("update");
            setupUpdatePanel(Convert.ToInt32((sender as Button).CommandParameter));
            selectedIdBahan = Convert.ToInt32((sender as Button).CommandParameter);
        }

        private void dgBahan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedIdBahan = Convert.ToInt32((((DataGrid)sender).SelectedItem as DataRowView)[5]);
            Console.WriteLine(((DataGrid)sender).SelectedItem);
        }


        private void loadCbJenisBahan(string state)
        {
            if(state == "insert")
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
            else
            {
                cbJenisBahanUpdate.Items.Clear();
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from jenis_bahan";
                cmd.Connection = App.conn;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbJenisBahanUpdate.Items.Add(new ComboBoxItem()
                    {
                        Name = "ID" + reader.GetString(0),
                        Content = reader.GetString(1)
                    });
                }
                reader.Close();
                cbJenisBahanUpdate.SelectedValuePath = "Name";
                cbJenisBahanUpdate.SelectedIndex = 0;
            }
            
        }

        private void loadCbSupplier(string state)
        {
            if (state == "insert")
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
                        Content = reader.GetString(2)
                    });
                }
                reader.Close();
                cbSupplier.SelectedValuePath = "Name";
                cbSupplier.SelectedIndex = 0;
            }
            else
            {
                cbSupplierUpdate.Items.Clear();
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from supplier";
                cmd.Connection = App.conn;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbSupplierUpdate.Items.Add(new ComboBoxItem()
                    {
                        Name = "ID" + reader.GetString(0),
                        Content = reader.GetString(2)
                    });
                }
                reader.Close();
                cbSupplierUpdate.SelectedValuePath = "Name";
                cbSupplierUpdate.SelectedIndex = 0;
            }
            
        }

        private void loadCbJabatan()
        {
            cbJabatan.Items.Clear();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from jabatan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbJabatan.Items.Add(new ComboBoxItem()
                {
                    Name = "ID" + reader.GetString(0),
                    Content = reader.GetString(1)
                });
            }
            reader.Close();
            cbJabatan.SelectedValuePath = "Name";
            cbJabatan.SelectedIndex = 0;
        }

        private void loadCbJenisRoti()
        {
            cbJenisRoti.Items.Clear();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from jenis_roti";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbJenisRoti.Items.Add(new ComboBoxItem()
                {
                    Name = "ID" + reader.GetString(0),
                    Content = reader.GetString(1)
                });
            }
            reader.Close();
            cbJenisRoti.SelectedValuePath = "Name";
            cbJenisRoti.SelectedIndex = 0;
        }

        string bahanImgSourceDir = "";
        private void BtnOpenImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Title = "Open Image";
            openFileDialog.Filter = "Image Files(*.jpg,*.png,*.tiff,*.bmp,*.gif)|*.jpg;*.png;*.tiff;*.bmp;*.gif";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                bahanImgSourceDir = selectedFileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                imgInsertbahan.Source = bitmap;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (tbMerk.Text != "" && tbQuantity.Text != "" && tbHarga.Text != "" && bahanImgSourceDir != "")
            {
                if (Regex.IsMatch(tbQuantity.Text, @"^\d+$") && Regex.IsMatch(tbHarga.Text, @"^\d+$"))
                {
                    if (Convert.ToInt32(tbQuantity.Text) > 0)
                    {
                        if (Convert.ToInt32(tbHarga.Text) > 0)
                        {
                            string merk = tbMerk.Text.ToUpper();
                            int qty = Convert.ToInt32(tbQuantity.Text);
                            int harga = Convert.ToInt32(tbHarga.Text);
                            string satuan = cbSatuan.SelectedItem.ToString();
                            string jenisBahan = cbJenisBahan.SelectedValue.ToString().Substring(2);
                            string supplier = cbSupplier.SelectedValue.ToString().Substring(2);
                        
                            OracleCommand cmd = new OracleCommand();
                            cmd.CommandText = "insert into bahan values(:1,:2,:3,:4,:5,:6,:7,:8,:9)";
                            cmd.Connection = App.conn;
                            cmd.Parameters.Add(":1", "0"); //id
                            cmd.Parameters.Add(":2", "0"); //kode
                            cmd.Parameters.Add(":3", merk);
                            cmd.Parameters.Add(":4", qty);
                            cmd.Parameters.Add(":5", harga);
                            cmd.Parameters.Add(":6", satuan);
                            cmd.Parameters.Add(":7", jenisBahan);
                            cmd.Parameters.Add(":8", supplier);
                            cmd.Parameters.Add(":9", "0"); //pic_loc
                            cmd.ExecuteNonQuery();
                        
                            cmd = new OracleCommand();
                            cmd.CommandText = "select picture_location from bahan group by id, picture_location having id = (select max(to_number(id)) from bahan)";
                            cmd.Connection = App.conn;
                            saveImage(bahanImgSourceDir, "\\Resources\\Bahan\\", cmd.ExecuteScalar().ToString());

                            MessageHandler.messageSuccess("Insert Bahan");

                            resetInsertPanel();
                            loadDataBahan();

                            panelMasterBahan.Visibility = Visibility.Visible;
                            panelInsertBahan.Visibility = Visibility.Hidden;
                            btnInsert.Visibility = Visibility.Visible;
                            btnDelete.Visibility = Visibility.Visible;
                            btnBack.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            MessageHandler.mustBeBigger("Harga ", 0);
                        }
                    }
                    else
                    {
                        MessageHandler.mustBeBigger("Harga ", 0);
                    }
                }
                else
                {
                    MessageHandler.isNotNumber("Quantity and Price");
                }
            }
            else
            {
                MessageHandler.requireField();
            }
        }

        bool gantiFotoBahan = false;
        private void BtnOpenImgBahanUpdate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Title = "Open Image";
            openFileDialog.Filter = "Image Files(*.jpg,*.png,*.tiff,*.bmp,*.gif)|*.jpg;*.png;*.tiff;*.bmp;*.gif";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                bahanImgSourceDir = selectedFileName;
                gantiFotoBahan = true;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                imgUpdateBahan.Source = bitmap;
            }
        }
       
        private void setupUpdatePanel(int id)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select * from bahan where ID = {id}";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tbMerkUpdate.Text = reader.GetValue(2).ToString();
                tbQuantityUpdate.Text = reader.GetValue(3).ToString();
                if(reader.GetValue(5).ToString() == "GRAM")
                {
                    cbSatuanUpdate.SelectedIndex = 0;
                }
                else if(reader.GetValue(5).ToString() == "mL")
                {
                    cbSatuanUpdate.SelectedIndex = 1;
                }
                else
                {
                    cbSatuanUpdate.SelectedIndex = 2;
                }
                tbHargaUpdate.Text = reader.GetValue(4).ToString();
                int idxJenisBahan = 0;
                foreach(ComboBoxItem cbItem in cbJenisBahanUpdate.Items)
                {
                    if(cbItem.Name == "ID" + reader.GetString(6))
                    {
                        break;
                    }
                    idxJenisBahan++;
                }
                int idxSupplier = 0;
                foreach (ComboBoxItem cbItem in cbSupplierUpdate.Items)
                {
                    if (cbItem.Name == "ID" + reader.GetString(7))
                    {
                        break;
                    }
                    idxSupplier++;
                }
                cbSupplierUpdate.SelectedIndex = idxSupplier;
                cbJenisBahanUpdate.SelectedIndex = idxJenisBahan;

                bahanImgSourceDir = reader.GetString(8);
                loadImage(imgUpdateBahan, "\\Resources\\Bahan\\" + bahanImgSourceDir);
            }
            reader.Close();
        }
        private void resetInsertPanel()
        {
            bahanImgSourceDir = "";
            tbMerk.Text = "";
            tbQuantity.Text = "";
            tbHarga.Text = "";
            tbQuantity.Text = "";
            cbSatuan.SelectedIndex = 0;
            selectedIdBahan = -1;
        }
        private void resetUpdatePanel()
        {
            gantiFotoBahan = false;
            bahanImgSourceDir = "";
            tbMerkUpdate.Text = "";
            tbQuantityUpdate.Text = "";
            tbHargaUpdate.Text = "";
            tbQuantityUpdate.Text = "";
            cbSatuanUpdate.SelectedIndex = 0;
            selectedIdBahan = -1;
        }
        private void resetInsertKaryawanPanel()
        {
            tbNamaKaryawan.Text = "";
            tbUsernameKaryawan.Text = "";
            tbPasswordKaryawan.Text = "";
            tbEmailKaryawan.Text = "";
            tbAlamatKaryawan.Text = "";
            tbNoTelpKaryawan.Text = "";
            dtpTanggalLahir.Text = "";
            rbLakiKaryawan.IsChecked = false;
            rbPerempuanKaryawan.IsChecked = false;
            cbJabatan.SelectedIndex = 0;
            selectedIdKaryawan = -1;
        }

        private void btnSubmitUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbMerkUpdate.Text != "" && tbQuantityUpdate.Text != "" && tbHargaUpdate.Text != "")
            {
                if (Regex.IsMatch(tbQuantityUpdate.Text, @"^\d+$") && Regex.IsMatch(tbHargaUpdate.Text, @"^\d+$"))
                {
                    if (Convert.ToInt32(tbQuantityUpdate.Text) > 0)
                    {
                        if (Convert.ToInt32(tbHargaUpdate.Text) > 0)
                        {
                            string merk = tbMerkUpdate.Text.ToUpper();
                            int qty = Convert.ToInt32(tbQuantityUpdate.Text);
                            int harga = Convert.ToInt32(tbHargaUpdate.Text);
                            string satuan = cbSatuanUpdate.SelectedItem.ToString();
                            string jenisBahan = cbJenisBahanUpdate.SelectedValue.ToString().Substring(2);
                            string supplier = cbSupplierUpdate.SelectedValue.ToString().Substring(2);


                            OracleCommand cmd = new OracleCommand();
                            cmd.CommandText = $"update bahan set merk=:1,qty_stok=:2,harga=:3,satuan=:4,jenis_bahan=:5,fk_supplier=:6 where ID={selectedIdBahan}";
                            cmd.Connection = App.conn;
                            cmd.Parameters.Add(":1", merk);
                            cmd.Parameters.Add(":2", qty);
                            cmd.Parameters.Add(":3", harga);
                            cmd.Parameters.Add(":4", satuan);
                            cmd.Parameters.Add(":5", jenisBahan);
                            cmd.Parameters.Add(":6", supplier);
                            cmd.ExecuteNonQuery();


                            cmd = new OracleCommand();
                            cmd.CommandText = $"select picture_location from bahan where id = {selectedIdBahan}";
                            cmd.Connection = App.conn;
                            string kodeBahanUpdating = cmd.ExecuteScalar().ToString();
                            if (gantiFotoBahan)
                            {
                                deleteImage(imgUpdateBahan, "\\Resources\\Bahan\\" + kodeBahanUpdating);
                                saveImage(bahanImgSourceDir, "\\Resources\\Bahan\\", kodeBahanUpdating);
                            }
                            else
                            {
                                if(bahanImgSourceDir != kodeBahanUpdating)
                                {
                                    var enviroment = System.Environment.CurrentDirectory;
                                    string imgSrc = Directory.GetParent(enviroment).Parent.FullName + "\\Resources\\Bahan\\" + bahanImgSourceDir;
                                    saveImage(imgSrc, "\\Resources\\Bahan\\", kodeBahanUpdating);
                                    MessageBox.Show("stop");
                                    deleteImage(imgUpdateBahan, "\\Resources\\Bahan\\" + bahanImgSourceDir);
                                }
                            }

                            MessageHandler.messageSuccess("Update Bahan");

                            resetUpdatePanel();
                            loadDataBahan();

                            panelMasterBahan.Visibility = Visibility.Visible;
                            panelUpdateBahan.Visibility = Visibility.Hidden;
                            btnInsert.Visibility = Visibility.Visible;
                            btnDelete.Visibility = Visibility.Visible;
                            btnBack.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            MessageHandler.mustBeBigger("Harga ", 0);
                        }
                    }
                    else
                    {
                        MessageHandler.mustBeBigger("Harga ", 0);
                    }
                }
                else
                {
                    MessageHandler.isNotNumber("Quantity and Price");
                }
            }
            else
            {
                MessageHandler.requireField();
            }
        }

        private void dgKaryawan_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if((((DataGrid)sender).SelectedItem as DataRowView) != null)
            {
                string username = (((DataGrid)sender).SelectedItem as DataRowView)[1].ToString();
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = $"select ID,status from karyawan where username='{username}'";
                cmd.Connection = App.conn;
                OracleDataReader reader = cmd.ExecuteReader();
                Console.WriteLine(username);
                while (reader.Read())
                {
                    selectedIdKaryawan = Convert.ToInt32(reader.GetValue(0).ToString());
                    if (reader.GetValue(1).ToString() == "1")
                    {
                        btnActive.IsEnabled = false;
                        btnSuspend.IsEnabled = true;
                    }
                    else
                    {
                        btnActive.IsEnabled = true;
                        btnSuspend.IsEnabled = false;
                    }
                }
                reader.Close();
            }
        }

        private void tabMasterKaryawan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedIdKaryawan = -1;
            btnActive.IsEnabled = false;
            btnSuspend.IsEnabled = false;
        }

        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            if(selectedIdKaryawan >= 0)
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = $"update karyawan set status=1 where ID={selectedIdKaryawan}";
                cmd.Connection = App.conn;
                cmd.ExecuteNonQuery();
                selectedIdKaryawan = -1;
                loadDataKaryawan();
            }
            else
            {
                MessageBox.Show("TOLONG PILIH KARYAWAN TERLEBIH DAHULU");
            }
            
        }

        private void btnSuspend_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdKaryawan >= 0)
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = $"update karyawan set status=0 where ID={selectedIdKaryawan}";
                cmd.Connection = App.conn;
                cmd.ExecuteNonQuery();
                selectedIdKaryawan = -1;
                loadDataKaryawan();
            }
            else
            {
                MessageBox.Show("TOLONG PILIH KARYAWAN TERLEBIH DAHULU");
            }
        }

        private void btnBackKaryawan_Click(object sender, RoutedEventArgs e)
        {
            panelMasterKaryawan.Visibility = Visibility.Visible;
            panelInsertKaryawan.Visibility = Visibility.Hidden;
            btnActive.Visibility = Visibility.Visible;
            btnSuspend.Visibility = Visibility.Visible;
            btnInsertKaryawan.Visibility = Visibility.Visible;
            btnBackKaryawan.Visibility = Visibility.Hidden;
            selectedIdKaryawan = -1;
        }

        private void btnInsertKaryawan_Click(object sender, RoutedEventArgs e)
        {
            panelMasterKaryawan.Visibility = Visibility.Hidden;
            panelInsertKaryawan.Visibility = Visibility.Visible;
            btnActive.Visibility = Visibility.Hidden;
            btnSuspend.Visibility = Visibility.Hidden;
            btnInsertKaryawan.Visibility = Visibility.Hidden;
            btnBackKaryawan.Visibility = Visibility.Visible;
            selectedIdKaryawan = -1;
            loadCbJabatan();
        }

        private void btnSubmitKaryawan_Click(object sender, RoutedEventArgs e)
        {
            if (dtpTanggalLahir.SelectedDate == null)
            {
                MessageBox.Show("PLEASE FILL OUT ALL THE FIELD FIRST!!!");
                return;
            }

            string nama = tbNamaKaryawan.Text;
            string username = tbUsernameKaryawan.Text;
            string password = tbPasswordKaryawan.Text;
            string email = tbEmailKaryawan.Text;
            string jenisKelamin = "";
            if(rbLakiKaryawan.IsChecked == true)
            {
                jenisKelamin = "L";
            }else if(rbPerempuanKaryawan.IsChecked == true)
            {
                jenisKelamin = "P";
            }
            string alamat = tbAlamatKaryawan.Text;
            string noTelp = tbNoTelpKaryawan.Text;
            DateTime tglLahir = dtpTanggalLahir.SelectedDate.Value;
            string jabatan = cbJabatan.SelectedValue.ToString().Substring(2);

            if (nama == "" || username == "" || password=="" || email=="" || jenisKelamin=="" || alamat=="" || noTelp=="")
            {
                MessageBox.Show("PLEASE FILL OUT ALL THE FIELD FIRST!!!");
                return;
            }

            if (!noTelp.All(Char.IsDigit))
            {
                MessageBox.Show("NOTELP MUST CONSIST ALL NUMBERS!!!");
                return;
            }

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "insert into karyawan values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11,:12,:13)";
            cmd.Connection = App.conn;
            cmd.Parameters.Add(":1", "0");
            cmd.Parameters.Add(":2", "0");
            cmd.Parameters.Add(":3", username);
            cmd.Parameters.Add(":4", password);
            cmd.Parameters.Add(":5", nama);
            cmd.Parameters.Add(":6", jenisKelamin);
            cmd.Parameters.Add(":7", alamat);
            cmd.Parameters.Add(":8", email);
            cmd.Parameters.Add(":9", noTelp);
            cmd.Parameters.Add(":10", tglLahir);
            cmd.Parameters.Add(":11", 1);
            cmd.Parameters.Add(":12", jabatan);
            cmd.Parameters.Add(":13", "0");
            cmd.ExecuteNonQuery();

            resetInsertKaryawanPanel();
            loadDataKaryawan();

            panelInsertKaryawan.Visibility = Visibility.Hidden;
            panelMasterKaryawan.Visibility = Visibility.Visible;
            btnBackKaryawan.Visibility = Visibility.Hidden;
            btnActive.Visibility = Visibility.Visible;
            btnSuspend.Visibility = Visibility.Visible;
            btnInsertKaryawan.Visibility = Visibility.Visible;
        }
        
        private void deleteImage(Image img, string path)
        {
            var enviroment = System.Environment.CurrentDirectory;
            string imgDist = Directory.GetParent(enviroment).Parent.FullName + path;
            
            img.Source = null;
            File.Delete(imgDist);
        }

        private void loadImage(Image img, string path)
        {
            var enviroment = System.Environment.CurrentDirectory;
            string imgDist = Directory.GetParent(enviroment).Parent.FullName + path;

            using (var stream = File.OpenRead(imgDist))
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = stream;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                img.Source = bmp;
            }
        }

        private void saveImage(string imgSrcDir, string imgDestDir, string filename)
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
            try
            {
                File.Copy(imgSrcDir, projectDirectory + imgDestDir + filename, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnUpdateRoti_Click(object sender, RoutedEventArgs e)
        {
            panelMasterRoti.Visibility = Visibility.Hidden;
            panelUpdateRoti.Visibility = Visibility.Visible;
            btnDeleteRoti.Visibility = Visibility.Hidden;
            btnBackRoti.Visibility = Visibility.Visible;
            loadCbJenisRoti();
            setupUpdatePanelRoti(Convert.ToInt32((sender as Button).CommandParameter));
            selectedIdRoti = Convert.ToInt32((sender as Button).CommandParameter);
        }

        private void btnBackRoti_Click(object sender, RoutedEventArgs e)
        {
            panelMasterRoti.Visibility = Visibility.Visible;
            panelUpdateRoti.Visibility = Visibility.Hidden;
            btnDeleteRoti.Visibility = Visibility.Visible;
            btnBackRoti.Visibility = Visibility.Hidden;
            selectedIdRoti = -1;
        }

        string rotiImgSourceDir = "";
        private void setupUpdatePanelRoti(int id)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select * from roti where ID = {id}";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tbNamaRoti.Text = reader.GetValue(2).ToString();
                tbDeskripsiRoti.Text = reader.GetValue(3).ToString();
                tbHargaRoti.Text = reader.GetValue(4).ToString();
                int idxJenisRoti = 0;
                foreach (ComboBoxItem cbItem in cbJenisRoti.Items)
                {
                    if (cbItem.Name == "ID" + reader.GetString(7))
                    {
                        break;
                    }
                    idxJenisRoti++;
                }
                cbJenisRoti.SelectedIndex = idxJenisRoti;

                rotiImgSourceDir = reader.GetString(9);
                loadImage(imgUpdateRoti, "\\Resources\\Roti\\" + rotiImgSourceDir);

            }
            reader.Close();
        }

        private void dgRoti_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selectedIdRoti = Convert.ToInt32((((DataGrid)sender).SelectedItem as Roti).id_roti);
            Console.WriteLine(selectedIdRoti);
        }

        private void btnDeleteRoti_Click(object sender, RoutedEventArgs e)
        {
            if(selectedIdRoti < 0)
            {
                MessageBox.Show("PLEASE CHOOSE ROTI FIRST!!!");
                return;
            }

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"update roti set status = 0 where ID='{selectedIdRoti}'";
            cmd.Connection = App.conn;
            cmd.ExecuteNonQuery();

            loadDataRoti();
            selectedIdRoti = -1;
        }

        bool gantiFotoRoti = false;
        private void btnOpenImgRotiUpdate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Title = "Open Image";
            openFileDialog.Filter = "Image Files(*.jpg,*.png,*.tiff,*.bmp,*.gif)|*.jpg;*.png;*.tiff;*.bmp;*.gif";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                rotiImgSourceDir = selectedFileName;
                gantiFotoRoti = true;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                imgUpdateRoti.Source = bitmap;
            }
        }

        private void btnSubmitRoti_Click(object sender, RoutedEventArgs e)
        {
            string nama = tbNamaRoti.Text;
            string deskripsi = tbDeskripsiRoti.Text;
            string harga = tbHargaRoti.Text;
            string jenisRoti = cbJenisRoti.SelectedValue.ToString().Substring(2);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select picture_location from roti where id = {selectedIdRoti}";
            cmd.Connection = App.conn;
            string kodeRotiLama = cmd.ExecuteScalar().ToString();

            if (nama == "" || deskripsi == "" || harga == "" || jenisRoti == "")
            {
                MessageBox.Show("PLEASE FILL OUT ALL THE FIELD FIRST!!!");
                return;
            }

            if (!harga.All(Char.IsDigit))
            {
                MessageBox.Show("HARGA MUST CONSIST ALL NUMBERS!!!");
                return;
            }

            cmd = new OracleCommand();
            cmd.CommandText = "update roti set kode=:1,nama=:2,deskripsi=:3,harga=:4,jenis_roti=:5 where id=:6";
            cmd.Connection = App.conn;
            cmd.Parameters.Add(":1", "0");
            cmd.Parameters.Add(":2", nama);
            cmd.Parameters.Add(":3", deskripsi);
            cmd.Parameters.Add(":4", Convert.ToInt32(harga));
            cmd.Parameters.Add(":5", jenisRoti);
            cmd.Parameters.Add(":6", selectedIdRoti);
            cmd.ExecuteNonQuery();

            cmd = new OracleCommand();
            cmd.CommandText = $"select picture_location from roti where id = {selectedIdRoti}";
            cmd.Connection = App.conn;
            string kodeRotiUpdating = cmd.ExecuteScalar().ToString();
            if (gantiFotoRoti)
            {
                deleteImage(imgUpdateRoti, "\\Resources\\Roti\\" + kodeRotiLama);
                saveImage(rotiImgSourceDir, "\\Resources\\Roti\\", kodeRotiUpdating);
            }
            else
            {
                if (rotiImgSourceDir != kodeRotiUpdating)
                {
                    var enviroment = System.Environment.CurrentDirectory;
                    string imgSrc = Directory.GetParent(enviroment).Parent.FullName + "\\Resources\\Roti\\" + rotiImgSourceDir;
                    saveImage(imgSrc, "\\Resources\\Roti\\", kodeRotiUpdating);
                    MessageBox.Show("stop");
                    deleteImage(imgUpdateRoti, "\\Resources\\Roti\\" + rotiImgSourceDir);
                }
            }

            selectedIdRoti = -1;
            loadDataRoti();

            panelUpdateRoti.Visibility = Visibility.Hidden;
            panelMasterRoti.Visibility = Visibility.Visible;
            btnBackRoti.Visibility = Visibility.Hidden;
            btnDeleteRoti.Visibility = Visibility.Visible;
        }
    }
}
