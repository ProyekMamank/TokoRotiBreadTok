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
using System.Data;
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

                    MainWindow mw = new MainWindow("0");
                    this.Hide();
                    mw.ShowDialog();
                    this.ShowDialog();
                }
                else
                {
                    if (rbKaryawan.IsChecked.Value)
                    {
                        OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM KARYAWAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                        cmd.Parameters.Add(":1", tbUsername.Text);
                        cmd.Parameters.Add(":2", tbPassword.Password);
                        int ada = Convert.ToInt32(cmd.ExecuteScalar());
                        if (ada != 0)
                        {
                            OracleCommand cmdID = new OracleCommand("SELECT ID FROM KARYAWAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                            cmdID.Parameters.Add(":1", tbUsername.Text);
                            cmdID.Parameters.Add(":2", tbPassword.Password);
                            string idUser = cmdID.ExecuteScalar().ToString();

                            tbUsername.Text = "";
                            tbPassword.Password = "";

                            MainWindow mw = new MainWindow(idUser);
                            this.Hide();
                            mw.ShowDialog();
                            this.ShowDialog();
                        }
                        else
                        {
                            MessageHandler.wrongUsernamePassword();
                        }
                    }
                    else
                    {
                        OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM PELANGGAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                        cmd.Parameters.Add(":1", tbUsername.Text);
                        cmd.Parameters.Add(":2", tbPassword.Password);
                        int ada = Convert.ToInt32(cmd.ExecuteScalar());
                        if (ada != 0)
                        {
                            OracleCommand cmdID = new OracleCommand("SELECT ID FROM PELANGGAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                            cmdID.Parameters.Add(":1", tbUsername.Text);
                            cmdID.Parameters.Add(":2", tbPassword.Password);
                            string idUser = cmdID.ExecuteScalar().ToString();

                            tbUsername.Text = "";
                            tbPassword.Password = "";

                            WindowPelanggan wp = new WindowPelanggan(idUser);
                            this.Hide();
                            wp.ShowDialog();
                            this.ShowDialog();
                        }
                        else
                        {
                            MessageHandler.wrongUsernamePassword();
                        }
                    }
                }
            }
            else
            {
                MessageHandler.requireField();
            }
        }
        
        private string getIdPelanggan()
        {
            OracleCommand cmd = new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                Connection = App.conn,
                CommandText = "AUTOGEN_ID_PELANGGAN"
            };

            cmd.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.ReturnValue,
                ParameterName = "id_pelanggan",
                OracleDbType = OracleDbType.Varchar2,
                Size = 15
            });

            cmd.ExecuteNonQuery();
            return cmd.Parameters["id_pelanggan"].Value.ToString();
        }

        private void Regis_MouseDown2(object sender, RoutedEventArgs e)
        {
            // Regis Button from Regis Form
            if (rTbUsername.Text != "" && rTbPassword.Password != "" && rTbConfirmPassword.Password != "" &&
                rTbNama.Text != "" && (rRbLaki.IsChecked == true || rRbPerempuan.IsChecked == true) && 
                rTbAlamat.Text != "" && rTbEmail.Text != "" && rTbNoTelp.Text != "" && rTglLahir.SelectedDate != null)
            {
                if (rTbPassword.Password == rTbConfirmPassword.Password)
                {
                    if (rTbNoTelp.Text.All(char.IsDigit))
                    {
                        if (!rTbUsername.Text.Contains(" "))
                        {
                            if (!rTbEmail.Text.Contains(" "))
                            {
                                OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM PELANGGAN WHERE USERNAME = :1 AND PASSWORD = :2", App.conn);
                                cmd.Parameters.Add(":1", rTbUsername.Text);
                                cmd.Parameters.Add(":2", rTbPassword.Password);
                                int ada = Convert.ToInt32(cmd.ExecuteScalar());
                                if (ada == 0)
                                {
                                    cmd = new OracleCommand("INSERT INTO PELANGGAN VALUES(:1, :2, :3, :4, :5, :6, :7, :8, :9, :10)", App.conn);
                                    cmd.Parameters.Add(":1", getIdPelanggan());
                                    cmd.Parameters.Add(":2", rTbUsername.Text);
                                    cmd.Parameters.Add(":3", rTbPassword.Password);
                                    cmd.Parameters.Add(":4", rTbNama.Text);
                                    cmd.Parameters.Add(":5", (rRbLaki.IsChecked == true) ? 'L' : 'P');
                                    cmd.Parameters.Add(":6", rTbAlamat.Text);
                                    cmd.Parameters.Add(":7", rTbEmail.Text);
                                    cmd.Parameters.Add(":8", rTbNoTelp.Text);
                                    cmd.Parameters.Add(":9", rTglLahir.SelectedDate);
                                    cmd.Parameters.Add(":10", 1);
                                    cmd.ExecuteNonQuery();

                                    MessageHandler.messageSuccess("Insert Account");
                                    clearRegisForm();
                                }
                                else
                                {
                                    MessageHandler.usernameExists();
                                }
                            }
                            else
                            {
                                MessageHandler.containSpaces("Email");
                            }
                        }
                        else
                        {
                            MessageHandler.containSpaces("Username");
                        }
                    }
                    else
                    {
                        MessageHandler.isNotNumber("Nomor Telpon");
                    }
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

        private void clearRegisForm()
        {
            rTbUsername.Text = "";
            rTbPassword.Password = "";
            rTbConfirmPassword.Password = "";
            rTbNama.Text = "";
            rRbLaki.IsChecked = false;
            rRbPerempuan.IsChecked = false;
            rTbAlamat.Text = "";
            rTbEmail.Text = "";
            rTbNoTelp.Text = "";
            rTglLahir.SelectedDate = null;
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
