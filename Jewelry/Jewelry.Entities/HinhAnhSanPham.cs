using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("HinhAnhSanPham")]
    public class HinhAnhSanPham
    {
        [Key]
        public int id_hinh_anh { get; set; }

        [ForeignKey("SanPham")]
        public int id_san_pham { get; set; }
        public string duong_dan { get; set; } = "";
        public bool la_chinh { get; set; }
        public int thu_tu { get; set; }
        public SanPham? SanPham { get; set; }
    }
}