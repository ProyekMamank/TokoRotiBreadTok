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
    /// Interaction logic for WindowReportPesanan.xaml
    /// </summary>
    public partial class WindowReportPesanan : Window
    {
        public WindowReportPesanan(DatePicker dateAwal, DatePicker dateAkhir)
        {
            InitializeComponent();

            Console.WriteLine(dateAwal.SelectedDate.ToString());

            reportPenjualan rpt = new reportPenjualan();
            rpt.SetDatabaseLogon(App.username, App.password, App.source, "");

            rpt.SetParameterValue("tglAwal", dateAwal.SelectedDate.Value.ToString("dd/MM/yyyy"));
            rpt.SetParameterValue("tglAkhir", dateAkhir.SelectedDate.Value.ToString("dd/MM/yyyy"));
            cReportViewer.ViewerCore.ReportSource = rpt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cReportViewer.Owner = Window.GetWindow(this);
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
    }
}
