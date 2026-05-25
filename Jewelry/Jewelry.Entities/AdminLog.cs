using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("AdminLog")]
    public class AdminLog
    {
        [Key]
        public int id_log { get; set; }

        [ForeignKey("NguoiDung")]
        public int id_admin { get; set; }
        public string ten_bang { get; set; } = "";
        public string hanh_dong { get; set; } = "";
        public int? id_ban_ghi { get; set; }
        public string? noi_dung { get; set; }
        public DateTime thoi_gian { get; set; }

        public NguoiDung? Admin { get; set; }
    }
}