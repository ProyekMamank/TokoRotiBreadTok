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
            LoginMode();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }


        private void BtnRegis_Click(object sender, RoutedEventArgs e)
        {
            // Regis Button from Login Form
            RegisMode();
        }
        
        private void BtnLogin2_Click(object sender, RoutedEventArgs e)
        {
            // Login Button from Regis Form
            LoginMode();
        }

        private void RegisMode()
        {
            GridLogin.Visibility = Visibility.Hidden;
            GridRegis.Visibility = Visibility.Visible;
        }

        private void LoginMode()
        {
            GridLogin.Visibility = Visibility.Visible;
            GridRegis.Visibility = Visibility.Hidden;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Login Button from Login Form
            if (tbUsername.Text != "" && tbPassword.Password != "")
            {
                if (tbUsername.Text == "admin" && tbPassword.Password == "admin")
                {
                    tbUsername.Text = "";
                    tbPassword.Password = "";

                    MainWindow mw = new MainWindow();
                    this.Hide();
                    mw.ShowDialog();
                    this.ShowDialog();
                }
                else
                {
                    OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM PELANGGAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                    cmd.Parameters.Add(":1", tbUsername.Text);
                    cmd.Parameters.Add(":2", tbPassword.Password);
                    int ada = Convert.ToInt32(cmd.ExecuteScalar());
                    if (ada != 0)
                    {
                        tbUsername.Text = "";
                        tbPassword.Password = "";

                        MainWindow mw = new MainWindow();
                        this.Hide();
                        mw.ShowDialog();
                        this.ShowDialog();
                    }
                    else
                    {
                        MessageHandler.wrongUsernamePassword();
                    }
                }
            }
            else
            {
                MessageHandler.requireField();
            }
        }

        private void Regis_MouseDown2(object sender, RoutedEventArgs e)
        {
            // Regis Button from Regis Form
            if (rTbUsername.Text != "" && rTbPassword.Password != "" && rTbConfirmPassword.Password != "" &&
                rTbNama.Text != "" && (rRbLaki.IsChecked == true || rRbPerempuan.IsChecked == false) && 
                rTbAlamat.Text != "" && rTbEmail.Text != "" && rTbNoTelp.Text != "" && rTglLahir.SelectedDate == null)
            {
                if (rTbPassword.Password == rTbConfirmPassword.Password)
                {

                }
                else
                {
                    MessageHandler.PWDoesntMatchConf();
                }
            }
            else
            {
                MessageHandler.requireField();
            }
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
