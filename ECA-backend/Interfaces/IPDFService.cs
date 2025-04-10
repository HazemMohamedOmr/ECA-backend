using ECA_backend.Contracts;

namespace ECA_backend.Interfaces
{
    public interface IPDFService
    {
        Task<AIServicePDFResponse> UploadPdfToAiService(GetPDFRequest request);
    }
}
