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
using Oracle.DataAccess.Client;
using System.Globalization;
using System.IO;
using Microsoft.Win32;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for WindowBahan.xaml
    /// </summary>
    public partial class WindowBahan : Window
    {
        string id;
        public WindowBahan(string id)
        {
            InitializeComponent();
            this.id = id;

            loadDetailBahan();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void loadDetailBahan()
        {
            OracleCommand cmd = new OracleCommand("SELECT B.KODE, B.MERK, B.QTY_STOK, B.HARGA, B.SATUAN, JB.NAMA_JENIS, S.NAMA, B.PICTURE_LOCATION " +
                                                "FROM BAHAN B, JENIS_BAHAN JB, SUPPLIER S " +
                                                "WHERE B.JENIS_BAHAN = JB.ID AND " +
                                                "B.FK_SUPPLIER = S.ID AND B.ID = '" + id + "' ", App.conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lblKode.Content = reader.GetValue(0).ToString();
                lblMerk.Content = reader.GetValue(1).ToString();
                lblQtyStock.Content = reader.GetValue(2).ToString();
                lblHarga.Content = Convert.ToInt32(reader.GetValue(3).ToString()).ToString("C", CultureInfo.CreateSpecificCulture("id-ID"));
                lblSatuan.Content = reader.GetValue(4).ToString();
                lblJenis.Content = reader.GetValue(5).ToString();
                lblSupplier.Content = reader.GetValue(6).ToString();
                loadImage(imgBahan, "\\Resources\\Bahan\\" + reader.GetValue(7).ToString());
            }
            reader.Close();
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
    }
}
