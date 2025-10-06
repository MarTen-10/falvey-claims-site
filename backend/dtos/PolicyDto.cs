namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class PolicyDto
    {
        public int? policy_id { get; set; }
        public required string account_number { get; set; }
        public required int customer_id { get; set; }
        public string? customer_name { get; set; }
        public int? manager_id { get; set; }
        public string? manager_name { get; set; }
        public required string policy_type { get; set; }
        public required string status { get; set; }
        public required DateTime start_date { get; set; }
        public required DateTime end_date { get; set; }
        public required decimal exposure_amount { get; set; }
        public required string loc_addr1 { get; set; }
        public string? loc_addr2 { get; set; }
        public required string loc_city { get; set; }
        public required string loc_state { get; set; }
        public required string loc_zip { get; set; }
        public required DateTime created_at { get; set; }
    }
}
