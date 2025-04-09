using ECA_backend.Contracts;
using ECA_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECA_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        private readonly IPDFService _pdfService;

        public PDFController(IPDFService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost("Analyze")]
        public async Task<IActionResult> getExtractedPDF([FromForm] GetPDFRequest request)
        {
            var result = await _pdfService.UploadPdfToAiService(request);
            return Ok(result);
        }
    }
}
