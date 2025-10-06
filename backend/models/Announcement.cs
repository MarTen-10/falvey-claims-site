using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
    public class Announcement
    {
        [Column("announcement_id")]
        [Key]
        public int? announcement_id { get; set; }

        [Column("title")]
        [Required]
        public required string title { get; set; }

        [Column("body")]
        public required string body { get; set; }

        [Column("publish_at")]
        [Required]
        public required DateTime publish_at { get; set; }

        [Column("expire_at")]
        [Required]
        public required DateTime expire_at { get; set; }

        [Column("created_by")]
        [Required]
        public required int created_by { get; set; }

        [ForeignKey("created_by")]
        public User? User { get; set; }  // links to user_id

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
