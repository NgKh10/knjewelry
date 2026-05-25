using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        public int id_loai_sp { get; set; }
        public string ten_loai { get; set; } = "";
        public int? id_loai_cha { get; set; }
        public int thu_tu { get; set; }
        public string? mo_ta { get; set; }
        public LoaiSanPham LoaiCha { get; set; }
        public ICollection<LoaiSanPham> DanhMucCon { get; set; } = new List<LoaiSanPham>();
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}