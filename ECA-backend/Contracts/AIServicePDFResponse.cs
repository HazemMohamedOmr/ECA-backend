namespace ECA_backend.Contracts
{
    public record AIServicePDFResponse
    {
        public string Status { get; set; }
        public string LastUpdateDataTime { get; set; }
        public string createdDataTime { get; set; }
        public AnalyzePDFResult analyzeResult { get; set; }
    }

    public record AnalyzePDFResult
    {
        public string apiVersion { get; set; }
        public string modelId { get; set; }
        public string stringIndexType { get; set; }
        public string content { get; set; }
    }
}
