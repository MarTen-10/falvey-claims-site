using System.ComponentModel.DataAnnotations;

namespace FalveyProject.backend.models
{
    public class Employee
    {
        [Key]
        public int employee_id { get; set; }
        public required string name { get; set; }
        public string? title { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public required string status { get; set; }
        public DateTime created_at { get; set; }

    }

    
}