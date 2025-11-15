namespace EntreLaunch.Plugin.SmartExam.Interfaces
{
    public interface IExamService
    {
        Task<string> GenerateQuestionsAsync(AIRequestDataDto aIRequestDataDto);
        Task<string> GenerateQuestionsFromFilesAsync(AIFileRequestDataDto aIFileRequestDataDto);
        Task<List<string>> EntreLaunchloadFilesToDify(List<IFormFile> files, string userId);
        Task<ExamGradeResultDto> GradeExamAsync(int examId, List<QuestionAnswerDto> userAnswers);
    }
}
