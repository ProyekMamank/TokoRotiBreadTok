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
    /// Interaction logic for WindowReportBahan.xaml
    /// </summary>
    public partial class WindowReportBahan : Window
    {
        public WindowReportBahan()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(dpTanggal.SelectedDate != null && dpTanggalAkhir.SelectedDate != null)
            {
                ReportBahan rpt = new ReportBahan();
                rpt.SetDatabaseLogon(App.username, App.password, App.source, "");
                rpt.SetParameterValue("tanggal_awal", dpTanggal.Text);
                rpt.SetParameterValue("tanggal_akhir", dpTanggalAkhir.Text);
                Report.ViewerCore.ReportSource = rpt;
            }
            else
            {
                MessageBox.Show("Pilih tanggal terlebih dahulu");
            }
            
        }

        
    }
}
