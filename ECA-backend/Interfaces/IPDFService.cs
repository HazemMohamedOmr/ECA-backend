using ECA_backend.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ECA_backend.Interfaces
{
    public interface IPDFService
    {
        Task<AIServicePDFResponse> UploadPdfToAiService(GetPDFRequest request);
    }
}
