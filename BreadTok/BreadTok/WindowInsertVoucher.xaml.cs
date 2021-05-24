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
    /// Interaction logic for WindowInsertVoucher.xaml
    /// </summary>
    public partial class WindowInsertVoucher : Window
    {
        public WindowInsertVoucher()
        {
            InitializeComponent();
            loadCbJenis();
        }

        private void loadCbJenis()
        {
            cbJenis.Items.Add("POTONGAN");
            cbJenis.Items.Add("DISKON");
            cbJenis.SelectedIndex = 0;
        }

        private void resetField()
        {
            tbNama.Text = "";
            tbHarga.Text = "";
            cbJenis.SelectedIndex = 0;
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

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(tbNama.Text == "" || tbHarga.Text == "")
            {
                MessageHandler.requireField();
                return;
            }

            if (!tbHarga.Text.All(Char.IsDigit))
            {
                MessageHandler.isNotNumber("Harga");
                return;
            }

            string nama = tbNama.Text;
            string jenis = cbJenis.SelectedItem.ToString();
            int harga = Convert.ToInt32(tbHarga.Text);

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "insert into voucher values(:1,:2,:3,:4)";
            cmd.Connection = App.conn;

            cmd.Parameters.Add(":1", "0");
            cmd.Parameters.Add(":2", nama);
            cmd.Parameters.Add(":3", jenis);
            cmd.Parameters.Add(":4", harga);

            cmd.ExecuteNonQuery();

            MessageHandler.messageSuccess("Insert Voucher");
            resetField();
        }
    }
}
