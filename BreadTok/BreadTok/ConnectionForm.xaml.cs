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
    /// Interaction logic for ConnectionForm.xaml
    /// </summary>
    public partial class ConnectionForm : Window
    {
        public ConnectionForm()
        {
            InitializeComponent();
            dtS.Text = "orcl";
            username.Text = "proyekpcs";
            password.Text = "proyekpcs";
        }

        private void BtnSubmit(object sender, RoutedEventArgs e)
        {
            if (dtS.Text != "" && username.Text != "" && password.Text != "") {
                App.source = dtS.Text;
                App.username = username.Text;
                App.password = password.Text;
                
                if (App.openConn()) {
                    LoginRegis lg = new LoginRegis();
                    this.Hide();
                    lg.ShowDialog();
                    this.Close();
                }
            }
            else {
                MessageHandler.requireField();
                MessageBox.Show("Test");
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            if(MessageHandler.confirmYesNo("Are you sure you want to close the program?"))
            {
                Application.Current.Shutdown();
            }
        }
    }
}
