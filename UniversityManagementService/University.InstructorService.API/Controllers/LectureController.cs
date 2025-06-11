using AutoMapper;
using University.InstructorService.API.Data;
using University.InstructorService.API.Entities;
using University.Shared.DtoLecture;

namespace University.InstructorService.API.Controllers;

public class LectureController: CrudController<Lecture, LectureDto, LectureCreateDto, LectureUpdateDto>
{
    public LectureController(InstructorContext ctx, IMapper mapper) : base(ctx, mapper)
    {
    }
}