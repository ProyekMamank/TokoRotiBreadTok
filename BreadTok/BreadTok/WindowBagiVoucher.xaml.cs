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
    /// Interaction logic for WindowBagiVoucher.xaml
    /// </summary>
    public partial class WindowBagiVoucher : Window
    {
        class customer
        {
            public string id_customer { get; set; }
            public string nama_customer { get; set; }
        }
        List<customer> listCustomer;
        List<customer> listAddedCustomer;
        public WindowBagiVoucher()
        {
            InitializeComponent();
            loadCbVoucher();
            loadDataListCustomer();
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

        private void loadCbVoucher()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"select * from voucher";
            cmd.Connection = App.conn;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbVoucher.Items.Add(new ComboBoxItem
                {
                    Content = reader.GetValue(1).ToString(),
                    Name = "ID"+ reader.GetValue(0).ToString(),
                });
            }
            reader.Close();
            cbVoucher.SelectedValuePath = "Name";
            cbVoucher.SelectedIndex = 0;
        }

        private void loadDataListCustomer()
        {
            dgListCustomer.ItemsSource = null;
            dgListAddedCustomer.ItemsSource = null;
            listCustomer = new List<customer>();
            listAddedCustomer = new List<customer>();

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = "select * from pelanggan";
            cmd.Connection = App.conn;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                listCustomer.Add(new customer
                {
                    id_customer = reader.GetValue(0).ToString(),
                    nama_customer = reader.GetValue(2).ToString()
                });
            }
            reader.Close();
            dgListCustomer.ItemsSource = listCustomer;
            dgListAddedCustomer.ItemsSource = listAddedCustomer;
        }

        private void dgListCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listCustomer.Remove(dgListCustomer.SelectedItem as customer);
            listAddedCustomer.Add(dgListCustomer.SelectedItem as customer);
            reloadDatagrid();
        }

        private void dgListAddedCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            listAddedCustomer.Remove(dgListAddedCustomer.SelectedItem as customer);
            listCustomer.Add(dgListAddedCustomer.SelectedItem as customer);
            reloadDatagrid();
        }

        private void reloadDatagrid()
        {
            dgListCustomer.ItemsSource = null;
            dgListAddedCustomer.ItemsSource = null;

            dgListCustomer.ItemsSource = listCustomer;
            dgListAddedCustomer.ItemsSource = listAddedCustomer;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            foreach(customer c in listAddedCustomer)
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "insert into user_voucher values(:1,:2,:3,:4)";
                cmd.Connection = App.conn;

                cmd.Parameters.Add(":1", "0");
                cmd.Parameters.Add(":2", c.id_customer);
                cmd.Parameters.Add(":3", cbVoucher.SelectedValue.ToString().Substring(2));
                cmd.Parameters.Add(":4", 1);

                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Berhasil Membagikan Voucher");
            resetBagiVoucher();
        }

        private void resetBagiVoucher()
        {
            cbVoucher.SelectedIndex = 0;
            loadDataListCustomer();
        }
    }
}
