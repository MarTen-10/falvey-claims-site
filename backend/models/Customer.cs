using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int customer_id { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public required string name { get; set; }

        [MaxLength(120)]
        [Column("email")]
        public string? email { get; set; }

        [MaxLength(25)]
        [Column("phone")]
        public string? phone { get; set; }

        [MaxLength(120)]
        [Column("addr_line1")]
        public string? addr_line1 { get; set; }

        [MaxLength(120)]
        [Column("addr_line2")]
        public string? addr_line2 { get; set; }

        [MaxLength(80)]
        [Column("city")]
        public string? city { get; set; }

        [MaxLength(10)]
        [Column("state_code")]
        public string? state_code { get; set; }

        [MaxLength(12)]
        [Column("zip_code")]
        public string? zip_code { get; set; }

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.Now;

        public ICollection<Policy> policies { get; set; } = new List<Policy>();
    }
}
