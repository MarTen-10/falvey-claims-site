using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Xml.Linq;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("policies")]
    public class Policy
    {
        [Column("policy_id")]
        [Key]
        public int? policy_id { get; set; }

        [Column("account_number")]
        [Required]
        [MaxLength(32)]
        public required string account_number { get; set; }

        [Column("customer_id")]
        [Required]
        public required int customer_id { get; set; }

        [ForeignKey("customer_id")]
        public Customer? customer { get; set; }  // links to customer_id
        
        [Column("manager_id")]
        public int? manager_id { get; set; }

        [ForeignKey("manager_id")]
        public Employee? manager { get; set; }  // links to manager_id
        
        [Column("policy_type")]
        [Required]
        [MaxLength(20)]
        public required string policy_type { get; set; } = "Other";

        [Column("status")]
        [Required]
        [MaxLength(20)]
        public required string status { get; set; } = "Active";

        [Column("start_date")]
        [Required]
        public required DateTime start_date { get; set; }

        [Column("end_date")]
        [Required]
        public required DateTime end_date { get; set; }

        [Column("exposure_amount", TypeName = "decimal(13,2)")]
        [Required]
        [Range(999.99D, 99999999999.99D, ErrorMessage = "Please enter a valid exposure amount (digits and period only).")]
        public required decimal exposure_amount { get; set; }

        [Column("loc_addr1")]
        [Required]
        [MaxLength(120)]
        public required string loc_addr1 { get; set; }

        [Column("loc_addr2")]
        [MaxLength(120)]
        public string? loc_addr2 { get; set; }

        [Column("loc_city")]
        [Required]
        [MaxLength(80)]
        public required string loc_city { get; set; }

        [Column("loc_state")]
        [Required]
        [MaxLength(10)]
        public required string loc_state { get; set; }

        [Column("loc_zip")]
        [StringLength(9, MinimumLength = 5, ErrorMessage = "Please enter a valid zip code between five and nine digits.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid zip code (digits only).")]
        public required string loc_zip { get; set; }

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.Now;

    }
}
