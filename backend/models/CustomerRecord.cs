using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("customer_records")]
    public class CustomerRecord
    {
        [Key]
        [Column("record_id")]
        public int record_id { get; set; }

        [MaxLength(200)]
        [Column("record_name")]
        public required string record_name { get; set; }

        [MaxLength(500)]
        [Column("url")]
        public required string url { get; set; }
        
        [Column("uploaded_by")]
        public required int uploaded_by { get; set; }

        [ForeignKey("uploaded_by")]
        public Employee? uploaded_by_employee { get; set; }  // links to employee_id

        [Column("uploaded_at")]
        public required DateTime uploaded_at { get; set; } = DateTime.Now;

        [Column("attached_to_type")]
        public required string attached_to_type { get; set; }

        [Column("attached_to_id")]
        public required int attached_to_id { get; set; }

        [Column("description")]
        public string? description { get; set; }
    }
}