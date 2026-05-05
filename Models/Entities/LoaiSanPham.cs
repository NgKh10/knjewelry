using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        public int id_loai_sp { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ten_loai { get; set; }

        public int? id_loai_cha { get; set; }

        public int thu_tu { get; set; } = 0;

        [MaxLength(500)]
        public string? mo_ta { get; set; }

        [ForeignKey("id_loai_cha")]
        public virtual LoaiSanPham LoaiCha { get; set; }
        public virtual ICollection<LoaiSanPham> LoaiCon { get; set; }
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}