using AutoMapper;
using University.InstructorService.API.Data;
using University.InstructorService.API.Dto;
using University.InstructorService.API.Entities;

namespace University.InstructorService.API.Controllers;

public class InstructorController
    : CrudController<Instructor, InstructorDto, InstructorCreateDto, InstructorUpdateDto>
{
    public InstructorController(InstructorContext ctx, IMapper mapper)
        : base(ctx, mapper) { }
}