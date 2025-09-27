using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace FalveyInsuranceGroup.Backend.Models
{
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int customer_id { get; set; }

        //[Required]
        [MaxLength(100)]
        [Column("name")]
        public string? name { get; set; }// = null!;

        //[Required]
        [MaxLength(120)]
        [Column("email")]
        public string? email { get; set; }// = null!;

        //[Required]
        [MaxLength(25)]
        [Column("phone")]
        public string? phone { get; set; }

        //[Required]
        [MaxLength(120)]
        [Column("addr_line1")]
        public string? addr_line1 { get; set; }// = null!;

        [MaxLength(120)]
        [Column("addr_line2")]
        public string? addr_line2 { get; set; }// = null!;

        //[Required]
        [MaxLength(80)]
        [Column("city")]
        public string? city { get; set; }// = null!;

        //[Required]
        [MaxLength(10)]
        [Column("state_code")]
        public string? state_code { get; set; }// = null!;

        //[Required]
        [MaxLength(12)]
        [Column("zip_code")]
        public string? zip_code { get; set; }// = null!;

        //[Required]
        [Column("created_at")]
        public DateTime created_at { get; set; }

        // Remove???

        //[JsonIgnore]
        //public ICollection<Policy>? policies { get; set; }
        public ICollection<Policy> policies { get; set; } = new List<Policy>();
    }
}
