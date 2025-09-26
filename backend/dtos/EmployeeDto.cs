namespace FalveyProject.backend.dtos
{
    public class EmployeeDto
    {
        public int employee_id { get; set; }
        public required string name { get; set; }
        public string? title { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
    }
}