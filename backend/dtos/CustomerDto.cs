namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class CustomerDto
    {
        public int? customer_id { get; set; }
        public required string name { get; set; }
        public required string email { get; set; }
        public required string phone { get; set; }
        public required string addr_line1 { get; set; }
        public string? addr_line2 { get; set; }
        public required string city { get; set; }
        public required string state_code { get; set; }
        public required string zip_code { get; set; }
        public DateTime created_at { get; set; }

    }
}
