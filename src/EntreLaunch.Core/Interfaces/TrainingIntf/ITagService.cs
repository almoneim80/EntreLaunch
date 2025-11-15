namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ITagService
    {
        /// <summary>
        /// Add tag.
        /// </summary>
        Task<GeneralResult> AddTagAsync(string tagName);

        /// <summary>
        /// Get all tags.
        /// </summary>
        Task<GeneralResult<List<Tag>>> GetAllTagsAsync();

        /// <summary>
        /// Delete tag.
        /// </summary>
        Task<GeneralResult> DeleteTagAsync(int tagId);

        /// <summary>
        /// EntreLaunchdate tag.
        /// </summary>
        Task<GeneralResult> EntreLaunchdateTagAsync(int tagId, string newTagName);

        /// <summary>
        /// Get tag by id.
        /// </summary>
        Task<GeneralResult<Tag>> GetTagByIdAsync(int tagId);

        // CourseTags Table

        /// <summary>
        /// assign tags to course.
        /// </summary>
        Task<GeneralResult> AssignTagsToCourseAsync(int courseId, List<int> tagIds);

        /// <summary>
        /// Get tags for course.
        /// </summary>
        Task<GeneralResult<List<string>>> GetTagsForCourseAsync(int courseId);

        /// <summary>
        /// Get courses by tag.
        /// </summary>
        Task<GeneralResult<List<string>>> GetCoursesByTagAsync(int tagId);

        /// <summary>
        /// Remove tags from course.
        /// </summary>
        Task<GeneralResult> RemoveTagsFromCourseAsync(int courseId, List<int> tagIds);
    }
}
