using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
    /// <summary>
    /// Represents an employee within the organization
    /// </summary>
    [Table("Employees")]
    public class Employee
    {
        /// <summary>
        /// The unique identifier for employee
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? employee_id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public required string name { get; set; }

        /// <summary>
        /// The title of employee. Field is optional
        /// </summary>
        [MaxLength(60)]
        [Column("title")]
        public string? title { get; set; }

        [MaxLength(120)]
        [Column("email")]
        public string? email { get; set; }

        [MaxLength(25)]
        [Column("phone")]
        public string? phone { get; set; }

        /// <summary>
        /// The employement status of employee (e.g., Active, Inactive, Leave, Terminated)
        /// </summary>
        [Required]
        [MaxLength(400)]
        [Column("status")]
        public required string status { get; set; } = "Active";

        /// <summary>
        /// The date and time when employee record was created
        /// </summary>
        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
