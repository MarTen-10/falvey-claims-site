using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FalveyProject.backend.models
{
    public class Document
    {
        [Key]
        public int document_id { get; set; }
        public required string file_name { get; set; }
        public required string url { get; set; }
        public int? uploaded_by { get; set; }

        [ForeignKey("uploaded_by")]
         public Employee? UploadedBy { get; set; }
        public DateTime uploaded_at { get; set; }
        public required string attached_to_type { get; set; }
        public required int attached_to_id { get; set; }
        public string? description { get; set; }
    }
}