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
    /// Interaction logic for WindowTransitionReportPesanan.xaml
    /// </summary>
    public partial class WindowTransitionReportPesanan : Window
    {
        public WindowTransitionReportPesanan()
        {
            InitializeComponent();

            cbStatus.Items.Add("Semua");
            cbStatus.Items.Add("Belum Bayar");
            cbStatus.Items.Add("Request Sudah Bayar");
            cbStatus.Items.Add("Sudah Bayar");
            cbStatus.Items.Add("Cancelled");
            cbStatus.SelectedIndex = 0;
        }

        private void BtnSelectReport_Click(object sender, RoutedEventArgs e)
        {
            if (dtAwal.SelectedDate != null && dtAkhir.SelectedDate != null)
            {
                if (dtAwal.SelectedDate <= dtAkhir.SelectedDate && dtAkhir.SelectedDate <= DateTime.Now)
                {
                    int status = 0;
                    if (cbStatus.SelectedIndex == 0)
                        status = 4;
                    else if (cbStatus.SelectedIndex == 1)
                        status = 0;
                    else if (cbStatus.SelectedIndex == 2)
                        status = 1;
                    else if (cbStatus.SelectedIndex == 3)
                        status = 2;
                    else if (cbStatus.SelectedIndex == 1)
                        status = 3;

                    WindowReportPesanan wpr = new WindowReportPesanan(dtAwal, dtAkhir, status);
                    wpr.ShowDialog();

                    dtAwal.SelectedDate = null;
                    dtAkhir.SelectedDate = null;
                    cbStatus.SelectedIndex = 0;
                }
                else
                {
                    MessageHandler.dateInvalid();
                }
            }
            else
            {
                MessageHandler.requireField();
            }
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
