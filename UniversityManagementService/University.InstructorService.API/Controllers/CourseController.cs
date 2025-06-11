using AutoMapper;
using University.InstructorService.API.Data;
using University.InstructorService.API.Entities;
using University.Shared.DtoCourse;

namespace University.InstructorService.API.Controllers;

public class CourseController: CrudController<Course, CourseDto, CourseCreateDto, CourseUpdateDto>
{
    public CourseController(InstructorContext ctx, IMapper mapper) : base(ctx, mapper)
    {
    }
}