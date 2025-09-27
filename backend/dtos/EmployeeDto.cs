namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class EmployeeDto
    {
        public int employee_id { get; set; }
        public required string name { get; set; }
        public string? title { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? status { get; set; }
        public DateTime created_at { get; set; }
    }
}