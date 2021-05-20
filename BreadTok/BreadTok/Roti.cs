using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadTok
{
    class Roti
    {
        public string id_roti { get; set; }
        public string nama_roti { get; set; }
        public string kode_roti { get; set; }
        public string deskripsi_roti { get; set; }
        public int harga_roti { get; set; }
        public int stok_roti { get; set; }
        public string status_roti { get; set; }
        public string fk_jenisroti { get; set; }
        public string fk_resep { get; set; }
    }
}
