using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.CertificateDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.TrainingSvc
{
    public class CertificateService(PgDbContext dbContext, ILogger<CertificateService> logger, ILocalizationManager localizationManager) : ICertificateService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<CertificateService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> IssuePathCertificateAsync(int pathId, string userId)
        {
            try
            {
                // check if user Id is provided.
                if (string.IsNullOrEmpty(userId))
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserIdRequired"));
                }

                // check if user exists
                var user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                // check if user is subscribed to the path
                var pathSubscription = await _dbContext.Subscriptions.AnyAsync(p =>
                p.ReferenceId == pathId && p.UserId == userId && !p.IsDeleted && p.Type == SubscriptionType.TrainingPath && p.Status == SubscriptionStatus.Active);
                if (!pathSubscription)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotSubscribedToPath"));
                }

                // check if the path exists
                var path = await _dbContext.TrainingPaths.FirstOrDefaultAsync(c => c.Id == pathId && !c.IsDeleted);
                if (path == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CourseNotFound"));
                }

                // check if the user has completed the path
                var progress = await _dbContext.StudentProgresses.FirstOrDefaultAsync(p => p.PathId == pathId && p.UserId == userId);
                if (progress == null || !progress.IsCompleted)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CourseNotCompleted"));
                }

                // calculate the expiration date
                var certificateExpirationDate = DateTimeOffset.UtcNow;
                if(path.CertificateValidityInDays > 0)
                {
                    certificateExpirationDate = DateTimeOffset.UtcNow.AddDays(path.CertificateValidityInDays);
                }

                // Issue the certificate
                var newCertificate = new Certificate
                {
                    UserId = userId,
                    CourseId = null,
                    PathId = pathId,
                    IssuedAt = DateTimeOffset.UtcNow,
                    CertificateType = Enums.StudentCertificateType.Path,
                    DeliveryMethod = DeliveryMethod.Online,
                    ExpirationDate = certificateExpirationDate,
                    ShippingStatus = ShippingStatus.NotRequired,
                    ShippingAddress = null
                };

                var addResult = _dbContext.Certificates.Add(newCertificate);
                await _dbContext.SaveChangesAsync();

                var dto = new CertificateDetailsDto
                {
                    Id = addResult.Entity.Id,
                    CertificateFor = path.Name,
                    IssuedAt = addResult.Entity.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = addResult.Entity.CertificateId,
                    ExpirationDate = path.CertificateValidityInDays > 0
                    ? addResult.Entity.ExpirationDate ?? DateTimeOffset.UtcNow
                    : default,
                    ShippingStatus = ShippingStatus.NotRequired,
                    ShippingAddress = null,
                    Student = new StudentData
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        NationalId = user.NationalId ?? 0,
                        PhoneNumber = user.PhoneNumber,
                        Specialization = user.Specialization
                    }
                };

                return new GeneralResult(true, _localizationManager.GetLocalizedString("CertificateIssued"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing a certificate.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateIssuanceFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> IssueCourseCertificateAsync(int courseId, string userId)
        {
            try
            {
                // check if user Id is provided.
                if (string.IsNullOrEmpty(userId))
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserIdRequired"));
                }

                // check if user exists
                var user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                // check if the course exists
                var course = await _dbContext.Courses.Where(c => c.Id == courseId && !c.IsDeleted).Select(c =>
                new { c.PathId, c.Type, c.CertificateValidityInDays, c.Name, }).FirstOrDefaultAsync();
                if (course == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CourseNotFound"));
                }

                if (course.Type == CourseType.PathCourse && course.PathId.HasValue)
                {
                    if (!await _dbContext.TrainingPaths.AnyAsync(c => c.Id == course.PathId && !c.IsDeleted))
                    {
                        return new GeneralResult(false, _localizationManager.GetLocalizedString("pathNotFound"));
                    }
                }

                var coursePurchase = false;
                switch (course.Type)
                {
                    case CourseType.OnlineCourse:
                        coursePurchase = await _dbContext.Purchases.AnyAsync(p =>
                        p.ReferenceId == courseId && p.UserId == userId && !p.IsDeleted
                        && p.ItemType == PurchaseItemType.OnlineCourse && !p.IsRefunded);
                        break;
                    case CourseType.SkillsLibCourse:
                        coursePurchase = await _dbContext.Purchases.AnyAsync(p =>
                        p.ReferenceId == courseId && p.UserId == userId && !p.IsDeleted
                        && p.ItemType == PurchaseItemType.SkillsLibCourse && !p.IsRefunded);
                        break;
                    case CourseType.PathCourse:
                        coursePurchase = await _dbContext.Subscriptions.AnyAsync(p =>
                        p.ReferenceId == course.PathId && p.UserId == userId && !p.IsDeleted
                        && p.Status == SubscriptionStatus.Active);
                        break;
                    default:
                        break;
                }
                
                if(coursePurchase == false)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotSubscribedToCourse"));
                }

                // check if the user has completed the course
                var progress = await _dbContext.StudentProgresses.FirstOrDefaultAsync(p => p.CourseId == courseId && p.UserId == userId);
                if (progress == null || !progress.IsCompleted)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CourseNotCompleted"));
                }

                // calculate the expiration date of the certificate 
                var certificateExpirationDate = DateTimeOffset.UtcNow;
                if (course.CertificateValidityInDays > 0)
                {
                    certificateExpirationDate = DateTimeOffset.UtcNow.AddDays(course.CertificateValidityInDays);
                }

                // Issue the certificate
                var newCertificate = new Certificate
                {
                    UserId = userId,
                    CourseId = courseId,
                    PathId = null,
                    IssuedAt = DateTimeOffset.UtcNow,
                    CertificateType = Enums.StudentCertificateType.Course,
                    DeliveryMethod = DeliveryMethod.Online,
                    ExpirationDate = certificateExpirationDate,
                    ShippingStatus = ShippingStatus.NotRequired,
                    ShippingAddress = null
                };

                var addResult = _dbContext.Certificates.Add(newCertificate);
                await _dbContext.SaveChangesAsync();

                if (addResult == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateIssuanceFailed"));
                }

                var dto = new CertificateDetailsDto
                {
                    Id = addResult.Entity.Id,
                    CertificateFor = course.Name,
                    IssuedAt = addResult.Entity.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = addResult.Entity.CertificateId,
                    ExpirationDate = course.CertificateValidityInDays > 0
                        ? addResult.Entity.ExpirationDate ?? DateTimeOffset.UtcNow
                        : default,
                    ShippingStatus = ShippingStatus.NotRequired,
                    ShippingAddress = null,
                    Student = new StudentData
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        NationalId = user.NationalId ?? 0,
                        PhoneNumber = user.PhoneNumber,
                        Specialization = user.Specialization
                    }
                };

                return new GeneralResult(true, _localizationManager.GetLocalizedString("CertificateIssued"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while issuing a certificate.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateIssuanceFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CertificateDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Certificates
                    .AsNoTracking()
                    .Where(c => !c.IsDeleted)
                    .Include(c => c.User)
                    .Include(c => c.Course)
                    .Include(c => c.Path)
                    .OrderByDescending(c => c.IssuedAt);

                var pagedResult = await query
                    .Select(c => new CertificateDetailsDto
                    {
                        Id = c.Id,
                        CertificateFor = c.CertificateType == Enums.StudentCertificateType.Path ? c.Path!.Name : c.Course!.Name,
                        IssuedAt = c.IssuedAt ?? DateTimeOffset.UtcNow,
                        CertificateId = c.CertificateId,
                        ExpirationDate = c.ExpirationDate ?? DateTimeOffset.MinValue,
                        ShippingStatus = c.ShippingStatus,
                        ShippingAddress = c.ShippingAddress,
                        Student = c.User != null ? new StudentData
                        {
                            FirstName = c.User.FirstName,
                            LastName = c.User.LastName,
                            Email = c.User.Email,
                            NationalId = c.User.NationalId ?? 0,
                            PhoneNumber = c.User.PhoneNumber,
                            Specialization = c.User.Specialization
                        }
                        : null
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogInformation("No certificates found.");
                    return new GeneralResult<PaginatedResult<CertificateDetailsDto>>(false, _localizationManager.GetLocalizedString("NoCertificatesFound"), null);
                }

                _logger.LogInformation("Retrieved {Count} certificates.", pagedResult.Items.Count);
                return new GeneralResult<PaginatedResult<CertificateDetailsDto>>(true, _localizationManager.GetLocalizedString("CertificatesRetrieved"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving certificates.");
                return new GeneralResult<PaginatedResult<CertificateDetailsDto>>(false, _localizationManager.GetLocalizedString("CertificatesRetrievalFailed"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CertificateDetailsDto>> GetOneAsync(int certificateId)
        {
            try
            {
                var certificate = await _dbContext.Certificates
                    .Include(c => c.User)
                    .Include(c => c.Course)
                    .Include(c => c.Path)
                    .Where(c => c.Id == certificateId && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                if (certificate == null)
                {
                    _logger.LogInformation("No certificates found.");
                    return new GeneralResult<CertificateDetailsDto>(true, _localizationManager.GetLocalizedString("NoCertificatesFound"), null);
                }

                var certificateDto = new CertificateDetailsDto
                {
                    Id = certificate.Id,
                    CertificateFor = certificate.CertificateType == Enums.StudentCertificateType.Path ? certificate.Path?.Name : certificate.Course?.Name,
                    IssuedAt = certificate.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = certificate.CertificateId,
                    ExpirationDate = certificate.ExpirationDate ?? DateTimeOffset.MinValue,
                    ShippingStatus = certificate.ShippingStatus,
                    ShippingAddress = certificate.ShippingAddress,
                    Student = certificate.User != null ? new StudentData
                    {
                        FirstName = certificate.User.FirstName,
                        LastName = certificate.User.LastName,
                        Email = certificate.User.Email,
                        NationalId = certificate.User.NationalId ?? 0,
                        PhoneNumber = certificate.User.PhoneNumber,
                        Specialization = certificate.User.Specialization
                    }
                    : null
                };

                _logger.LogInformation("Retrieved certificate with ID {CertificateId}.", certificateId);
                return new GeneralResult<CertificateDetailsDto>(true, _localizationManager.GetLocalizedString("CertificatesRetrieved"), certificateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving certificate with ID {CertificateId}.", certificateId);
                return new GeneralResult<CertificateDetailsDto>(false, _localizationManager.GetLocalizedString("CertificatesRetrievalFailed"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CertificateDetailsDto>>> GetUserCertificatesAsync(string userId)
        {
            try
            {
                var certificates = await _dbContext.Certificates
                    .Include(c => c.User)
                    .Include(c => c.Course)
                    .Include(c => c.Path)
                    .Where(c => c.UserId == userId && !c.IsDeleted)
                    .ToListAsync();

                if (!certificates.Any())
                {
                    _logger.LogInformation("No certificates found for User ID {UserId}.", userId);
                    return new GeneralResult<List<CertificateDetailsDto>>(true, _localizationManager.GetLocalizedString("NoCertificatesFound"), null);
                }

                var certificateDtos = certificates.Select(c => new CertificateDetailsDto
                {
                    Id = c.Id,
                    CertificateFor = c.CertificateType == Enums.StudentCertificateType.Path ? c.Path?.Name : c.Course?.Name,
                    IssuedAt = c.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = c.CertificateId,
                    ExpirationDate = c.ExpirationDate ?? DateTimeOffset.MinValue,
                    ShippingStatus = c.ShippingStatus,
                    ShippingAddress = c.ShippingAddress,
                    Student = null
                }).ToList();

                _logger.LogInformation("Retrieved {Count} certificates for User ID {UserId}.", certificateDtos.Count, userId);
                return new GeneralResult<List<CertificateDetailsDto>>(true, _localizationManager.GetLocalizedString("CertificatesRetrieved"), certificateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving certificates for User ID {UserId}.", userId);
                return new GeneralResult<List<CertificateDetailsDto>>(false, _localizationManager.GetLocalizedString("CertificatesRetrievalFailed"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ShippingCertificateAsync(int id, string shippingAddress, string userId)
        {
            try
            {
                if (shippingAddress == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("ShippingAddressRequired"));
                }

                // check if user exists
                var user = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                var certificate = await _dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && !c.IsDeleted);
                if (certificate == null)
                {
                    _logger.LogWarning("Certificate with ID {CertificateId} not found.", id);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateNotFound"));
                }

                var isBuyForShpping = await _dbContext.Purchases
                    .Where(cr => cr.UserId == userId && cr.ItemType == PurchaseItemType.CertificateShipping && !cr.IsRefunded).AnyAsync();
                if (!isBuyForShpping)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateShippingNotPurchased"));
                }

                // Update only delivery-related fields
                certificate.DeliveryMethod = DeliveryMethod.Shipping;
                certificate.ShippingAddress = shippingAddress;
                certificate.ShippingStatus = ShippingStatus.Pending;
                certificate.UpdatedAt = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Shipping data updated successfully for certificate ID {CertificateId}.", id);
                return new GeneralResult(true, _localizationManager.GetLocalizedString("CertificateUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shipping data for certificate ID {CertificateId}.", id);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CertificateDetailsDto>>> GetAllShippingCertificatesAsync()
        {
            try
            {
                var certificates = await _dbContext.Certificates
                    .Include(c => c.User)
                    .Include(c => c.Course)
                    .Include(c => c.Path)
                    .Where(c =>
                        c.DeliveryMethod == DeliveryMethod.Shipping &&
                        c.ShippingStatus != ShippingStatus.NotRequired &&
                        !c.IsDeleted)
                    .ToListAsync();

                if (!certificates.Any())
                {
                    _logger.LogInformation("No certificates with shipping requested found.");
                    return new GeneralResult<List<CertificateDetailsDto>>(
                        true,
                        _localizationManager.GetLocalizedString("NoShippingCertificatesFound"),
                        new List<CertificateDetailsDto>());
                }

                var result = certificates.Select(c => new CertificateDetailsDto
                {
                    Id = c.Id,
                    CertificateFor = c.CertificateType == Enums.StudentCertificateType.Path ? c.Path?.Name : c.Course?.Name,
                    IssuedAt = c.IssuedAt ?? DateTimeOffset.UtcNow,
                    CertificateId = c.CertificateId,
                    ExpirationDate = c.ExpirationDate ?? DateTimeOffset.MinValue,
                    ShippingStatus = c.ShippingStatus,
                    ShippingAddress = c.ShippingAddress,
                    Student = c.User != null ? new StudentData
                    {
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        Email = c.User.Email,
                        NationalId = c.User.NationalId ?? 0,
                        PhoneNumber = c.User.PhoneNumber,
                        Specialization = c.User.Specialization
                    }
                    : null
                }).ToList();

                return new GeneralResult<List<CertificateDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("ShippingCertificatesRetrieved"),
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving certificates with shipping requested.");
                return new GeneralResult<List<CertificateDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("ShippingCertificateRetrievalFailed"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteAsync(int id)
        {
            try
            {
                var certificate = await _dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (certificate == null)
                {
                    _logger.LogWarning("Certificate with ID {CertificateId} not found.", id);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateNotFound"));
                }

                certificate.IsDeleted = true;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Certificate with ID {CertificateId} deleted successfully.", id);
                return new GeneralResult(true, _localizationManager.GetLocalizedString("CertificateDeleted"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting certificate with ID {CertificateId}.", id);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateDeletionFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateShippingStatusAsync(int certificateId, ShippingStatus newStatus)
        {
            try
            {
                if (!Enum.IsDefined(typeof(ShippingStatus), newStatus))
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Invalid shipping status value."
                    };
                }

                // check if certificate exists
                var certificate = await _dbContext.Certificates
                    .FirstOrDefaultAsync(c => c.Id == certificateId && !c.IsDeleted && c.DeliveryMethod == DeliveryMethod.Shipping);
                if (certificate == null)
                {
                    _logger.LogWarning("Certificate with ID {CertificateId} not found.", certificateId);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("CertificateNotFound"));
                }

                // check if delivery method is shipping
                if (certificate.DeliveryMethod != DeliveryMethod.Shipping)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("ShippingNotApplicable"));
                }

                // check if shipping status is not already updated
                if (!certificate.ShippingStatus.Equals(newStatus))
                {
                    certificate.ShippingStatus = newStatus;
                    certificate.UpdatedAt = DateTimeOffset.UtcNow;
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation("Updated shipping status for certificate ID {CertificateId} to {Status}", certificateId, newStatus);
                }

                return new GeneralResult(true, _localizationManager.GetLocalizedString("ShippingStatusUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shipping status for certificate ID {CertificateId}", certificateId);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ShippingStatusUpdateFailed"));
            }
        }
    }
}
