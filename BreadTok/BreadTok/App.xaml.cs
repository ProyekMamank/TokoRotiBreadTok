using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Oracle.DataAccess.Client;

namespace BreadTok
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static OracleConnection conn = new OracleConnection();
        public static string source = "";
        public static string username = "";
        public static string password = "";

        public static bool openConn()
        {
            try
            {
                conn.ConnectionString = "Data Source = " + source + "; User Id = " + username + "; Password = " + password + ";";
                conn.Open();
                return true;
            }
            catch (OracleException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool checkConn()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
