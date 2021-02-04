using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;

namespace TGS.Services.BackOffice.Infrastructure.Services
{
    public interface IUploadService
    {
        Task<List<T>> GetUploadedExcelAsync<T>(IFormFile file) where T : new();
    }

    public class UploadService : IUploadService
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public UploadService(IJsonSerializationProvider jsonSerializationProvider)
        {
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        async Task<List<T>> IUploadService.GetUploadedExcelAsync<T>(IFormFile file)
        {
            var result = new List<T>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = ExcelReaderFactory.CreateReader(stream);

            reader.Read();
            var headers = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++) headers.Add(reader.GetValue(i).ToString());
            var fieldCount = reader.FieldCount;
            while (reader.Read()) //Each row of the file
            {
                var obj = new ExpandoObject();

                for (var i = 0; i < fieldCount; i++)
                {
                    var header = headers[i];
                    obj.TryAdd(header, reader.GetValue(i));
                    ((IDictionary<string, object>) obj)[header] = reader.GetValue(i)?.ToString();
                }

                var json = _jsonSerializationProvider.SerializeToString(obj);
                result.Add(_jsonSerializationProvider.DeserializeFromString<T>(json));
            }

            return result;
        }
    }
}