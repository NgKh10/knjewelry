using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("HinhAnhSanPham")]
    public class HinhAnhSanPham
    {
        [Key]
        public int id_hinh_anh { get; set; }

        [Required]
        public int id_san_pham { get; set; }

        [Required]
        [MaxLength(500)]
        public string duong_dan { get; set; }

        public bool la_chinh { get; set; } = false;

        public int thu_tu { get; set; } = 0;

        [ForeignKey("id_san_pham")]
        public virtual SanPham SanPham { get; set; }
    }
}