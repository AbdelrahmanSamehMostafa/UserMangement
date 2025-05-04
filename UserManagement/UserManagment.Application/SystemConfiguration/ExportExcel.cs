using Microsoft.Extensions.Logging;
using ClosedXML.Excel;


namespace UserManagment.Application.SystemConfiguration
{
    public static class ExportExcel
    {
        /// <summary>
        /// Generates an Excel file without images based on the provided data, column names, and property names.
        /// </summary>
        /// <typeparam name="T">The type of data to be exported.</typeparam>
        /// <param name="data">A list of data objects to be included in the Excel file.</param>
        /// <param name="columnNames">A list of column names to be used as headers in the Excel file.</param>
        /// <param name="propertyNames">A list of property names to extract values from the data objects.</param>
        /// <param name="fileName">The name of the Excel file to be generated.</param>
        /// <param name="_logger">The logger used to log information and errors.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> containing the generated Excel file.
        /// </returns>
        /// <exception cref="Exception">Thrown if an error occurs during Excel file generation.</exception>
        public static async Task<MemoryStream> GenerateExcelAsync<T>(
            List<T> data,
            List<string> columnNames,
            List<string> propertyNames,
            string fileName,
            ILogger<dynamic> _logger) where T : class
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(fileName);

                    worksheet.ShowGridLines = false;
                    worksheet.RightToLeft = true;

                    // Add Header Row
                    var headerRow = 1;
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        var cell = worksheet.Cell(headerRow, i + 1);
                        cell.Value = columnNames[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center align the header
                    }

                    // Populate Data Rows
                    var currentRow = headerRow + 1; // Start from the row below the header
                    foreach (var item in data)
                    {
                        for (int i = 0; i < propertyNames.Count; i++)
                        {
                            var propertyValue = item.GetType().GetProperty(propertyNames[i])?.GetValue(item, null);

                            if (propertyValue != null)
                            {
                                var cell = worksheet.Cell(currentRow, i + 1);
                                cell.Value = propertyValue.ToString();
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Center align the cell
                            }
                        }
                        currentRow++;
                    }

                    // Apply Borders Only to Populated Rows
                    int lastColumnIndex = Math.Max(columnNames.Count, 1);
                    for (int row = 1; row < currentRow; row++) // Changed from <= to <
                    {
                        for (int col = 1; col <= lastColumnIndex; col++)
                        {
                            var cell = worksheet.Cell(row, col);
                            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        }
                    }

                    // Adjust Column Widths and Finalize Workbook
                    worksheet.Columns().AdjustToContents();
                    worksheet.RightToLeft = true;

                    var memoryStream = new MemoryStream();
                    workbook.SaveAs(memoryStream);
                    memoryStream.Position = 0;

                    _logger.LogInformation("Successfully generated Excel report.");
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating Excel report.");
                throw;
            }
        }
    }
}
