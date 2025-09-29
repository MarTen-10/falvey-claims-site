using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyInsuranceGroup.Backend.Models
{
	[Table("users")]
	public class User
	{
		[Key]
        [Column("user_id")]
        public int user_id { get; set; }

        [MaxLength(120)]
        [Column("email")]
        public string email { get; set; }

        [MaxLength(200)]
        [Column("password_hash")]
        public string password_hash { get; set; }

        [Column("role")]
        public string role { get; set; }

        [Column("customer_id")]
        public int? customer_id { get; set; }

        [ForeignKey("customer_id")]
        public Customer? customer { get; set; } // links to customer_id

        [Column("employee_id")]
        public int? employee_id { get; set; }

        [ForeignKey("employee_id")]
        public Employee? employee { get; set; } // links to employee_id 

        [Column("is_active")]
        public bool is_active { get; set; } = true;
        
        [Column("created_at")]
        public required DateTime created_at { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime? updated_at { get; set; } = null;

    }
}
