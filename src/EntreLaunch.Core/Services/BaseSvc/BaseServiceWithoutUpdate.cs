namespace EntreLaunch.Services.BaseSvc
{
    public class BaseServiceWithoutEntreLaunchdate<T, TC, TD>
        where T : SharedData, new()
        where TC : class
        where TD : class
    {
        protected readonly IMapper _mapper;
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;
        private readonly string _cacheKeyPrefix;
        private readonly CacheSettings _cacheSettings;

        public BaseServiceWithoutEntreLaunchdate(
            IDbContextFactory<PgDbContext> dbContextFactory,
            IMapper mapper,
            ICacheService cacheService,
            IOptions<CacheSettings> cacheSettings)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _cacheService = cacheService;
            _cacheSettings = cacheSettings.Value;
            _cacheKeyPrefix = typeof(T).Name.ToLower(); // Key Prefix for all cache entries of this type
        }

        // Get
        public async Task<List<TD>> GetAllAsync()
        {
            string cacheKey = $"{_cacheKeyPrefix}_all";
            var cachedData = await _cacheService.GetAsync<List<TD>>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            var entities = await dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
            var result = _mapper.Map<List<TD>>(entities);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes),
                useSlidingExpiration: true,
                _cacheSettings.CacheItemPriority,
                _cacheSettings.CacheItemSize);

            return result;
        }
        public async Task<TD?> GetOneAsync(int id)
        {
            string cacheKey = $"{_cacheKeyPrefix}_one_{id}";
            var cachedData = await _cacheService.GetAsync<TD>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            var entity = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            var result = _mapper.Map<TD>(entity);

            await _cacheService.SetAsync(
                    cacheKey,
                    result,
                    TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes),
                    useSlidingExpiration: true,
                    _cacheSettings.CacheItemPriority,
                    _cacheSettings.CacheItemSize);
            return result;
        }

        // Post
        public async Task<TD> CreateAsync(TC createDto)
        {
            var entity = _mapper.Map<T>(createDto);
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Set<T>().Add(entity);
            await dbContext.SaveChangesAsync();
            return _mapper.Map<TD>(entity);
        }

        // Delete
        public virtual async Task<bool> DeleteAsync(int id)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entity = await dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();

            return true;
        }

        // Csv export
        public async Task<string> ExportToCsvAsync<TDto>()
            where TDto : class
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entities = await dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
            var dtos = _mapper.Map<List<TDto>>(entities);

            return ConvertToCsv(dtos);
        }

        // Excel export
        public async Task<byte[]> ExportToExcelAsync<TDto>()
            where TDto : class
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entities = await dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
            var dtos = _mapper.Map<List<TDto>>(entities);

            return ConvertToExcel(dtos);
        }

        // json export
        public async Task<string> ExportToJsonAsync<TDto>()
        where TDto : class
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var entities = await dbContext.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
            var dtos = _mapper.Map<List<TDto>>(entities);

            return ConvertToJson(dtos);
        }

        // RemoveSecondLevelObjects
        public List<TD> RemoveSecondLevelObjects(IList<TD> data)
        {
            var refs = new Dictionary<PropertyInfo, List<PropertyInfo>>();

            foreach (var property in typeof(TD).GetProperties())
            {
                if (property.PropertyType.GetInterface("IEnumerable") != null && property.PropertyType.IsGenericType)
                {
                    var innerType = property.PropertyType.GetGenericArguments()[0];
                    var nestedProps = innerType.GetProperties()
                        .Where(p => !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string))
                        .ToList();

                    if (nestedProps.Any())
                    {
                        refs[property] = nestedProps;
                    }
                }
            }

            foreach (var item in data)
            {
                foreach (var r in refs)
                {
                    var propertyObject = r.Key.GetValue(item);
                    if (propertyObject != null)
                    {
                        if (r.Key.PropertyType.GetInterface("IEnumerable") != null && r.Key.PropertyType.IsGenericType)
                        {
                            var e = propertyObject as System.Collections.IEnumerable;
                            foreach (var obj in e!)
                            {
                                foreach (var p in r.Value)
                                {
                                    p.SetValue(obj, null);
                                }
                            }
                        }
                        else
                        {
                            foreach (var p in r.Value)
                            {
                                p.SetValue(propertyObject, null);
                            }
                        }
                    }
                }
            }

            return data.ToList();
        }

        /// <summary>
        /// Converts a collection of objects to a CSV string.
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
                var values = properties.Select(p => p.GetValue(item)?.ToString()?.Replace(",", " ") ?? string.Empty);
                stringBuilder.AppendLine(string.Join(",", values));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a collection of objects to an Excel file.
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
                    worksheet.Cells[rowIndex, colIndex + 1].Value = value?.ToString();
                }

                rowIndex++;
            }

            // Auto-fit columns for better visibility
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

        /// <summary>
        /// Converts a collection of objects to a JSON string.
        /// </summary>
        private string ConvertToJson<TDto>(IEnumerable<TDto> data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }
    }
}
