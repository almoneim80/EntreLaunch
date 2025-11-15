using Microsoft.AspNetCore.Mvc;
using EntreLaunch.Plugin.AI.DTOs;
using EntreLaunch.Plugin.AI.Service;

namespace EntreLaunch.Plugin.AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIAnswersController : ControllerBase
    {
        private readonly DifyApiClient _difyClient;
        public AIAnswersController()
        {
            _difyClient = new DifyApiClient("app-oSv7mGba5IfBF1VlfxYsSMrH");
        }

        /// <summary>
        /// Correct answer using dify.ai.
        /// </summary>
        [HttpPost("correct")]
        public async Task<IActionResult> CorrectAnswer([FromBody] CorrectionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.QuestionText) || string.IsNullOrWhiteSpace(request.UserAnswer))
            {
                return BadRequest("Both QuestionText and UserAnswer must be provided.");
            }

            try
            {
                // استدعاء خدمة تصحيح الإجابة عبر dify.ai
                string result = await _difyClient.CorrectAnswerAsync(request.QuestionText, request.UserAnswer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error correcting answer: {ex.Message}");
            }
        }
    }
}
