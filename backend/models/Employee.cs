using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public int employee_id { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public required string name { get; set; }

        [MaxLength(60)]
        [Column("title")]
        public string? title { get; set; }

        [MaxLength(120)]
        [Column("email")]
        public string? email { get; set; }

        [MaxLength(25)]
        [Column("phone")]
        public string? phone { get; set; }

        [Column("status")]
        public required string status { get; set; } = "Active";

        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.Now;

    }
}
