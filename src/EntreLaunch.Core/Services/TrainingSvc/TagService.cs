using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class TagService(PgDbContext dbContext, ILogger<TagService> logger, ILocalizationManager localizationManager) : ITagService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<TagService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        // Tag Table

        /// <inheritdoc />
        public async Task<GeneralResult> AddTagAsync(string tagName)
        {
            try
            {
                // Checking for pre-existing tags
                var existingTag = await _dbContext.Tags.AnyAsync(t => t.Name.ToLower() == tagName.ToLower() && !t.IsDeleted);

                if (existingTag)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TagAlreadyExists")
                    };
                }

                // Adding the new tag
                var newTag = new Tag
                {
                    Name = tagName,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                };
                await _dbContext.Tags.AddAsync(newTag);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TagAdded"),
                    Data = newTag,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new tag.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("TagAddError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<Tag>> GetTagByIdAsync(int tagId)
        {
            try
            {
                var tag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Id == tagId && !t.IsDeleted);
                if(tag == null)
                {
                    return new GeneralResult<Tag>(false, _localizationManager.GetLocalizedString("TagNotFound"), null);
                }

                return new GeneralResult<Tag>(true, _localizationManager.GetLocalizedString("TagFound"), tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag with ID {TagId}.", tagId);
                return new GeneralResult<Tag>(false, _localizationManager.GetLocalizedString("TagRetrieveError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateTagAsync(int tagId, string newTagName)
        {
            try
            {
                var tag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Id == tagId && !t.IsDeleted);

                if (tag == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TagNotFound")
                    };
                }

                // Checking for duplicate name
                var existingTag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == newTagName.ToLower() && t.Id != tagId && !t.IsDeleted);

                if (existingTag != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TagNameExists")
                    };
                }

                tag.Name = newTagName;
                tag.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Tags.Update(tag);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TagUpdated")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag with ID {TagId}.", tagId);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("TagUpdateError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteTagAsync(int tagId)
        {
            try
            {
                var tag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Id == tagId && !t.IsDeleted);

                if (tag == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TagNotFound")
                    };
                }

                tag.IsDeleted = true;
                tag.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.Tags.Update(tag);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TagDeleted")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag with ID {TagId}.", tagId);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("TagDeleteError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<Tag>>> GetAllTagsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Tags
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted);

                var totalCount = await query.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    _logger.LogInformation("No tags found.");
                    return new GeneralResult<PaginatedResult<Tag>>(false, _localizationManager.GetLocalizedString("NoTagsFound"), null);
                }

                var pagedTags = await query
                    .OrderBy(t => t.Name)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                var result = new PaginatedResult<Tag>
                {
                    Items = pagedTags,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                };

                return new GeneralResult<PaginatedResult<Tag>>(true, _localizationManager.GetLocalizedString("TagsFound"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tags.");
                return new GeneralResult<PaginatedResult<Tag>>(false, _localizationManager.GetLocalizedString("TagsRetrieveError"), null);
            }
        }

        // CourseTag Table

        /// <inheritdoc />
        public async Task<GeneralResult> AssignTagsToCourseAsync(int courseId, List<int> tagIds)
        {
            try
            {
                // Checking the existence of the Course.
                var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted);
                if (!courseExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CourseNotFoundOrDeleted")
                    };
                }

                // Checking Tags
                var validTags = await _dbContext.Tags.Where(t => tagIds.Contains(t.Id) && !t.IsDeleted).ToListAsync();
                if (validTags.Count != tagIds.Count)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("InvalidTags")
                    };
                }

                // Remove any existing links to the same course (to avoid duplication)
                var existingCourseTags = await _dbContext.CourseTags.Where(ct => ct.CourseId == courseId).ToListAsync();
                _dbContext.CourseTags.RemoveRange(existingCourseTags);

                // Linking tags to the course
                var newCourseTags = validTags.Select(tag => new CourseTag
                {
                    CourseId = courseId,
                    TagId = tag.Id,
                    IsDeleted = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                }).ToList();

                await _dbContext.CourseTags.AddRangeAsync(newCourseTags);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TagsAssignedToCourse")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tags to course.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("TagsAssignError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetTagsForCourseAsync(int courseId)
        {
            try
            {
                var tagNames = await _dbContext.CourseTags
                    .Where(ct => ct.CourseId == courseId && !ct.Tag!.IsDeleted)
                    .Select(ct => ct.Tag!.Name)
                    .ToListAsync();

                if (!tagNames.Any())
                {
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoTagsForCourse"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("TagsForCourseFound"), tagNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for course ID {CourseId}.", courseId);
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("TagsForCourseError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetCoursesByTagAsync(int tagId)
        {
            try
            {
                var courseNames = await _dbContext.CourseTags
                    .Where(ct => ct.TagId == tagId && !ct.Course!.IsDeleted)
                    .Select(ct => ct.Course!.Name).ToListAsync();

                if (!courseNames.Any())
                {
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoCoursesFound"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("CoursesFound"), courseNames!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for tag ID {TagId}.", tagId);
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("CoursesRetrieveError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> RemoveTagsFromCourseAsync(int courseId, List<int> tagIds)
        {
            try
            {
                var courseTags = await _dbContext.CourseTags.Where(ct => ct.CourseId == courseId && tagIds.Contains(ct.TagId))
                    .ToListAsync();

                if (!courseTags.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoMatchingTagsFound")
                    };
                }

                _dbContext.CourseTags.RemoveRange(courseTags);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TagsRemoved")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tags from course.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("TagsRemoveError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetCoursesByTagNameAsync(string tagName)
        {
            try
            {
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                    return new GeneralResult<List<string>>(false, "Tag not found", null);

                var courses = await _dbContext.CourseTags
                    .Where(ct => ct.TagId == tag.Id && ct.Course != null)
                    .Select(ct => ct.Course!.Name)
                    .ToListAsync();

                if (!courses.Any())
                    return new GeneralResult<List<string>>(false, "No courses found", null);

                return new GeneralResult<List<string>>(true, "Courses retrieved successfully", courses!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for tag name {TagName}.", tagName);
                return new GeneralResult<List<string>>(false, "Error retrieving courses", null);
            }
        }
    }
}
