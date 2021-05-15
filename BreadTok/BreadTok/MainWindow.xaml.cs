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
        string loggedUserID;
        public MainWindow(string id)
        {
            InitializeComponent();
            b = new bahan();
            k = new Karyawan();
            loggedUserID = id;
            loadDataBahan();
            loadDataKaryawan();
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
            loadPlaceHolder(imgInsert);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            panelMasterBahan.Visibility = Visibility.Visible;
            panelInsertBahan.Visibility = Visibility.Hidden;
            panelUpdateBahan.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Visible;
            btnInsert.Visibility = Visibility.Visible;
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
                        Content = reader.GetString(1)
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
                        Content = reader.GetString(1)
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
                imgInsert.Source = bitmap;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // TODO : pengecekan harus diisi + harus ada foto (cek imgSourceDir != "")
            string merk = tbMerk.Text.ToUpper();
            int qty = Convert.ToInt32(tbQuantity.Text);
            int harga = Convert.ToInt32(tbHarga.Text);
            string satuan = cbSatuan.SelectedItem.ToString();
            string jenisBahan = cbJenisBahan.SelectedValue.ToString().Substring(2);
            string supplier = cbSupplier.SelectedValue.ToString().Substring(2);

            // TODO : TRIGGER ID + KODE + Pic_Loc
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

            // TODO : GANTI NAMA GAMBAR
            saveImage(bahanImgSourceDir, "Resources\\Bahan\\", "tes.jpg");

            resetInsertPanel();
            loadDataBahan();

            panelMasterBahan.Visibility = Visibility.Visible;
            panelInsertBahan.Visibility = Visibility.Hidden;
            btnInsert.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
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

            }
            reader.Close();
        }
        private void resetInsertPanel()
        {
            tbMerk.Text = "";
            tbQuantity.Text = "";
            tbHarga.Text = "";
            tbQuantity.Text = "";
            cbSatuan.SelectedIndex = 0;
            selectedIdBahan = -1;
        }
        private void resetUpdatePanel()
        {
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

            resetUpdatePanel();
            loadDataBahan();

            panelMasterBahan.Visibility = Visibility.Visible;
            panelUpdateBahan.Visibility = Visibility.Hidden;
            btnInsert.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Hidden;
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
                MessageBox.Show("PLEAS FILL OUT ALL THE FIELD FIRST!!!");
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
                MessageBox.Show("PLEAS FILL OUT ALL THE FIELD FIRST!!!");
                return;
            }

            if (!noTelp.All(Char.IsDigit))
            {
                MessageBox.Show("NOTELP MUST CONSIST ALL NUMBERS!!!");
                return;
            }

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "insert into karyawan values(:1,:2,:3,:4,:5,:6,:7,:8,:9,:10,:11)";
            cmd.Connection = App.conn;
            cmd.Parameters.Add(":1", "0");
            cmd.Parameters.Add(":2", username);
            cmd.Parameters.Add(":3", password);
            cmd.Parameters.Add(":4", nama);
            cmd.Parameters.Add(":5", jenisKelamin);
            cmd.Parameters.Add(":6", alamat);
            cmd.Parameters.Add(":7", email);
            cmd.Parameters.Add(":8", noTelp);
            cmd.Parameters.Add(":9", tglLahir);
            cmd.Parameters.Add(":10", 1);
            cmd.Parameters.Add(":11", jabatan);
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
        
        private void loadPlaceHolder(Image img)
        {
            var enviroment = System.Environment.CurrentDirectory;
            string imgPlaceholder = Directory.GetParent(enviroment).Parent.FullName + "\\Resources\\ImagePlaceholder.png";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgPlaceholder);
            bitmap.EndInit();
            img.Source = bitmap;
        }

        private void saveImage(string imgSrcDir, string imgDestDir, string filename)
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName + "\\";
            try
            {
                File.Copy(imgSrcDir, projectDirectory + imgDestDir + filename, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
