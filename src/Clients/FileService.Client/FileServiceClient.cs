using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Apis.FileService.Clients
{
    class FileServiceClient
    {
        private readonly HttpClient _httpClient;

        public FileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task UploadFileAsync(string path, Stream fileStream)
        {
            using var uploadContent = new MultipartFormDataContent();

            using var fileContent = new StreamContent(fileStream);

            uploadContent.Add(fileContent, "file");

            var response = await _httpClient
                .PostAsync(path, uploadContent)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);


        }
    }
}
