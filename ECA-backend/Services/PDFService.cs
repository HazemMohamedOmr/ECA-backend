using System.Net.Http.Headers;
using System.Text.Json;
using ECA_backend.Contracts;
using ECA_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECA_backend.Services
{
    public class PDFService : IPDFService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string aiServiceUploadUrl = "https://doccumentintelligenceresourceresource1.cognitiveservices.azure.com/documentintelligence/documentModels/ECA_model:analyze?_overload=analyzeDocument&api-version=2024-11-30";
        private readonly string HeaderAuthName = "Ocp-Apim-Subscription-Key";
        private readonly string HeaderGETName = "Operation-Location";
        private readonly string ApiKey = "DI7aNDyMVllNyngXZhgQo7u7h3PlsDksqmwyz3lSUUVq112tCC4aJQQJ99BCACYeBjFXJ3w3AAALACOGQtw6";

        public PDFService()
        {
            client.DefaultRequestHeaders.Add(HeaderAuthName, ApiKey);
        }
        public async Task<AIServicePDFResponse> UploadPdfToAiService(GetPDFRequest request)
        {
            IFormFile pdfFile = request.Pdf;
            using (var formData = new MultipartFormDataContent())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await pdfFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var pdfContent = new StreamContent(memoryStream);
                    pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    formData.Add(pdfContent, "file", pdfFile.FileName);

                    var response = await client.PostAsync(aiServiceUploadUrl, formData);
                    response.EnsureSuccessStatusCode();

                    if (response.Headers.TryGetValues(HeaderGETName, out var headerValues))
                    {
                        var url = headerValues.FirstOrDefault() ?? throw new Exception("Operation-Location header not found");
                        var result = await PollForResult(url);
                        return result;
                    }
                    throw new Exception("Operation-Location header not present in response");
                }
            }
        }

        private async Task<AIServicePDFResponse> PollForResult(string url)
        {
            while (true)
            {
                var response = await client.GetAsync(url);
                var jsonResult = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get result: {response.StatusCode}");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                AIServicePDFResponse result = JsonSerializer.Deserialize<AIServicePDFResponse>(jsonResult, options)!;

                if (IsProcessingComplete(result))
                {
                    return result;
                }

                await Task.Delay(500);
            }
        }

        private bool IsProcessingComplete(AIServicePDFResponse result)
        {
            return result?.Status?.ToLower() == "succeeded";
        }
    }
}
