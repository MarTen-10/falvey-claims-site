using FalveyInsuranceGroup.Backend.Models;

namespace FalveyInsuranceGroup.Backend.Dtos
{
    public class SessionDto
    {
        public Guid? session_id { get; set; }
        public required int user_id { get; set; }
        public string? session_hash { get; set; }
        public required DateTime created_at { get; set; }
        public DateTime? expires_at { get; set; }
        public DateTime? revoked_at { get; set; }
        public string? ip_address { get; set; }
        public string? user_agent { get; set; }
    }
}
