using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.EnrollmentService.API.Data;
using University.EnrollmentService.API.Entities;
using University.Shared.Client;
using University.Shared.DtoEnrollment;
using University.Shared.DtoLecture;
using University.StudentService.API.Dto;

namespace University.EnrollmentService.API.Controller;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentContext _ctx;
    private readonly IStudentClient    _students;
    private readonly ILectureClient     _lectures;

    public EnrollmentController(
        EnrollmentContext ctx,
        IStudentClient students,
        ILectureClient lectures)
    {
        _ctx      = ctx;
        _students = students;
        _lectures  = lectures;
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetAll()
    {
        var list = await _ctx.Enrollments
            .Select(e => new EnrollmentDto {
                StudentId      = e.StudentId,
                LectureId      = e.LectureId,
                EnrollmentDate = e.EnrollmentDate
            })
            .ToListAsync();
        return Ok(list);
    }
    
    [HttpPost("assign")]
    public async Task<IActionResult> Assign(
        [FromQuery] string studentId,
        [FromQuery] string lectureId)
    {
        _ctx.Enrollments.Add(new Enrollment { StudentId = studentId, LectureId = lectureId });
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // GET api/enrollments/student/{studentId}
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<LectureDto>>> GetLectures(string studentId)
    {
        var allLectures = await _lectures.GetAllAsync();
        var lecturesIds = await _ctx.Enrollments
                            .Where(e => e.StudentId == studentId)
                            .Select(e => e.LectureId)
                            .ToListAsync();
        return Ok(allLectures.Where(c => lecturesIds.Contains(c.Id)).ToList());
    }

    // GET api/enrollments/lectures/{lectureId}
    [HttpGet("lecture/{lectureId}")]
    public async Task<ActionResult<List<StudentDto>>> GetStudents(string lectureId)
    {
        var allStudents = await _students.GetAllAsync();
        List<string> studentIds  = await _ctx.Enrollments
                            .Where(e => e.LectureId == lectureId)
                            .Select(e => e.StudentId)
                            .ToListAsync();
        return Ok(allStudents.Where(s => studentIds.Contains(s.Id)).ToList());
    }
    
    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(
        [FromQuery] string studentId,
        [FromQuery] string lectureId)
    {
        var e = await _ctx.Enrollments
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.LectureId == lectureId);
        if (e == null) return NotFound();
        _ctx.Enrollments.Remove(e);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}