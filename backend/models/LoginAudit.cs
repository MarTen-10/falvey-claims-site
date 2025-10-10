using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace backend.models
{
    [Table("login_audit")]
    public class LoginAudit
    {
        [Key]
        public long audit_id { get; set; } // audit id for the login event
        public int? user_id { get; set; } // the user id in the login event
        public required string login_event { get; set; } // cotains the event type for the login
        public string? ip_address { get; set; } // the ip address in the login event 
        public string? user_agent { get; set; }
        public DateTime occurred_at { get; set; } // what time the login event occurred at

        // Navigation property for foreign key relationships
        [ForeignKey("user_id")]
        public User? UserId { get; set; }


    }


}