using Oracle.DataAccess.Client;
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

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for WindowPelanggan.xaml
    /// </summary>
    public partial class WindowPelanggan : Window
    {
        string loggedUserID;
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
    }
}
