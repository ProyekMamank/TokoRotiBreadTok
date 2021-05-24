using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for WindowKaryawan.xaml
    /// </summary>
    public partial class WindowKaryawan : Window
    {
        string id;
        public WindowKaryawan(string id)
        {
            InitializeComponent();
            this.id = id;

            loadDetailKaryawan();
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

        private void loadDetailKaryawan()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select * from karyawan where id='{id}'";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lblKode.Content = reader.GetValue(1).ToString();
                lblNama.Content = reader.GetValue(4).ToString();
                lblUsername.Content = reader.GetValue(2).ToString();
                lblPassword.Content = reader.GetValue(3).ToString();
                lblEmail.Content = reader.GetValue(7).ToString();

                string jabatan = "";
                cmd = new OracleCommand();
                cmd.CommandText = $"select nama_jabatan from jabatan where id='{reader.GetValue(11).ToString()}'";
                cmd.Connection = App.conn;
                lblJabatan.Content = cmd.ExecuteScalar().ToString();

                string jk = "";
                if(reader.GetValue(5).ToString() == "L")
                {
                    jk = "Laki-Laki";
                }
                else
                {
                    jk = "Perempuan";
                }
                lblJK.Content = jk;

                lblTglLahir.Content = reader.GetValue(9).ToString();
                lblTelp.Content = reader.GetValue(8).ToString();
                lblAlamat.Content = reader.GetValue(6).ToString();
                loadImage(imgKaryawan, "\\Resources\\Karyawan\\" + reader.GetValue(12).ToString());
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
