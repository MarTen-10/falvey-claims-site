using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
    public class Session
    {

        [Column("session_id")]
        [Key]
        public Guid? session_id { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        [Required]
        public required int user_id { get; set; }

        [ForeignKey("user_id")]
        public User? session_user { get; set; }  // links to user_id

        [Column("session_hash")]
        public string? session_hash { get; set; } = string.Empty;

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("expires_at")]
        public DateTime? expires_at { get; set; }

        [Column("revoked_at")]
        public DateTime? revoked_at { get; set; }

        [Column("ip_address")]
        [Required]
        public string ip_address { get; set; } = string.Empty;

        [Column("user_agent")]
        public string? user_agent { get; set; }

    }
}
