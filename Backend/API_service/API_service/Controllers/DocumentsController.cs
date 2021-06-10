using Microsoft.AspNetCore.Mvc;
using API_service.Services;
using API_service.Models;

namespace API_service.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public DocumentsController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public IActionResult GetAllDocuments()
        {
            // Gets the list of documents from the database
            return Ok(_documentService.Get());
        }

        [HttpGet("{id:length(24)}")]
        public IActionResult GetDocumentById(string id)
        {
            // Gets a specific document from the database 
            Document document = _documentService.GetById(id);
            // If the document doesn't exist in the database
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        [HttpGet("{name}")]
        public IActionResult GetDocumentByName(string name)
        {
            // Gets a specific document from the database 
            Document document = _documentService.GetByName(name);
            // If the document doesn't exist in the database
            if (document == null)
                return NotFound();

            return Ok(document);
        }

    }
}
