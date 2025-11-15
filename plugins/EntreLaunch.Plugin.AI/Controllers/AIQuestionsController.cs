//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using EntreLaunch.Entities;
//using EntreLaunch.Data;
//using EntreLaunch.DTOs;
//using AutoMapper;

//namespace EntreLaunch.Plugin.AI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AIQuestionsController : ControllerBase
//    {
//        private readonly PgDbContext _context;
//        private readonly IMapper _mapper;

//        public AIQuestionsController(PgDbContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        // GET: api/Questions/exam/5
//        [HttpGet("exam/{examId}")]
//        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByExam(int examId)
//        {
//            return await _context.Questions!
//                .Where(q => q.ExamId == examId)
//                .ToListAsync();
//        }

//        // GET: api/Questions/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Question>> GetQuestion(int id)
//        {
//            var question = await _context.Questions!.FindAsync(id);
//            if (question == null)
//            {
//                return NotFound();
//            }
//            return question;
//        }

//        // POST: api/Questions
//        [HttpPost]
//        public async Task<ActionResult<Question>> CreateQuestion(QuestionCreateDto question)
//        {
//            var questionEntity = _mapper.Map<Question>(question);
//            _context.Questions!.Add(questionEntity);
//            await _context.SaveChangesAsync();
//            return Ok(question);
//        }

//        // PUT: api/Questions/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> EntreLaunchdateQuestion(int id, QuestionEntreLaunchdateDto question)
//        {
//            var questionEntity = _mapper.Map<Question>(question);
//            if (id != questionEntity.Id)
//            {
//                return BadRequest();
//            }


//            _context.Entry(question).State = EntityState.Modified;
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbEntreLaunchdateConcurrencyException)
//            {
//                if (!_context.Questions!.Any(q => q.Id == id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            return NoContent();
//        }

//        // DELETE: api/Questions/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteQuestion(int id)
//        {
//            var question = await _context.Questions!.FindAsync(id);
//            if (question == null)
//            {
//                return NotFound();
//            }
//            _context.Questions.Remove(question);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}
