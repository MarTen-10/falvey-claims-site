using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("customers")]
    public class Customer
    {
        [Column("customer_id")]
        [Key]
        public int? customer_id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public required string name { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(120)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string email { get; set; }

        [Column("phone")]
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid phone number with ten digits.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid ten-digit phone number (numbers only).")]
        public required string phone { get; set; }

        [Column("addr_line1")]
        [Required]
        [StringLength(120, MinimumLength = 4, ErrorMessage = "Please enter a valid address.")]
        public required string addr_line1 { get; set; }

        [Column("addr_line2")]
        [MaxLength(120)]
        public string? addr_line2 { get; set; }

        [Column("city")]
        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Please enter a valid city name.")]
        public required string city { get; set; }

        [Column("state_code")]
        [Required]
        [MaxLength(10)]
        public required string state_code { get; set; }

        [Column("zip_code")]
        [StringLength(9, MinimumLength = 5, ErrorMessage = "Please enter a valid zip code between five and nine digits.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid zip code (digits only).")]
        public required string zip_code { get; set; }

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation for records
        // Commented out to prevent policy data from printing.
        //public ICollection<Policy> policies { get; set; } = new List<Policy>();
        //public ICollection<CustomerRecord> customer_records { get; set; }
    }
}
