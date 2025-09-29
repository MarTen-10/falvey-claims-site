using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Xml.Linq;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("policies")]
    public class Policy
    {
        [Key]
        [Column("policy_id")]
        public int policy_id { get; set; }

        [MaxLength(32)]
        [Column("account_number")]
        public required string account_number { get; set; }

        [Column("customer_id")]
        public required int customer_id { get; set; }

        [ForeignKey("customer_id")]
        public Customer? customer { get; set; }  // links to customer_id
        
        [Column("manager_id")]
        public int? manager_id { get; set; }

        [ForeignKey("manager_id")]
        public Employee? manager { get; set; }  // links to manager_id
        
        [Column("policy_type")]
        [MaxLength(20)]
        public string? policy_type { get; set; } = "Other";

        [Column("status")]
        [MaxLength(20)]
        public required string status { get; set; } = "Active";

        [Column("start_date")]
        public DateTime? start_date { get; set; }

        [Column("end_date")]
        public DateTime? end_date { get; set; }

        [Column("exposure_amount", TypeName = "decimal(13,2)")]
        public decimal? exposure_amount { get; set; }

        [Column("loc_addr1")]
        [MaxLength(120)]
        public string? loc_addr1 { get; set; }

        [Column("loc_addr2")]
        [MaxLength(120)]
        public string? loc_addr2 { get; set; }

        [Column("loc_city")]
        [MaxLength(80)]
        public string? loc_city { get; set; }

        [Column("loc_state")]
        [MaxLength(10)]
        public string? loc_state { get; set; }

        [Column("loc_zip")]
        [MaxLength(12)]
        public string? loc_zip { get; set; }

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.Now;

    }
}
