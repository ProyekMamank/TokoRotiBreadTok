using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadTok
{
    public class HTrans
    {
        public string nomor_nota { get; set; }
        public string tanggal_trans { get; set; }
        public int total { get; set; }
        public string id_karyawan { get; set; }
        public string id_pelanggan { get; set; }
        public string nama_karyawan { get; set; }
        public string nama_pelanggan { get; set; }
        public string metode_pembayaran { get; set; }
        public string id_voucher { get; set; }
        public string status { get; set; }
        public string countRoti { get; set; }
        public List<DTrans> dtrans { get; set; }
        public string action { get; set; }
    }
}
