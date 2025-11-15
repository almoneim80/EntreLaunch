namespace EntreLaunch.Services.TrainingSvc
{
    public class TagService : ITagService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<TagService> _logger;
        public TagService(PgDbContext dbContext, ILogger<TagService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

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
                        Message = "Tag already exists."
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
                    Message = "Tag added successfully.",
                    Data = newTag,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new tag.");
                return new GeneralResult(false, "Error adding a new tag.", null);
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
                    return new GeneralResult<Tag>(false, "Tag not found.", null);
                }

                return new GeneralResult<Tag>(true, "Tag found.", tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag with ID {TagId}.", tagId);
                return new GeneralResult<Tag>(false, "Error retrieving tag.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateTagAsync(int tagId, string newTagName)
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
                        Message = "Tag not found."
                    };
                }

                // Checking for dEntreLaunchlicate name
                var existingTag = await _dbContext.Tags
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == newTagName.ToLower() && t.Id != tagId && !t.IsDeleted);

                if (existingTag != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "A tag with the same name already exists."
                    };
                }

                tag.Name = newTagName;
                tag.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Tags.EntreLaunchdate(tag);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Tag EntreLaunchdated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating tag with ID {TagId}.", tagId);
                return new GeneralResult(false, "Error EntreLaunchdating tag.", null);
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
                        Message = "Tag not found."
                    };
                }

                tag.IsDeleted = true;
                tag.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.Tags.EntreLaunchdate(tag);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Tag deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag with ID {TagId}.", tagId);
                return new GeneralResult(false, "Error deleting tag.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<Tag>>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _dbContext.Tags.Where(t => !t.IsDeleted).ToListAsync();
                if (!tags.Any())
                {
                    return new GeneralResult<List<Tag>>(false, "No tags found.", null);
                }

                return new GeneralResult<List<Tag>>(true, "Tags found.", tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tags.");
                return new GeneralResult<List<Tag>>(false, "Error retrieving tags.", null);
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
                        Message = "Course not found or is deleted."
                    };
                }

                // Checking Tags
                var validTags = await _dbContext.Tags.Where(t => tagIds.Contains(t.Id) && !t.IsDeleted).ToListAsync();
                if (validTags.Count != tagIds.Count)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "One or more tags are invalid or deleted."
                    };
                }

                // Remove any existing links to the same course (to avoid dEntreLaunchlication)
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
                    Message = "Tags assigned to course successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tags to course.");
                return new GeneralResult(false, "Error assigning tags to course.", null);
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
                    return new GeneralResult<List<string>>(false, "No tags found.", null);
                }

                return new GeneralResult<List<string>>(true, "Tags found.", tagNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for course ID {CourseId}.", courseId);
                return new GeneralResult<List<string>>(false, "Error retrieving tags.", null);
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
                    return new GeneralResult<List<string>>(false, "No courses found.", null);
                }

                return new GeneralResult<List<string>>(true, "Courses found.", courseNames!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for tag ID {TagId}.", tagId);
                return new GeneralResult<List<string>>(false, "Error retrieving courses.", null);
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
                        Message = "No matching tags found for this course."
                    };
                }

                _dbContext.CourseTags.RemoveRange(courseTags);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Tags removed from course successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tags from course.");
                return new GeneralResult(false, "Error removing tags from course.", null);
            }
        }
    }
}
