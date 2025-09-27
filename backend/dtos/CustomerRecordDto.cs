namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class CustomerRecordDto
    {
        public int record_id { get; set; }
        public required string record_name { get; set; }
        public required string url { get; set; }
        public required int uploaded_by { get; set; }
        public string? uploaded_by_name { get; set; }
        public DateTime uploaded_at { get; set; }
        public required string attached_to_type { get; set; }
        public required int attached_to_id { get; set; }
        public string? description { get; set; }
    }
}