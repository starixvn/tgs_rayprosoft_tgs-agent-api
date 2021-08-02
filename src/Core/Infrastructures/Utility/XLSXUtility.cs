using System.Collections.Generic;
using ClosedXML.Excel;

namespace TGS.Core.Infrastructures.Utility
{
    public static class XLSXUtility
    {
        public static XLWorkbook Export<T>(this IEnumerable<T> models, string sheetName)
        {
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(sheetName);
            var colIdx = 1;

            foreach (var item in typeof(T).GetProperties())
            {
                sheet.Cell(1, colIdx++).Value = item.Name;
            }

            var rowIdx = 2;
            foreach (var model in models)
            {
                var columnIndex = 1;
                foreach (var propertyInfo in model.GetType().GetProperties())
                {
                    sheet.Cell(rowIdx, columnIndex).Value = propertyInfo.GetValue(model);
                    columnIndex++;
                }

                rowIdx++;
            }

            return workbook;
        }
    }
}