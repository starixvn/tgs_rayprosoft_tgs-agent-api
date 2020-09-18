namespace TGS.Apis.BackendOffice.Services
{
    //public interface IUploadService
    //{
    //    Task<List<List<string>>> GetUploadedExcelAsync(HttpContent httpContent);

    //    Task<ValidateExcelResponse> ValidateExcelAsync(HttpRequestMessage request);
    //}

    //public class UploadService : IUploadService
    //{
    //    async Task<List<List<string>>> IUploadService.GetUploadedExcelAsync(HttpContent httpContent)
    //    {
    //        var newTable = new List<List<string>>();
    //        using (var package = new ExcelPackage(await httpContent.ReadAsStreamAsync()))
    //        {
    //            var sheet = package.Workbook.Worksheets[1];
    //            if (sheet.Dimension != null)
    //            {
    //                for (var currentRow = sheet.Dimension.Start.Row; currentRow <= sheet.Dimension.End.Row; currentRow++)
    //                {
    //                    var rowData = new List<string>();
    //                    for (var currentColumn = sheet.Dimension.Start.Column; currentColumn <= sheet.Dimension.End.Column; currentColumn++)
    //                    {
    //                        var range = Convert.ToString(sheet.Cells[currentRow, currentColumn, currentRow, currentColumn].Value);
    //                        rowData.Add(range);
    //                    }
    //                    if (!rowData.Any(c => c.IsNotNullOrEmpty())) continue;

    //                    newTable.Add(rowData);
    //                }
    //            }

    //            return newTable;
    //        }
    //    }

    //    async Task<ValidateExcelResponse> IUploadService.ValidateExcelAsync(HttpRequestMessage request)
    //    {
    //        if (!request.Content.IsMimeMultipartContent())
    //        {
    //            return new ValidateExcelResponse(Resources.Common.UnsupportedMediaType);
    //        }

    //        var provider = await request.Content.ReadAsMultipartAsync();
    //        var httpContent = provider.Contents.FirstOrDefault();

    //        if (httpContent == null)
    //        {
    //            return new ValidateExcelResponse(Resources.Common.UnsupportedMultiPartContent, httpContent);
    //        }

    //        var filename = httpContent.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
    //        if (Path.GetExtension(filename).IgnoreCaseNotEquals(".xlsx"))
    //        {
    //            return new ValidateExcelResponse(Resources.Common.PleaseUploadXlsxFile, httpContent);
    //        }

    //        return new ValidateExcelResponse
    //        {
    //            Valid = true,
    //            HttpContent = httpContent
    //        };
    //    }
    //}
}
