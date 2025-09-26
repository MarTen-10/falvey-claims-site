using Microsoft.AspNetCore.Mvc;
using FalveyProject.backend.dtos;
using FalveyProject.database;
using Microsoft.EntityFrameworkCore;
using FalveyProject.backend.models;

namespace FalveyProject.backend.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly BaseContext _context;

        public DocumentsController(BaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentDto>>> getDocuments()
        {
            var documents = await _context.Documents
            .Include(d => d.UploadedBy)
            .Select(d => new DocumentDto
            {
                document_id = d.document_id,
                file_name = d.file_name,
                url = d.url,
                uploaded_by = d.uploaded_by,
                uploaded_by_name = d.UploadedBy != null ? d.UploadedBy.name : null,
                uploaded_at = d.uploaded_at,
                attached_to_type = d.attached_to_type,
                attached_to_id = d.attached_to_id,
                description = d.description
            }).ToListAsync();


            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentDto>> getDocument(int id)
        {
            var document = await getDocumentById(id);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(createDocumentDto(document));
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> postDocument(Document doc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getDocument), new { id = doc.document_id }, createDocumentDto(doc));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> updateDocument(int id, Document doc)
        {
            if (id != doc.document_id)
            {
                return BadRequest();
            }

            _context.Entry(doc).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteDocument(int id)
        {
            var doc = await getDocumentById(id);

            if (doc == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private async Task<Document?> getDocumentById(int id)
        {
            var document = await _context.Documents
            .Include(d => d.UploadedBy)
            .FirstOrDefaultAsync(d => d.document_id == id);

            return document != null ? document : null;
        }
    

        private DocumentDto createDocumentDto(Document d)
        {
            return new DocumentDto
            {
                document_id = d.document_id,
                file_name = d.file_name,
                url = d.url,
                uploaded_by = d.uploaded_by,
                uploaded_by_name = d.UploadedBy != null ? d.UploadedBy.name : null,
                uploaded_at = d.uploaded_at,
                attached_to_type = d.attached_to_type,
                attached_to_id = d.attached_to_id,
                description = d.description
            };
        }

    }
}