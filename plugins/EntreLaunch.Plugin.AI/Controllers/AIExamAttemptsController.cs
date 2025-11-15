//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Text.Json;
//using EntreLaunch.Data;
//using EntreLaunch.Entities;
//using EntreLaunch.Plugin.AI.DTOs;
//using EntreLaunch.Plugin.AI.Service;

//namespace EntreLaunch.Plugin.AI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AIExamAttemptsController : ControllerBase
//    {
//        private readonly PgDbContext _context;
//        private readonly DifyApiClient _difyClient;

//        public AIExamAttemptsController(PgDbContext context, DifyApiClient difyClient)
//        {
//            _context = context;
//            _difyClient = difyClient;
//        }

//        /// <summary>
//        /// Submit an exam attempt.
//        /// </summary>
//        [HttpPost("submit")]
//        public async Task<IActionResult> SubmitExamAttempt([FromBody] ExamAttemptRequest request)
//        {
//            var exam = await _context.Exams!.Include(e => e.Questions)
//                .FirstOrDefaultAsync(e => e.Id == request.ExamId);

//            if (exam == null)
//            {
//                return NotFound("Exam not found.");
//            }

//            decimal totalScore = 0;
//            decimal maxScore = 0;

//            // Repetition across each answer provided
//            foreach (var ans in request.Answers)
//            {
//                // Find the corresponding question in the exam
//                var question = exam.Questions?.FirstOrDefault(q => q.Id == ans.QuestionId);
//                if (question == null)
//                {
//                    // If the question doesn't exist, we skip it
//                    continue;
//                }

//                // Call dify.ai to correct the answer
//                string correctionResult = await _difyClient.CorrectAnswerAsync(question.Text!, ans.AnswerText);

//                // Attempting to extract the score from the returned result
//                decimal questionScore = 0;
//                try
//                {
//                    using var doc = JsonDocument.Parse(correctionResult);
//                    if (doc.RootElement.TryGetProperty("score", out var scoreProp))
//                    {
//                        questionScore = scoreProp.GetDecimal();
//                    }
//                }
//                catch (Exception)
//                {
//                    // The error can be logged; we consider the score to be zero if it fails to be extracted
//                    questionScore = 0;
//                }

//                totalScore += questionScore;
//                // We assume that each question has a maximum score saved in the Mark property; if it is not available, we consider it to be 1
//                maxScore += question.Mark ?? 1;
//            }

//            // Create an exam result record
//            var examResult = new ExamResult
//            {
//                ExamId = exam.Id,
//                UserId = request.UserId,
//                Mark = totalScore,
//                Status = (totalScore >= (exam.MinMark ?? 0)) ? "Passed" : "Failed",
//                AttemptNumber = 1,  // This can be adjusted based on the number of previous attempts
//                IsActive = true
//            };

//            _context.ExamResults!.Add(examResult);
//            await _context.SaveChangesAsync();

//            // Return the final result with the overall score and maximum points
//            return Ok(new
//            {
//                TotalScore = totalScore,
//                MaxScore = maxScore,
//                Status = examResult.Status
//            });
//        }
//    }
//}
