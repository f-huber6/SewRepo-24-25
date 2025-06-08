using AutoMapper;
using University.StudentService.API.Data;
using University.StudentService.API.Dto;
using University.StudentService.API.Entities;

namespace University.StudentService.API.Controllers;

public class StudentController: CrudController<Student,StudentDto, StudentCreateDto, StudentUpdateDto>
{
    public StudentController(StudentContext ctx, IMapper mapper) : base(ctx, mapper)
    {
    }
}