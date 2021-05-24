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
    /// Interaction logic for WindowCetakStruk.xaml
    /// </summary>
    public partial class WindowCetakStruk : Window
    {
        string nonota;
        public WindowCetakStruk(string nonota, Window w)
        {
            InitializeComponent();
            this.nonota = nonota;
            this.Owner = w;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportTransStruk cr = new ReportTransStruk();
            cr.SetDatabaseLogon(App.username, App.password, App.source,"");
            cr.SetParameterValue("nonota", nonota);
            crvCetakStruk.ViewerCore.ReportSource = cr;
        }
    }
}
