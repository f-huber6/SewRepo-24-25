using BlazorApp1.Data;
using BlazorApp1.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.TestsService.API.Dto;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TestsContext _context;

    public TestController(TestsContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestsDto.TestDto>>> GetAll()
    {
        var tests = await _context.Tests
            .Include(t => t.Student)
            .Select(t => new TestsDto.TestDto
            {
                Id = t.Id,
                Titel = t.Titel,
                Datum = t.Datum,
                StudentId = t.StudentId,
                StudentFullName = t.Student.FirstName + " " + t.Student.LastName
            })
            .ToListAsync();

        return Ok(tests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestsDto.TestDto>> GetById(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Student)
            .Where(t => t.Id == id)
            .Select(t => new TestsDto.TestDto
            {
                Id = t.Id,
                Titel = t.Titel,
                Datum = t.Datum,
                StudentId = t.StudentId,
                StudentFullName = t.Student.FirstName + " " + t.Student.LastName
            })
            .FirstOrDefaultAsync();

        if (test == null)
            return NotFound();

        return Ok(test);
    }
    
    [HttpPost]
    public async Task<ActionResult<TestsDto.TestDto>> Create(TestsCreateDto dto)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.Id == dto.StudentId);
        if (!studentExists)
            return BadRequest("StudentId ungültig.");

        var test = new Test
        {
            Titel = dto.Titel,
            Datum = dto.Datum,
            StudentId = dto.StudentId
        };

        _context.Tests.Add(test);
        await _context.SaveChangesAsync();

        var resultDto = new TestsDto.TestDto
        {
            Id = test.Id,
            Titel = test.Titel,
            Datum = test.Datum,
            StudentId = test.StudentId,
            StudentFullName = "" // optional: Student abrufen
        };

        return CreatedAtAction(nameof(GetById), new { id = test.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TestsUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID stimmt nicht überein.");

        var test = await _context.Tests.FindAsync(id);
        if (test == null)
            return NotFound();

        var studentExists = await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test == null)
            return NotFound();

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
