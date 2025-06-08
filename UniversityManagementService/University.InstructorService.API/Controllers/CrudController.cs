using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.InstructorService.API.Data;

namespace University.InstructorService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class CrudController<TEntity, TDto, TCreate, TUpdate> : ControllerBase
    where TEntity : class
    where TDto    : class
{
    private readonly InstructorContext _ctx;
    private readonly IMapper _mapper;
    private readonly DbSet<TEntity> _db;

    public CrudController(InstructorContext ctx, IMapper mapper)
    {
        _ctx    = ctx;
        _mapper = mapper;
        _db     = ctx.Set<TEntity>();
    }

    [HttpGet("all")]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        var list = await _db.AsNoTracking().ToListAsync();
        return list.Select(e => _mapper.Map<TDto>(e)).ToList();
    }

    [HttpGet("read/{id}")]
    public virtual async Task<ActionResult<TDto>> Get(string id)
    {
        var entity = await _db.FindAsync(id);
        if (entity == null) return NotFound();
        return _mapper.Map<TDto>(entity);
    }

    [HttpPost("create")]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreate dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        _db.Add(entity);
        await _ctx.SaveChangesAsync();
        var result = _mapper.Map<TDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = result.GetType().GetProperty("Id")!.GetValue(result) }, result);
    }

    [HttpPut("update/{id}")]
    public virtual async Task<ActionResult> Update(string id, [FromBody] TUpdate dto)
    {
        var entity = await _db.FindAsync(id);
        if (entity == null) return NotFound();
        _mapper.Map(dto, entity);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    public virtual async Task<ActionResult> Delete(string id)
    {
        var entity = await _db.FindAsync(id);
        if (entity == null) return NotFound();
        _db.Remove(entity);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}