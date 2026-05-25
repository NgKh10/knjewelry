using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        public int id_san_pham { get; set; }
        public string ten_sp { get; set; }
        public decimal gia { get; set; }
        public decimal? gia_khuyen_mai { get; set; }

        [ForeignKey("LoaiSanPham")]  // ← Thêm dòng này
        public int id_loai_sp { get; set; }

        [ForeignKey("ChatLieu")]     // ← Thêm dòng này
        public int id_chat_lieu { get; set; }

        public decimal? trong_luong { get; set; }
        public string? mo_ta { get; set; }
        public byte trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public LoaiSanPham LoaiSanPham { get; set; }
        public List<BienThe> BienThes { get; set; }
        public List<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new();
        public ChatLieu? ChatLieu { get; set; }
    }
}