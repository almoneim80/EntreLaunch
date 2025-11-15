namespace EntreLaunch.Plugin.SmartExam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public SmartExamController(IExamService examService)
        {
            _examService = examService;
        }

        /// <summary>
        /// generate exam using ai.
        /// </summary>
        [HttpPost("exam/generate-exam-by-ai")]
        public async Task<IActionResult> GenerateExamByAI([FromBody] AIRequestDataDto aIRequestDataDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Error");
            }

            var questions = await _examService.GenerateQuestionsAsync(aIRequestDataDto);
            return Ok(questions);
        }

        /// <summary>
        /// EntreLaunchload a file and generate exam from its content.
        /// </summary>
        [HttpPost("exam/generate-exam-from-documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EntreLaunchloadFilesToAI(List<IFormFile> files, [FromForm] GenerateQuestionsRequestDto requestDto)
        {
            if (files == null || files.Count == 0)
                return BadRequest("❌ Error: No files EntreLaunchloaded.");

            if(files.Count > 5)
            {
                return BadRequest("❌ Error: You can EntreLaunchload a maximum of 5 files.");
            }

            try
            {
                var fileIds = await _examService.EntreLaunchloadFilesToDify(files, "123");

                var data = new AIFileRequestDataDto
                {
                    fileIds = fileIds,
                    QuestionsNumber = requestDto.QuestionsNumber,
                    MinMark = requestDto.MinMark,
                    MaxMark = requestDto.MaxMark,
                    Language = requestDto.Language
                };

                if (data.Equals(null))
                {
                    return BadRequest("❌ Error: You can EntreLaunchload a maximum of 5 files.");
                }

                var questions = await _examService.GenerateQuestionsFromFilesAsync(data);

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest($"❌ Error: {ex.Message}");
            }
        }

        ///// <summary>
        ///// استلام إجابات الطالب وتصحيحها.
        ///// </summary>
        //[HttpPost("{examId}/submit")]
        //public async Task<IActionResult> SubmitAnswers(int examId, [FromBody] List<QuestionAnswerDto> answers)
        //{
        //    if (answers == null || answers.Count == 0)
        //        return BadRequest("No answers provided.");

        //    try
        //    {
        //        var gradeResult = await _examService.GradeExamAsync(examId, answers);
        //        return Ok(gradeResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        // يجب تسجيل الخطأ Log والتعامل معه
        //        return BadRequest($"Error grading exam: {ex.Message}");
        //    }
        //}
    }
}
