//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using EntreLaunch.Data;
//using EntreLaunch.DTOs;
//using EntreLaunch.Entities;
//using EntreLaunch.Plugin.AI.Service;

//namespace EntreLaunch.Plugin.AI.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class AIExamsController : ControllerBase
//{
//    private readonly DifyApiClient _difyClient;
//    private readonly string _apiKey = "app-oSv7mGba5IfBF1VlfxYsSMrH";
//    private readonly PgDbContext _context;
//    private readonly IMapper _mapper;

//    public AIExamsController(PgDbContext context, IMapper mapper)
//    {
//        _difyClient = new DifyApiClient(_apiKey);
//        _context = context;
//        _mapper = mapper;
//    }

//    /// <summary>
//    /// Get all exams.
//    /// </summary>
//    [HttpGet]
//    public async Task<ActionResult<IEnumerable<Exam>>> GetExams()
//    {
//        return await _context.Exams!.Include(e => e.Questions).ToListAsync();
//    }

//    /// <summary>
//    /// Get an exam by id.
//    /// </summary>
//    [HttpGet("{id}")]
//    public async Task<ActionResult<Exam>> GetExam(int id)
//    {
//        var exam = await _context.Exams!.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == id);

//        if (exam == null)
//        {
//            return NotFound();
//        }

//        return exam;
//    }

//    /// <summary>
//    /// Create an exam.
//    /// </summary>
//    [HttpPost]
//    public async Task<ActionResult<Exam>> CreateExam(ExamCreateDto exam)
//    {
//        var newExam = _mapper.Map<Exam>(exam);
//        _context.Exams!.Add(newExam);
//        await _context.SaveChangesAsync();

//        return Ok();
//    }

//    /// <summary>
//    /// EntreLaunchdate an exam.
//    /// </summary>
//    [HttpPut("{id}")]
//    public async Task<IActionResult> EntreLaunchdateExam(int id, Exam exam)
//    {
//        if (id != exam.Id)
//        {
//            return BadRequest();
//        }

//        _context.Entry(exam).State = EntityState.Modified;

//        try
//        {
//            await _context.SaveChangesAsync();
//        }
//        catch (DbEntreLaunchdateConcurrencyException)
//        {
//            if (!ExamExists(id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }

//        return NoContent();
//    }

//    /// <summary>
//    /// Delete an exam.
//    /// </summary>
//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteExam(int id)
//    {
//        var exam = await _context.Exams!.FindAsync(id);
//        if (exam == null)
//        {
//            return NotFound();
//        }

//        _context.Exams.Remove(exam);
//        await _context.SaveChangesAsync();

//        return NoContent();
//    }

//    /// <summary>
//    /// Check if an exam exists.
//    /// </summary>
//    private bool ExamExists(int id)
//    {
//        return _context.Exams!.Any(e => e.Id == id);
//    }
//}
