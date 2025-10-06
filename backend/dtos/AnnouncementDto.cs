namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class AnnouncementDto
    {
        public int? announcement_id { get; set; }
        public required string title { get; set; }
        public required string body { get; set; }
        public required DateTime publish_at { get; set; }
        public required DateTime expire_at { get; set; }
        public required int created_by { get; set; }
        public required DateTime created_at { get; set; }
    }
}
