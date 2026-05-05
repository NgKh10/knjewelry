using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("ChatLieu")]
    public class ChatLieu
    {
        [Key]
        public int id_chat_lieu { get; set; }

        [Required]
        [MaxLength(100)]
        public string ten_chat_lieu { get; set; }

        [MaxLength(50)]
        public string do_tinh_khiet { get; set; }

        [MaxLength(500)]
        public string mo_ta { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}