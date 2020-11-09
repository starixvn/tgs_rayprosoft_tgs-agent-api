using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.BackendOffice.Services
{
    public interface IUploadService
    {
        Task<List<List<string>>> GetUploadedExcelAsync(IFormFile file);
    }

    public class UploadService : IUploadService
    {
        async Task<List<List<string>>> IUploadService.GetUploadedExcelAsync(IFormFile file)
        {
            var result = new List<List<string>>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var fieldCount = reader.FieldCount;
                    while (reader.Read()) //Each row of the file
                    {
                        var rows = new List<string>();
                        for (var i = 0; i < fieldCount; i++) rows.Add(reader.GetValue(i).ToString());
                        result.Add(rows);
                    }
                }
            }

            return result;
        }
    }
}