namespace ECA_backend.Contracts
{
    public record GetPDFRequest
    {
        public IFormFile Pdf { get; init; }
    }
}
