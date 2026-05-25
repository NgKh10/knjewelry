using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("ChatLieu")]
    public class ChatLieu
    {
        [Key]
        public int id_chat_lieu { get; set; }
        public string ten_chat_lieu { get; set; } = "";
        public string? do_tinh_khiet { get; set; }
        public string? mo_ta { get; set; }
    }
}