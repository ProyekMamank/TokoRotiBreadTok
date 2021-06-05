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
using System.ComponentModel;
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
        private Supplier s;
        private Roti r;
        private DataTable cartBahan, pengadaanBahan, pengadaanSupplier, chefRoti, chefResep;
        private DataRow itemDataSupport;
        private int selectedIdBahan;
        private int selectedIdKaryawan;
        private int selectedIdRoti;
        string loggedUserID;
        List<Roti> rotis;
        List<Resep> reseps;
        private int idhresep;
        private string koderoti;
        public MainWindow(string id, string jabatan)
        {
            InitializeComponent();
            b = new bahan();
            k = new Karyawan();
            s = new Supplier();
            r = new Roti();
            itemDataSupport = null;
            cartBahan = new DataTable();
            pengadaanBahan = new DataTable();
            pengadaanSupplier = new DataTable();
            chefRoti = new DataTable();
            chefResep = new DataTable();
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
            loadDataResep();
            loadDaftarPesanan();
            initPengadaanBahan();
            initChef();

            if (jabatan == "KARYAWAN")
            {
                tabMasterBahan.Visibility = Visibility.Collapsed;
                tabMasterResep.Visibility = Visibility.Collapsed;
                tabMasterRoti.Visibility = Visibility.Collapsed;
                tabMasterKaryawan.Visibility = Visibility.Collapsed;
                tabMasterVoucher.Visibility = Visibility.Collapsed;
                tabDaftarPesanan.Visibility = Visibility.Visible;
                tabPengadaanBahan.Visibility = Visibility.Visible;
                tabChef.Visibility = Visibility.Collapsed;
                tabDaftarPesanan.IsSelected = true;
            }
            else if (jabatan == "MANAGER")
            {
                tabMasterBahan.Visibility = Visibility.Collapsed;
                tabMasterResep.Visibility = Visibility.Collapsed;
                tabMasterRoti.Visibility = Visibility.Collapsed;
                tabMasterKaryawan.Visibility = Visibility.Visible;
                tabMasterVoucher.Visibility = Visibility.Visible;
                tabDaftarPesanan.Visibility = Visibility.Visible;
                tabPengadaanBahan.Visibility = Visibility.Collapsed;
                tabChef.Visibility = Visibility.Collapsed;
                tabMasterKaryawan.IsSelected = true;
            }
            else if (jabatan == "CHEF")
            {
                tabMasterBahan.Visibility = Visibility.Collapsed;
                tabMasterResep.Visibility = Visibility.Collapsed;
                tabMasterRoti.Visibility = Visibility.Hidden;
                tabMasterKaryawan.Visibility = Visibility.Collapsed;
                tabMasterVoucher.Visibility = Visibility.Collapsed;
                tabDaftarPesanan.Visibility = Visibility.Collapsed;
                tabPengadaanBahan.Visibility = Visibility.Visible;
                tabChef.Visibility = Visibility.Visible;
                tabPengadaanBahan.IsSelected = true;
            }
            else if (jabatan == "ADMIN")
            {
                tabMasterBahan.Visibility = Visibility.Visible;
                tabMasterResep.Visibility = Visibility.Visible;
                tabMasterRoti.Visibility = Visibility.Visible;
                tabMasterKaryawan.Visibility = Visibility.Visible;
                tabMasterVoucher.Visibility = Visibility.Visible;
                tabDaftarPesanan.Visibility = Visibility.Visible;
                tabPengadaanBahan.Visibility = Visibility.Visible;
                tabChef.Visibility = Visibility.Visible;
                tabMasterBahan.IsSelected = true;
            }
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

        //PENGADAAN BAHAN
        private void resetPengadaanDetailBahan()
        {
            if(cartBahan.Rows.Count == 0) {
                dgPengadaanBahan.ItemsSource = null;
                dgPengadaanBahan.ItemsSource = pengadaanBahan.DefaultView;
                dgCartBahan.ItemsSource = null;
                bPengadaanBayar.IsEnabled = false;
                bClearCartBahan.IsEnabled = false;
                cbPengadaanSupplier.IsEnabled = false;
                lbBeliBahanGrandTotal.Text = "Rp -";
                initPengadaanBahan();
            }
            tbDetailBahanQuantity.IsEnabled = false;
            bHapusCartBahan.IsEnabled = false;
            bPengadaanUpdate.IsEnabled = false;
            tbDetailBahanQuantity.Text = "0";
            lbSubtotalBeliBahan.Text = "Rp -";
            lbDetailBahanNamaBahan.Text = "-";
            lbDetailBahanKodeBahan.Text = "-";
            lbDetailBahanStokSekarang.Text = "-";
            lbDetailBahanHargaSatuan.Text = "Rp -";
           
        }
        
        private void initPengadaanBahan()
        {
            dgPengadaanBahan.ItemsSource = null;
            pengadaanBahan = b.fillDataTable("B.KODE AS KODE, JB.NAMA_JENIS || ' - ' || B.MERK AS BAHAN, B.QTY_STOK || ' ' || B.SATUAN AS STOK, B.HARGA ", "", new DataTable());
            dgPengadaanBahan.ItemsSource = pengadaanBahan.DefaultView;
            pengadaanSupplier = s.GetDataTable("ID, NAMA");
            cbPengadaanSupplier.ItemsSource = null;
            cbPengadaanSupplier.ItemsSource = pengadaanSupplier.DefaultView;
            cbPengadaanSupplier.DisplayMemberPath = pengadaanSupplier.Columns["NAMA"].ToString();
            cbPengadaanSupplier.SelectedValuePath = "ID";
        }
        private void updateCartBahanGrandTotal()
        {
            int grandtotal = 0;
            for (int i = 0; i < cartBahan.Rows.Count; i++)
            {
                int subtotal = Convert.ToInt32(cartBahan.Rows[i]["SUBTOTAL"].ToString());
                grandtotal += subtotal;
            }
            lbBeliBahanGrandTotal.Text = CurrencyConverter.ToRupiah(grandtotal);
        }

        private void dgPengadaanBahan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgPengadaanBahan.SelectedIndex >= 0)
            {
                DataRowView selectedPengadaanBahan = dgPengadaanBahan.SelectedItem as DataRowView;
                b.fillDataTable("B.KODE AS KODE, JB.NAMA_JENIS || ' - ' || B.MERK AS BAHAN, '1' || ' ' || B.SATUAN AS JUMLAH, B.HARGA AS SUBTOTAL", $"WHERE B.KODE = '{selectedPengadaanBahan.Row.ItemArray[0].ToString()}'", cartBahan);
                dgCartBahan.ItemsSource = null;
                dgCartBahan.ItemsSource = cartBahan.DefaultView;
                updateCartBahanGrandTotal();
                for (int i = 0; i < pengadaanBahan.Rows.Count; i++)
                {
                    if (pengadaanBahan.Rows[i]["KODE"].ToString().Equals(selectedPengadaanBahan.Row.ItemArray[0].ToString()))
                    {
                        pengadaanBahan.Rows.RemoveAt(i);
                        break;
                    }
                }
                bClearCartBahan.IsEnabled = true;
                bPengadaanBayar.IsEnabled = true;
                cbPengadaanSupplier.IsEnabled = true;

                resetPengadaanDetailBahan();
            }

        }
        private void tbDetailBahanQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dgCartBahan.SelectedIndex >= 0)
            {
                if (tbDetailBahanQuantity.Text.Length == 0) tbDetailBahanQuantity.Text = "1";
                int harga = Convert.ToInt32(itemDataSupport[1].ToString());
                int jumlah = Convert.ToInt32(tbDetailBahanQuantity.Text);
                lbSubtotalBeliBahan.Text = CurrencyConverter.ToRupiah(harga * jumlah);
            }
        }
        private void dgCartBahan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(dgCartBahan.SelectedIndex >= 0)
            {
                DataRowView selectedCartItem = dgCartBahan.SelectedItem as DataRowView;
                bHapusCartBahan.IsEnabled = true;
                itemDataSupport = b.getOneRecordData("B.SATUAN AS SATUAN, B.HARGA AS HARGA, B.QTY_STOK AS STOK, B.ID AS ID", $"WHERE B.KODE = '{selectedCartItem.Row.ItemArray[0]}'");
                //selectedCartItem.Row.ItemArray[0]//KODE
                lbSubtotalBeliBahan.Text = CurrencyConverter.ToRupiah(Convert.ToInt32(selectedCartItem.Row.ItemArray[3].ToString()));
                string jumlah = selectedCartItem.Row.ItemArray[2].ToString();
                lbDetailBahanStokSekarang.Text = itemDataSupport[2].ToString();
                lbDetailBahanKodeBahan.Text = selectedCartItem.Row.ItemArray[0].ToString();
                lbDetailBahanHargaSatuan.Text = CurrencyConverter.ToRupiah(Convert.ToInt32(itemDataSupport[1].ToString()));
                tbDetailBahanQuantity.Text = jumlah.Substring(0,jumlah.IndexOf(' '));
                lbDetailBahanSuffix.Text = itemDataSupport[0].ToString();
                tbDetailBahanQuantity.IsEnabled = true;
                bPengadaanUpdate.IsEnabled = true;
                bHapusCartBahan.IsEnabled = true;
                lbDetailBahanNamaBahan.Text = selectedCartItem.Row.ItemArray[1].ToString();
            }
            else
            {
                resetPengadaanDetailBahan();
            }
        }

        private void bHapusCartBahan_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedCartItem = dgCartBahan.SelectedItem as DataRowView;
            for (int i = 0; i < cartBahan.Rows.Count; i++)
            {
                if (cartBahan.Rows[i]["KODE"].ToString().Equals(selectedCartItem.Row.ItemArray[0].ToString()))
                {
                    b.fillDataTable("B.KODE AS KODE, JB.NAMA_JENIS || ' - ' || B.MERK AS BAHAN, B.QTY_STOK || ' ' || B.SATUAN AS STOK, B.HARGA", $"WHERE B.KODE = '{selectedCartItem.Row.ItemArray[0].ToString()}'", pengadaanBahan);
                    dgPengadaanBahan.ItemsSource = null;
                    dgPengadaanBahan.ItemsSource = pengadaanBahan.DefaultView;
                    dgCartBahan.SelectedIndex = -1;
                    resetPengadaanDetailBahan();
                    cartBahan.Rows.RemoveAt(i);
                    break;
                }
            }
        }

        private void bPengadaanUpdate_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedCartItem = dgCartBahan.SelectedItem as DataRowView;
            for (int i = 0; i < cartBahan.Rows.Count; i++)
            {
                if (cartBahan.Rows[i]["KODE"].ToString().Equals(selectedCartItem.Row.ItemArray[0].ToString()))
                {
                    cartBahan.Rows[i]["SUBTOTAL"] = CurrencyConverter.ToAngka(lbSubtotalBeliBahan.Text) + "";
                    string jumlah = cartBahan.Rows[i]["JUMLAH"].ToString();
                    cartBahan.Rows[i]["JUMLAH"] = tbDetailBahanQuantity.Text+" "+ itemDataSupport[0].ToString();
                    dgCartBahan.SelectedIndex = -1;
                    updateCartBahanGrandTotal();
                    resetPengadaanDetailBahan();
                    break;
                }
            }
        }

        private void bPengadaanBayar_Click(object sender, RoutedEventArgs e)
        {
            if(cbPengadaanSupplier.SelectedIndex >= 0)
            {
                OracleTransaction trans;
                trans = App.conn.BeginTransaction();
                try
                {
                    OracleCommand cmd = new OracleCommand("SELECT 'BELI' || TO_CHAR(SYSDATE,'YYYYMMDD') || LPAD(COUNT(*)+1,3,'0') AS NOTA FROM H_BELI_BAHAN WHERE NOMOR_NOTA LIKE 'BELI' || TO_CHAR(SYSDATE, 'YYYYMMDD') || '%'", App.conn);
                    string nota = cmd.ExecuteScalar().ToString();
                    cmd = new OracleCommand($"INSERT INTO H_BELI_BAHAN VALUES('{nota}',SYSDATE,{CurrencyConverter.ToAngka(lbBeliBahanGrandTotal.Text)},'{cbPengadaanSupplier.SelectedValue.ToString()}')", App.conn);
                    cmd.ExecuteNonQuery();
                    for (int i = 0; i < cartBahan.Rows.Count; i++)
                    {
                        itemDataSupport = b.getOneRecordData("B.SATUAN AS SATUAN, B.HARGA AS HARGA, B.QTY_STOK AS STOK, B.ID AS ID", $"WHERE B.KODE = '{cartBahan.Rows[i][0].ToString()}'");
                        string jumlah = cartBahan.Rows[i]["JUMLAH"].ToString();
                        jumlah = jumlah.Substring(0, jumlah.IndexOf(' '));
                        cmd = new OracleCommand($"INSERT INTO D_BELI_BAHAN VALUES('{nota}','{itemDataSupport[3].ToString()}',{jumlah},{itemDataSupport[1].ToString()},{cartBahan.Rows[i]["SUBTOTAL"].ToString()})", App.conn);
                        cmd.ExecuteNonQuery();

                        cmd = new OracleCommand($"SELECT QTY_STOK AS STOK FROM BAHAN WHERE ID = '{itemDataSupport[3].ToString()}'", App.conn);
                        string stok = cmd.ExecuteScalar().ToString();

                        cmd = new OracleCommand($"UPDATE BAHAN SET QTY_STOK = {Convert.ToInt32(jumlah) + Convert.ToInt32(stok)} WHERE ID = '{itemDataSupport[3].ToString()}'", App.conn);
                        cmd.ExecuteNonQuery();

                    }
                    trans.Commit();
                    MessageBox.Show("Bahan berhasil ditambah");
                    cartBahan = new DataTable();
                    resetPengadaanDetailBahan();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.Rollback();
                }
            }
            else
            {
                MessageBox.Show("Supplier Belum dipilih");
            }
        }

        private void bClearCartBahan_Click(object sender, RoutedEventArgs e)
        {

            cartBahan = new DataTable();
            resetPengadaanDetailBahan();
        }

        private void updateStokBahan(object sender, MouseButtonEventArgs e)
        {
            
        }

        //[END] PENGADAAN BAHAN

        //CHEF

        private void initChef()
        {
            dgChefRoti.ItemsSource = null;
            chefRoti = new DataTable();
            chefRoti = r.fillDataTable("R.KODE AS KODE, R.NAMA AS NAMA, R.DESKRIPSI AS DESKRIPSI, JR.NAMA_JENIS AS JENIS, R.HARGA AS HARGA, R.STOK AS STOK", "WHERE R.STATUS = 1", chefRoti);
            dgChefRoti.ItemsSource = chefRoti.DefaultView;
            cbChefJenisRoti.ItemsSource = null;
            
            cbChefJenisRoti.ItemsSource = r.fillDataJenisRoti("ID, NAMA_JENIS", "", new DataTable()).DefaultView;
            cbChefJenisRoti.DisplayMemberPath = "NAMA_JENIS";
            cbChefJenisRoti.SelectedValuePath = "ID";
            cbChefBahan.ItemsSource = null;
            cbChefBahan.ItemsSource = b.fillDataTable("B.ID AS ID, B.MERK || ' - ' || JB.NAMA_JENIS AS NAMA", "", new DataTable()).DefaultView;
            cbChefBahan.DisplayMemberPath = "NAMA";
            cbChefBahan.SelectedValuePath = "ID";
        }
        private void dgChefRoti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            resetChefDetailRoti(false);
            bChefOptionInsert.IsChecked = false;
            DataRowView selectedChefRoti = dgChefRoti.SelectedItem as DataRowView;
            OracleCommand cmd = new OracleCommand($"SELECT JENIS_ROTI FROM ROTI WHERE KODE = '{selectedChefRoti.Row.ItemArray[0].ToString()}'", App.conn);
            cbChefJenisRoti.SelectedValue = cmd.ExecuteScalar().ToString();
            tbChefNamaRoti.Text = selectedChefRoti.Row.ItemArray[1].ToString();
            tbChefDeskripsiRoti.Text = selectedChefRoti.Row.ItemArray[2].ToString();
            cmd = new OracleCommand($"SELECT FK_RESEP FROM ROTI WHERE KODE = '{selectedChefRoti.Row.ItemArray[0].ToString()}'", App.conn);
            chefResep = r.fillResep("B.MERK || ' - ' || JB.NAMA_JENIS AS NAMA, D.QTY AS STOK", $"WHERE D.ID_H_RESEP = '{cmd.ExecuteScalar().ToString()}'", new DataTable());
            dgChefResep.ItemsSource = null;
            dgChefResep.ItemsSource = chefResep.DefaultView;
            bChefTambahStokRoti.IsEnabled = true;
            koderoti = selectedChefRoti.Row.ItemArray[0].ToString();
        }
        private void bChefTambahStokRoti_Click(object sender, RoutedEventArgs e)
        {

            OracleTransaction trans;
            trans = App.conn.BeginTransaction();
            try
            {
                DataRowView selectedChefRoti = dgChefRoti.SelectedItem as DataRowView;
                OracleCommand idHresep = new OracleCommand($"SELECT FK_RESEP FROM ROTI WHERE KODE = '{koderoti}'", App.conn);

                for (int i = 0; i < chefResep.Rows.Count; i++)
                {
                    OracleCommand stokbahan = new OracleCommand($"SELECT QTY_STOK FROM BAHAN WHERE ID = '{idHresep.ExecuteScalar().ToString()}'",App.conn);
                    int stok = Convert.ToInt32(stokbahan.ExecuteScalar().ToString());
                    int qtyresep = Convert.ToInt32(chefResep.Rows[i][1].ToString());
                    if (stok < qtyresep)
                    {
                        throw new InvalidOperationException("Stok bahan tidak cukup");
                    }
                    else
                    {
                        string namabahan = chefResep.Rows[i][0].ToString();
                        OracleCommand idbahan = new OracleCommand($"SELECT ID FROM BAHAN WHERE MERK = '{namabahan.Substring(0, namabahan.IndexOf('-') - 1)}'", App.conn);
                        OracleCommand cmd = new OracleCommand($"UPDATE BAHAN SET QTY_STOK = QTY_STOK-1 WHERE ID = '{idbahan.ExecuteScalar().ToString()}'", App.conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                OracleCommand cmd2 = new OracleCommand($"UPDATE ROTI SET STOK = STOK+1 WHERE KODE = '{koderoti}'", App.conn);
                cmd2.ExecuteNonQuery();
                trans.Commit();
                MessageBox.Show("Stok Roti berhasil ditambahkan");
                chefRoti = new DataTable();
                var index = dgChefRoti.SelectedValue;
                initChef();
                dgChefRoti.SelectedValue = index;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                trans.Rollback();
            }
        }

        private void bChefTambahBahan_Click(object sender, RoutedEventArgs e)
        {
            if(cbChefBahan.SelectedIndex == -1 || tbChefBahanQuantity.Text == "0" || tbChefBahanQuantity.Text == "")
            {
                MessageBox.Show("Input tidak lengkap");
            }
            else
            {
                int adaresep = -1;
                for (int i = 0; i < chefResep.Rows.Count; i++)
                {
                    if (chefResep.Rows[i][0].ToString().Equals(cbChefBahan.Text))
                    {
                        adaresep = i;
                    }
                }
                if(adaresep == -1)
                {
                    chefResep = b.fillDataTable($"B.MERK || ' - ' || JB.NAMA_JENIS AS NAMA, {tbChefBahanQuantity.Text} AS STOK", $"WHERE B.ID = '{cbChefBahan.SelectedValue}'", chefResep);
                }
                else {
                    chefResep.Rows[adaresep][1] = Convert.ToInt32(chefResep.Rows[adaresep][1]) + Convert.ToInt32(tbChefBahanQuantity.Text);
                }
                MessageBox.Show(adaresep + " " + cbChefBahan.Text);
                dgChefResep.ItemsSource = null;
                dgChefResep.ItemsSource = chefResep.DefaultView;
                cbChefBahan.SelectedIndex = -1;
                tbChefBahanQuantity.Text = "0";
                

            }
        }

        private void bChefTambahRoti_Click(object sender, RoutedEventArgs e)
        {
            if (tbChefDeskripsiRoti.Text == "" || tbChefNamaRoti.Text == "" || cbChefJenisRoti.SelectedIndex == -1 || chefResep.Rows.Count == 0)
            {
                MessageBox.Show("Input tidak lengkap");
            }
            else
            {
                OracleTransaction trans;
                trans = App.conn.BeginTransaction();
                try
                {
                    DataRowView selectedChefRoti = dgChefRoti.SelectedItem as DataRowView;
                    OracleCommand cmd = new OracleCommand($"INSERT INTO H_RESEP VALUES('{idhresep}', 'RESEP {tbChefNamaRoti.Text}')", App.conn);
                    cmd.ExecuteNonQuery();
                    int harga = 0;
                    for (int i = 0; i < chefResep.Rows.Count; i++)
                    {
                        string namabahan = chefResep.Rows[i][0].ToString();
                        OracleCommand idbahan = new OracleCommand($"SELECT ID FROM BAHAN WHERE MERK = '{namabahan.Substring(0, namabahan.IndexOf('-') - 1)}'", App.conn);
                        OracleCommand hargabahan = new OracleCommand($"SELECT HARGA FROM BAHAN WHERE MERK = '{namabahan.Substring(0, namabahan.IndexOf('-') - 1)}'", App.conn);
                        harga += Convert.ToInt32(hargabahan.ExecuteScalar().ToString());
                        cmd = new OracleCommand($"INSERT INTO D_RESEP VALUES('{idhresep}', '{idbahan.ExecuteScalar().ToString()}',{chefResep.Rows[i][1].ToString()})", App.conn);
                        cmd.ExecuteNonQuery();
                    }
                    harga += (harga * 20) / 100;
                    OracleCommand idRoti = new OracleCommand("SELECT COUNT(*)+1 FROM ROTI", App.conn);
                    string namaroti = tbChefNamaRoti.Text;
                    string kode = "";
                    if(namaroti.IndexOf(' ') >= 0)
                    {
                        kode = namaroti.Substring(0, 2)+namaroti.Substring(namaroti.IndexOf(' ')+1,2);
                    }
                    else
                    {
                        kode = namaroti.Substring(0, 4);
                    }
                    OracleCommand nourut = new OracleCommand($"SELECT LPAD(COUNT(*)+1,5,'0') FROM ROTI WHERE KODE LIKE '{kode}%'", App.conn);

                    OracleCommand cmd2 = new OracleCommand($"INSERT INTO ROTI VALUES('{idRoti.ExecuteScalar().ToString()}', '{kode+ nourut.ExecuteScalar().ToString()}', '{tbChefNamaRoti.Text}', '{tbChefDeskripsiRoti.Text}', {harga}, 0, 1, '{cbChefJenisRoti.SelectedValue}', '{idhresep}', '{tbChefNamaRoti.Text}.jpg')", App.conn);
                    cmd2.ExecuteNonQuery();
                    trans.Commit();
                    MessageBox.Show("Roti berhasil ditambahkan");
                    resetChefDetailRoti(false);
                    initChef();
                    bChefOptionInsert.IsChecked = false;

                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.Rollback();
                }
            }
        }
        private void resetChefDetailRoti(bool aktif) {
            tbChefNamaRoti.IsEnabled = aktif;
            tbChefDeskripsiRoti.IsEnabled = aktif;
            tbChefBahanQuantity.IsEnabled = aktif;
            cbChefBahan.IsEnabled = aktif;
            cbChefJenisRoti.IsEnabled = aktif;
            bChefTambahBahan.IsEnabled = aktif;
            bChefTambahStokRoti.IsEnabled = !aktif;
            bChefTambahRoti.IsEnabled = aktif;
            lbChefHint.Opacity = aktif ? 1 : 0;
            chefResep = new DataTable();
            dgChefResep.ItemsSource = null;
            dgChefResep.ItemsSource = chefResep.DefaultView;
            tbChefNamaRoti.Text = "-";
            tbChefDeskripsiRoti.Text = "-";
            cbChefJenisRoti.SelectedIndex = -1;
            cbChefBahan.SelectedIndex = -1;
            tbChefBahanQuantity.Text = "0";

        }
        private void bChefOptionInsert_Click(object sender, RoutedEventArgs e)
        {
            if (bChefOptionInsert.IsChecked.Value)
            {
                resetChefDetailRoti(true);
                OracleCommand idHresep = new OracleCommand("SELECT COUNT(*)+1 FROM H_RESEP", App.conn);
                idhresep = Convert.ToInt32(idHresep.ExecuteScalar().ToString());
                chefResep = new DataTable();
            }
            else
            {
                resetChefDetailRoti(false);
            }
        }

        private void dgChefResep_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (bChefOptionInsert.IsChecked.Value)
            {
                DataRowView selectedResep = dgChefResep.SelectedItem as DataRowView;
                for (int i = 0; i < chefResep.Rows.Count; i++)
                {
                    if (chefResep.Rows[i][0].ToString().Equals(selectedResep.Row.ItemArray[0].ToString()))
                    {
                        chefResep.Rows.RemoveAt(i);
                    }
                }
            }
        }
        //[END] JEFF

        // DAFTAR PESANAN
        private void loadDaftarPesanan()
        {
            OracleCommand cmd = new OracleCommand("SELECT H.NOMOR_NOTA, INITCAP(TO_CHAR(H.TANGGAL_TRANS, 'DD MONTH YYYY')), H.TOTAL, H.FK_KARYAWAN, P.NAMA, H.METODE_PEMBAYARAN, " +
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
                string id_karyawan = "-";
                if (reader.GetValue(3).ToString() != "")
                {
                    OracleCommand cmd2 = new OracleCommand("SELECT NAMA FROM KARYAWAN WHERE ID = '" + reader.GetValue(3).ToString() + "'", App.conn);
                    id_karyawan = cmd2.ExecuteScalar().ToString();
                }
                htranses.Add(new HTrans()
                {
                    nomor_nota = reader.GetValue(0).ToString(),
                    tanggal_trans = reader.GetValue(1).ToString(),
                    total = Convert.ToInt32(reader.GetValue(2).ToString()),
                    id_karyawan = id_karyawan,
                    id_pelanggan = reader.GetValue(4).ToString(),
                    metode_pembayaran = reader.GetValue(5).ToString(),
                    status = reader.GetValue(6).ToString()
                });
            }
            reader.Close();

            dtGridPesanan.ItemsSource = htranses;

            cbFilter.Items.Add("Nomor Nota");
            cbFilter.Items.Add("Tanggal");
            cbFilter.Items.Add("Total");
            cbFilter.Items.Add("Pelanggan");
            cbFilter.Items.Add("Pembayaran");
            cbFilter.Items.Add("Status");
            cbFilter.SelectedIndex = 0;
        }
        
        private void TbKeywordDafarPesanan_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPesanan();
        }

        private void CbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPesanan();
        }

        private void FilterPesanan()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(dtGridPesanan.ItemsSource);
            string filter = tbKeywordDafarPesanan.Text;
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    HTrans p = o as HTrans;
                    if (cbFilter.SelectedItem.Equals("Nomor Nota"))
                        return (p.nomor_nota.ToUpper().Contains(filter.ToUpper()));
                    else if (cbFilter.SelectedItem.Equals("Tanggal"))
                        return (p.tanggal_trans.ToUpper().Contains(filter.ToUpper()));
                    else if (cbFilter.SelectedItem.Equals("Total"))
                        return (p.total.ToString().Contains(filter.ToUpper()));
                    else if (cbFilter.SelectedItem.Equals("Pelanggan"))
                        return (p.id_pelanggan.ToUpper().Contains(filter.ToUpper()));
                    else if (cbFilter.SelectedItem.Equals("Pembayaran"))
                        return (p.metode_pembayaran.ToUpper().Contains(filter.ToUpper()));
                    else if (cbFilter.SelectedItem.Equals("Status"))
                        return (p.status.ToUpper().Contains(filter.ToUpper()));
                    return (p.nomor_nota.ToUpper().Contains(filter.ToUpper()));
                };
            }
        }

        private void BtnReportPesanan_Click(object sender, RoutedEventArgs e)
        {
            WindowTransitionReportPesanan wtrp = new WindowTransitionReportPesanan();
            toggleOverlay();
            wtrp.ShowDialog();

            toggleOverlay();
        }
        private void BtnDetailHTrans_Click(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowPesanan wp = new WindowPesanan(ID.ToString(), loggedUserID);
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            //toggleOverlay();
            wp.ShowDialog();

            loadDaftarPesanan();
            toggleOverlay();
        }

        private void toggleOverlay()
        {
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);

            if (overlay.Visibility == Visibility.Hidden)
            {
                overlay.Visibility = Visibility.Visible;
            }
            else if (overlay.Visibility == Visibility.Visible)
            {
                overlay.Visibility = Visibility.Hidden;
            }
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

        private void loadDataResep()
        {
            reseps = new List<Resep>();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = App.conn;
            cmd.CommandText = "select * from h_resep";
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cmd = new OracleCommand();
                cmd.CommandText = $"select count(*) from d_resep where id_h_resep='{reader.GetValue(0).ToString()}'";
                cmd.Connection = App.conn;
                reseps.Add(new Resep()
                {
                    id_resep = reader.GetValue(0).ToString(),
                    nama_resep = reader.GetValue(1).ToString(),
                    jumlah_bahan = Convert.ToInt32(cmd.ExecuteScalar().ToString()),
                });
            }
            reader.Close();

            dgResep.ItemsSource = reseps;
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
            //loadCbSupplier("insert");
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
            if (dgBahan.Columns.Count > 0)
            {
                dgBahan.Columns[0].Width = DataGridLength.SizeToCells;
                dgBahan.Columns[1].Width = DataGridLength.SizeToCells;
                dgBahan.Columns[2].Width = DataGridLength.Auto;
                dgBahan.Columns[3].Width = DataGridLength.Auto;
                dgBahan.Columns[4].Width = DataGridLength.SizeToCells;
                dgBahan.Columns[5].Width = DataGridLength.SizeToCells;
            }
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
            //loadCbSupplier("update");
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

        //private void loadCbSupplier(string state)
        //{
        //    if (state == "insert")
        //    {
        //        cbSupplier.Items.Clear();
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.CommandText = "select * from supplier";
        //        cmd.Connection = App.conn;
        //        OracleDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            cbSupplier.Items.Add(new ComboBoxItem()
        //            {
        //                Name = "ID" + reader.GetString(0),
        //                Content = reader.GetString(2)
        //            });
        //        }
        //        reader.Close();
        //        cbSupplier.SelectedValuePath = "Name";
        //        cbSupplier.SelectedIndex = 0;
        //    }
        //    else
        //    {
        //        cbSupplierUpdate.Items.Clear();
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.CommandText = "select * from supplier";
        //        cmd.Connection = App.conn;
        //        OracleDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            cbSupplierUpdate.Items.Add(new ComboBoxItem()
        //            {
        //                Name = "ID" + reader.GetString(0),
        //                Content = reader.GetString(2)
        //            });
        //        }
        //        reader.Close();
        //        cbSupplierUpdate.SelectedValuePath = "Name";
        //        cbSupplierUpdate.SelectedIndex = 0;
        //    }
            
        //}

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
                            cmd.Parameters.Add(":8", "0"); //pic_loc
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
                //int idxSupplier = 0;
                //foreach (ComboBoxItem cbItem in cbSupplierUpdate.Items)
                //{
                //    if (cbItem.Name == "ID" + reader.GetString(7))
                //    {
                //        break;
                //    }
                //    idxSupplier++;
                //}
                //cbSupplierUpdate.SelectedIndex = idxSupplier;
                cbJenisBahanUpdate.SelectedIndex = idxJenisBahan;

                bahanImgSourceDir = reader.GetString(7);
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
            karyawanImgSourceDir = "";
            tbNamaKaryawan.Text = "";
            tbUsernameKaryawan.Text = "";
            tbPasswordKaryawan.Password = "";
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


                            OracleCommand cmd = new OracleCommand();
                            cmd.CommandText = $"update bahan set merk=:1,qty_stok=:2,harga=:3,satuan=:4,jenis_bahan=:5 where ID={selectedIdBahan}";
                            cmd.Connection = App.conn;
                            cmd.Parameters.Add(":1", merk);
                            cmd.Parameters.Add(":2", qty);
                            cmd.Parameters.Add(":3", harga);
                            cmd.Parameters.Add(":4", satuan);
                            cmd.Parameters.Add(":5", jenisBahan);
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
        

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdBahan < 0)
            {
                MessageBox.Show("Please Pick Ingredient First");
                return;
            }

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"update bahan set status = 0 where ID='{selectedIdBahan}'";
            cmd.Connection = App.conn;
            cmd.ExecuteNonQuery();

            loadDataBahan();
            selectedIdBahan = -1;
            MessageHandler.messageSuccess("Delete Ingredients");
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
                btnActive.IsEnabled = false;
                btnSuspend.IsEnabled = false;
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
                btnActive.IsEnabled = false;
                btnSuspend.IsEnabled = false;
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
            loadImage(imgInsertkaryawan, "\\Resources\\ImagePlaceholder.png");
        }

        string karyawanImgSourceDir = "";
        private void btnOpenImgKaryawan_Click(object sender, RoutedEventArgs e)
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
                karyawanImgSourceDir = selectedFileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                imgInsertkaryawan.Source = bitmap;
            }
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
            string password = tbPasswordKaryawan.Password;
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

            if (nama == "" || username == "" || password=="" || email=="" || jenisKelamin=="" || alamat=="" || noTelp=="" || karyawanImgSourceDir == "")
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

            cmd = new OracleCommand();
            cmd.CommandText = "select picture_location from karyawan group by id, picture_location having id = (select max(to_number(id)) from karyawan)";
            cmd.Connection = App.conn;
            saveImage(karyawanImgSourceDir, "\\Resources\\Karyawan\\", cmd.ExecuteScalar().ToString());

            MessageHandler.messageSuccess("Insert Karyawan");

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
                MessageBox.Show("Please Pick Bread First");
                return;
            }

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"update roti set status = 0 where ID='{selectedIdRoti}'";
            cmd.Connection = App.conn;
            cmd.ExecuteNonQuery();

            loadDataRoti();
            selectedIdRoti = -1;
            MessageHandler.messageSuccess("Delete Bread");
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

        private void btnOpenWindowDetailRoti(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowResep wr = new WindowResep(ID.ToString());
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wr.ShowDialog();

            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }

        private void dgKaryawan_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Detail")
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                DataTemplate buttonTemplate = new DataTemplate();
                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonTemplate.VisualTree = buttonFactory;
                //add handler or you can add binding to command if you want to handle click
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnOpenWindowDetailKaryawan));
                buttonFactory.SetBinding(Button.CommandParameterProperty, new Binding("Detail"));
                buttonFactory.SetValue(Button.ContentProperty, "Detail");
                buttonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Colors.CadetBlue));
                buttonFactory.SetValue(Button.WidthProperty, 80.0);
                buttonColumn.CellTemplate = buttonTemplate;
                e.Column = buttonColumn;
            }
        }

        private void btnOpenWindowDetailKaryawan(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;

            WindowKaryawan wk = new WindowKaryawan(ID.ToString());
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wk.ShowDialog();

            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }

        private void btnBuatVoucher_Click(object sender, RoutedEventArgs e)
        {
            WindowInsertVoucher wiv = new WindowInsertVoucher();
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wiv.ShowDialog();

            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }

        private void btnBagiVoucher_Click(object sender, RoutedEventArgs e)
        {
            WindowBagiVoucher wib = new WindowBagiVoucher();
            overlay.Visibility = Visibility.Visible;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
            wib.ShowDialog();

            overlay.Visibility = Visibility.Hidden;
            overlay.Width = windowPesanan.ActualWidth;
            overlay.Height = windowPesanan.ActualHeight;
            overlay.Margin = new Thickness(0, 0, 0, 0);
        }
        
        //Number only input

        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        //Paste ke input hanya dibolehin angka(di xaml tambahin DataObject.Pasting = "TextBoxPasting")
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        //[END] Number only input
    }
}
