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
    /// Interaction logic for LoginRegis.xaml
    /// </summary>
    public partial class LoginRegis : Window
    {
        public LoginRegis()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }

        private void Regis_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // ubah form jadi regis mode

            MainWindow mw = new MainWindow();
            this.Hide();
            mw.ShowDialog();
            this.ShowDialog();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // pengecekan username password

            MainWindow mw = new MainWindow();
            this.Hide();
            mw.ShowDialog();
            this.ShowDialog();
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            if (MessageHandler.confirmYesNo("Are you sure you want to close the program?"))
            {
                this.Close();
            }
        }
    }
}
