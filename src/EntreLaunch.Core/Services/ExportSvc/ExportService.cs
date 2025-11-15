namespace EntreLaunch.Services.ExportSvc
{
    public class ExportService : IExportService
    {
        private readonly PgDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ExportService> _logger;
        public ExportService(PgDbContext dbContextFactory, IMapper mapper, ILogger<ExportService> logger)
        {
            _dbContext = dbContextFactory;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> ExportToCsvAsync<T, TDto>()
            where T : SharedData, new()
            where TDto : class
        {
            try
            {
                var entities = await _dbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
                if (!entities.Any())
                {
                    return new GeneralResult<string>(false, "No data found.");
                }

                var dtos = _mapper.Map<List<TDto>>(entities);
                return new GeneralResult<string>(true, "Data exported to csv successfully.", ConvertToCsv(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data to CSV.");
                return new GeneralResult<string>(false, "Error exporting data to CSV.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<byte[]>> ExportToExcelAsync<T, TDto>()
            where T : SharedData, new()
            where TDto : class
        {
            try
            {
                var entities = await _dbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
                if (!entities.Any())
                {
                    return new GeneralResult<byte[]>(false, "No data found.");
                }

                var dtos = _mapper.Map<List<TDto>>(entities);
                return new GeneralResult<byte[]>(true, "Data exported to excel successfully.", ConvertToExcel(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data to Excel.");
                return new GeneralResult<byte[]>(false, "Error exporting data to Excel.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<string>> ExportToJsonAsync<T, TDto>()
            where T : SharedData, new()
            where TDto : class
        {
            try
            {
                var entities = await _dbContext.Set<T>().AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
                if (!entities.Any())
                {
                    return new GeneralResult<string>(false, "No data found.");
                }

                var dtos = _mapper.Map<List<TDto>>(entities);
                return new GeneralResult<string>(true, "Data exported to json successfully.", ConvertToJson(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data to JSON.");
                return new GeneralResult<string>(false, "Error exporting data to JSON.", null);
            }
        }

        // private methods

        /// <summary>
        /// Converts a list of DTOs to a CSV string with the specified properties.
        /// </summary>
        private string ConvertToCsv<TDto>(IEnumerable<TDto> data)
        {
            var stringBuilder = new StringBuilder();
            var properties = typeof(TDto).GetProperties();

            // Write header
            stringBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Write rows
            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);

                    if (value is IEnumerable<string> list)
                    {
                        // Convert list to a comma-separated string, escaping commas and quotes
                        return "[ " + string.Join(", ", list.Select(v => v.Replace(" ]", "\"\""))) + "\"";
                    }
                    else
                    {
                        var stringValue = value?.ToString()?.Replace(",", " ").Replace("\"", "\"\"") ?? string.Empty;
                        return "\"" + stringValue + "\"";
                    }
                });

                stringBuilder.AppendLine(string.Join(",", values));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a list of DTOs to an Excel file.
        /// </summary>
        private byte[] ConvertToExcel<TDto>(IEnumerable<TDto> data)
        {
            // License
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Export");

            // Get properties of the DTO
            var properties = typeof(TDto).GetProperties();

            // Write header
            for (var i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }

            // Write rows
            var rowIndex = 2;
            foreach (var item in data)
            {
                for (var colIndex = 0; colIndex < properties.Length; colIndex++)
                {
                    var value = properties[colIndex].GetValue(item);

                    if (value is IEnumerable<string> list)
                    {
                        // Convert list to a string surrounded by brackets and separated by commas
                        worksheet.Cells[rowIndex, colIndex + 1].Value = "\"" + string.Join(", ", list) + "\"";
                    }
                    else
                    {
                        worksheet.Cells[rowIndex, colIndex + 1].Value = value?.ToString();
                    }
                }

                rowIndex++;
            }

            // Auto-fit columns for better visibility
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

        /// <summary>
        /// Converts a list of DTOs to a JSON string.
        /// </summary>
        private string ConvertToJson<TDto>(IEnumerable<TDto> data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }
}
