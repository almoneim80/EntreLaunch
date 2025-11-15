namespace EntreLaunch.Services.ImportSvc
{
    public class ImportService<T, TI> : IImportService<T, TI>
        where TI : BaseEntityWithId
        where T : BaseEntityWithId, new()
    {
        protected AdditionalImportChecker additionalImportChecker = new AdditionalImportChecker();
        private readonly ILogger<ImportService<T, TI>> _logger;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<PgDbContext> _dbContextFactory;

        public ImportService(
            IDbContextFactory<PgDbContext> dbContextFactory,
            IMapper mapper,
            ILogger<ImportService<T, TI>> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ImportResult>> ImportFromListAsync(List<TI> importRecords)
        {
            try
            {
                if (importRecords == null || importRecords.Count == 0)
                {
                    _logger.LogWarning("No data provided for import operation.");
                    return new GeneralResult<ImportResult>(false, "No data provided for import operation.", null);
                }

                var result = new ImportResult();
                var newRecords = new List<T>();
                var EntreLaunchdatedRecords = new List<T>();
                var dEntreLaunchlicates = new Dictionary<TI, object>();

                using var dbContext = _dbContextFactory.CreateDbContext();
                dbContext.IsImportRequest = true;

                var typeIdentifiersMap = BuildTypeIdentifiersMap(importRecords);
                var relatedObjectsMap = BuildRelatedObjectsMap(typeIdentifiersMap, importRecords, newRecords, dEntreLaunchlicates);
                var relatedTObjectsMap = relatedObjectsMap[typeof(T)];

                additionalImportChecker.SetData(importRecords);

                for (var i = 0; i < importRecords.Count; i++)
                {
                    var importRecord = importRecords[i];

                    if (!additionalImportChecker.Check(i, result))
                    {
                        result.Skipped++;
                        result.AddMessage($"Row number {i} skipped due to additional import checker.");
                        continue;
                    }

                    if (dEntreLaunchlicates.TryGetValue(importRecord, out var identifierValue))
                    {
                        string message = $"Row number {i} has a dEntreLaunchlicate identification value {identifierValue} and will be skipped.";
                        _logger.LogInformation(i, message);
                        result.AddError(i, message);
                        result.Skipped++;
                        result.AddMessage($"Item with identifier {identifierValue} skipped because it is a dEntreLaunchlicate.");
                        continue;
                    }

                    BaseEntityWithId? dbRecord = null;
                    foreach (var identifierProperty in relatedTObjectsMap.IdentifierPropertyNames)
                    {
                        var identifierPropertyInfo = importRecord.GetType().GetProperty(identifierProperty)!;
                        var propertyValue = identifierPropertyInfo.GetValue(importRecord);

                        if (propertyValue != null && relatedTObjectsMap[identifierProperty].TryGetValue(propertyValue, out dbRecord))
                        {
                            _mapper.Map(importRecord, dbRecord);
                            EntreLaunchdatedRecords.Add((T)dbRecord!);
                            result.EntreLaunchdated++;
                            result.AddMessage($"Item with Id {dbRecord!.Id} successfully EntreLaunchdated.");
                            break;
                        }
                    }

                    if (dbRecord == null)
                    {
                        dbRecord = _mapper.Map<T>(importRecord);
                        newRecords.Add((T)dbRecord);
                        result.AddMessage($"Item with temporary Id {dbRecord.Id} successfully added.");
                        result.Added++;
                    }
                }

                if (newRecords.Any())
                {
                    await dbContext.Set<T>().AddRangeAsync(newRecords);
                }

                if (EntreLaunchdatedRecords.Any())
                {
                    foreach (var record in EntreLaunchdatedRecords)
                    {
                        dbContext.EntreLaunchdate(record);
                    }
                }

                await dbContext.SaveChangesAsync();

                return new GeneralResult<ImportResult>(true, "Data imported successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing data.");
                return new GeneralResult<ImportResult>(false, "Error occurred while importing data.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ImportResult>> ImportFromFileAsync(IFormFile file)
        {
            try
            {
                List<TI> importRecords;

                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                if (fileExtension == ".csv")
                {
                    importRecords = await ProcessCsvFile(file);
                }
                else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    importRecords = await ProcessExcelFile(file);
                }
                else
                {
                    return new GeneralResult<ImportResult>(false, "UnsEntreLaunchported file format, only CSV and Excel files are sEntreLaunchported.", null);
                }

                return await ImportFromListAsync(importRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing data.");
                return new GeneralResult<ImportResult>(false, "Error occurred while importing data.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<byte[]>> GenerateCsvTemplateAsync<TDto>() where TDto : class
        {
            try
            {
                var memoryStream = new MemoryStream();

                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var properties = typeof(TDto).GetProperties();
                    foreach (var property in properties)
                    {
                        csv.WriteField(property.Name);
                    }
                    await csv.NextRecordAsync();
                }

                return new GeneralResult<byte[]>(true, "CSV template generated successfully.", memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating CSV template.");
                return new GeneralResult<byte[]>(false, "Error occurred while generating CSV template.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<byte[]>> GenerateExcelTemplateAsync<TDto>() where TDto : class
        {
            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Template");

                var properties = typeof(TDto).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                var result = await package.GetAsByteArrayAsync();
                return new GeneralResult<byte[]>(true, "Excel template generated successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating Excel template.");
                return new GeneralResult<byte[]>(false, "Error occurred while generating Excel template.", null);
            }
        }


        /// <summary>
        /// Processes a CSV file and returns a list of records.
        /// </summary>
        private async Task<List<TI>> ProcessCsvFile(IFormFile file)
        {
            var records = new List<TI>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                await foreach (var record in csv.GetRecordsAsync<TI>())
                {
                    records.Add(record);
                }
            }

            return records;
        }

        /// <summary>
        /// Processes an Excel file and returns a list of records.
        /// </summary>
        private async Task<List<TI>> ProcessExcelFile(IFormFile file)
        {
            var records = new List<TI>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var record = Activator.CreateInstance<TI>();
                        var properties = typeof(TI).GetProperties();

                        foreach (var property in properties)
                        {
                            var cellValue = worksheet.Cells[$"{property.Name}{row}"].Text;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                property.SetValue(record, Convert.ChangeType(cellValue, property.PropertyType));
                            }
                        }

                        records.Add(record);
                    }
                }
            }

            return records;
        }

        /// <summary>
        /// Fixes the date kind if needed.
        /// </summary>
        private void FixDateKindIfNeeded(T record)
        {
            if (record is IHasCreatedAt createdAtRecord && createdAtRecord.CreatedAt.Kind != DateTimeKind.Utc)
            {
                createdAtRecord.CreatedAt = createdAtRecord.CreatedAt.ToUniversalTime();
            }

            if (record is IHasEntreLaunchdatedAt EntreLaunchdatedAtRecord)
            {
                if (EntreLaunchdatedAtRecord.EntreLaunchdatedAt.HasValue)
                {
                    EntreLaunchdatedAtRecord.EntreLaunchdatedAt = EntreLaunchdatedAtRecord.EntreLaunchdatedAt.Value.ToUniversalTime();
                }
                else
                {
                    EntreLaunchdatedAtRecord.EntreLaunchdatedAt = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Builds the related objects map.
        /// </summary>
        private TypedRelatedObjectsMap BuildRelatedObjectsMap(TypeIdentifiers typeIdentifiersMap, List<TI> importRecords, List<T> newRecords, Dictionary<TI, object> dEntreLaunchlicates)
        {
            var typedRelatedObjectsMap = new TypedRelatedObjectsMap();

            foreach (var type in typeIdentifiersMap.Keys)
            {
                var identifierValues = typeIdentifiersMap[type];

                var relatedObjectsMap = new RelatedObjectsMap
                {
                    IdentifierPropertyNames = identifierValues.IdentifierPropertyNames,
                    SurrogateKeyPropertyNames = identifierValues.SurrogateKeyPropertyNames,
                    SurrogateKeyPropertyAttributes = identifierValues.SurrogateKeyPropertyAttributes,
                };

                var mappedObjectsCash = new Dictionary<TI, object>();

                foreach (var propertyName in identifierValues.Keys)
                {
                    var existingRecordsProperty = type.GetProperty(propertyName)!;
                    var importRecordsProperty = typeof(TI).GetProperty(propertyName)!;

                    var propertyValues = identifierValues[propertyName];

                    var predicate = BuildPropertyValuesPredicate(type, propertyName, propertyValues);

                    using var dbContext = _dbContextFactory.CreateDbContext();
                    var existingObjectsDict = dbContext.SetDbEntity(type)
                                            .Where(predicate).AsQueryable()
                                            .ToDictionary(x => existingRecordsProperty.GetValue(x)!, x => x);

                    Dictionary<object, TI>? importRecordsDict = null;

                    if (type == typeof(T))
                    {
                        var uniqueGroEntreLaunchs = importRecords
                                            .Select(x => new { Identifier = importRecordsProperty.GetValue(x), Record = x })
                                            .Where(x => x.Identifier != null && x.Identifier.ToString() != "0" && x.Identifier.ToString() != string.Empty)
                                            .GroEntreLaunchBy(x => x.Identifier!);

                        importRecordsDict = uniqueGroEntreLaunchs.ToDictionary(g => g.Key, g => g.First().Record);

                        dEntreLaunchlicates.AddRangeIfNotExists(uniqueGroEntreLaunchs
                                            .Where(g => g.Count() > 1)
                                            .SelectMany(g => g.Skip(1))
                                            .ToDictionary(x => x.Record, x => x.Identifier!));
                    }

                    relatedObjectsMap[propertyName] = propertyValues
                           .Select(uid =>
                           {
                               existingObjectsDict.TryGetValue(uid, out var record);

                               if (type == typeof(T) && importRecordsDict!.TryGetValue(uid, out var importRecord))
                               {
                                   if (record == null && !mappedObjectsCash.TryGetValue(importRecord, out record))
                                   {
                                       record = _mapper.Map<T>(importRecord);
                                       FixDateKindIfNeeded((T)record);
                                       newRecords.Add((T)record);
                                   }

                                   mappedObjectsCash[importRecord] = record;
                               }

                               return new { Uid = uid, Record = record };
                           })
                           .ToDictionary(x => x.Uid, x => x.Record as BaseEntityWithId);
                }

                typedRelatedObjectsMap[type] = relatedObjectsMap;
            }

            return typedRelatedObjectsMap;
        }

        /// <summary>
        /// Builds the type identifiers map.
        /// </summary>
        private TypeIdentifiers BuildTypeIdentifiersMap(List<TI> importRecords)
        {
            var typeIdentifiersMap = new TypeIdentifiers
    {
        { typeof(T), new IdentifierValues() },
    };

            var idValues = importRecords
                .Where(r => r.Id > 0)
                .Select(r => (object)r.Id)
                .Distinct()
                .ToList();

            if (idValues.Count > 0)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var existingIds = dbContext.Set<T>()
                                            .Where(e => idValues.Contains(e.Id))
                                            .Select(e => (object)e.Id)
                                            .ToList();

                if (existingIds.Count > 0)
                {
                    typeIdentifiersMap[typeof(T)]["Id"] = existingIds;
                    typeIdentifiersMap[typeof(T)].IdentifierPropertyNames.Add("Id");
                }
            }

            var uniqueIndexPropertyName = FindAlternateKeyPropertyName();

            if (uniqueIndexPropertyName != null)
            {
                var property = typeof(TI).GetProperty(uniqueIndexPropertyName)!;

                var uniqueValues = importRecords
                                       .Where(r => property.GetValue(r) != null && property.GetValue(r)!.ToString() != string.Empty)
                                       .Select(r => property.GetValue(r))
                                       .Distinct()
                                       .ToList();

                if (uniqueValues.Count > 0)
                {
                    typeIdentifiersMap[typeof(T)][uniqueIndexPropertyName] = uniqueValues!;
                    typeIdentifiersMap[typeof(T)].IdentifierPropertyNames.Add(uniqueIndexPropertyName);
                }
            }

            var importProperties = typeof(TI).GetProperties();

            foreach (var property in importProperties)
            {
                if (property.GetCustomAttributes(typeof(SurrogateForeignKeyAttribute), true).FirstOrDefault() is not SurrogateForeignKeyAttribute surrogateForeignKeyAttribute)
                {
                    continue;
                }

                var type = surrogateForeignKeyAttribute.RelatedType;
                var identifierName = surrogateForeignKeyAttribute.RelatedTypeUniqeIndex;

                var identifierValues = importRecords
                                       .Where(r => property.GetValue(r) != null && property.GetValue(r)!.ToString() != string.Empty)
                                       .Select(r => property.GetValue(r))
                                       .Distinct()
                                       .ToList();

                if (identifierValues.Count == 0)
                {
                    continue;
                }

                if (!typeIdentifiersMap.ContainsKey(type))
                {
                    typeIdentifiersMap[type] = new IdentifierValues();
                }

                if (!typeIdentifiersMap[type].ContainsKey(identifierName))
                {
                    typeIdentifiersMap[type][identifierName] = new List<object>();
                }

                typeIdentifiersMap[type][identifierName].AddRange(identifierValues!);
                typeIdentifiersMap[type][identifierName] = typeIdentifiersMap[type][identifierName].Distinct().ToList();

                typeIdentifiersMap[typeof(T)].SurrogateKeyPropertyNames.Add(property.Name);
                typeIdentifiersMap[typeof(T)].SurrogateKeyPropertyAttributes.Add(surrogateForeignKeyAttribute);
            }

            _logger.LogInformation($"Identifier Map: {string.Join(", ", typeIdentifiersMap[typeof(T)].Keys)}");
            return typeIdentifiersMap;
        }

        /// <summary>
        /// Finds the alternate key property name.
        /// </summary>
        private string FindAlternateKeyPropertyName()
        {
            var uniqueIndexPropertyName = typeof(T).GetCustomAttributes(typeof(IndexAttribute), true)
                                   .Select(a => (IndexAttribute)a)
                                   .Where(a => a.IsUnique)
                                   .Select(a => a.PropertyNames[0]) // for now the assumption is that we do not sEntreLaunchport composite indexes
                                   .FirstOrDefault(); // and we only sEntreLaunchport a single index per entity

            if (uniqueIndexPropertyName is null)
            {
                uniqueIndexPropertyName = typeof(T).GetCustomAttributes(typeof(SurrogateIdentityAttribute), true)
                                       .Select(a => (SurrogateIdentityAttribute)a)
                                       .Select(a => a.PropertyName) // for now the assumption is that we do not sEntreLaunchport composite indexes
                                       .FirstOrDefault(); // and we only sEntreLaunchport a single index per entity
            }

            return uniqueIndexPropertyName!;
        }

        /// <summary>
        /// Builds the property values predicate.
        /// </summary>
        private Func<object, bool> BuildPropertyValuesPredicate(Type targetType, string propertyName, List<object> propertyValues)
        {
            // Get the property info for the property name
            var propertyInfo = targetType.GetProperty(propertyName);

            // Create a parameter expression for the object type
            var objectParam = Expression.Parameter(typeof(object), "o");

            // Convert the object parameter to the target type
            var convertedParam = Expression.Convert(objectParam, targetType);

            // Create the property access expression for the property name
            var propertyAccess = Expression.Property(convertedParam, propertyInfo!);

            // Convert the property access expression to type object
            var convertedPropertyAccess = Expression.Convert(propertyAccess, typeof(object));

            // Create the constant expression for the property values
            var valuesConstant = Expression.Constant(propertyValues, typeof(List<object>));
            var containsMethod = typeof(List<object>).GetMethod("Contains", new[] { typeof(object) });
            var containsExpression = Expression.Call(valuesConstant, containsMethod!, convertedPropertyAccess);

            // Create the lambda expression for the predicate
            var lambdaExpression = Expression.Lambda<Func<object, bool>>(containsExpression, objectParam);

            return lambdaExpression.Compile();
        }

        /// <summary>
        /// Additional import checker for the import.
        /// </summary>
        protected class AdditionalImportChecker
        {
            public virtual void SetData(List<TI> importRecords)
            {
            }

            public virtual bool Check(int index, ImportResult result)
            {
                return true;
            }
        }
    }
}
