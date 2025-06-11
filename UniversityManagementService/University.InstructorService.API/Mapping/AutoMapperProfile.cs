using AutoMapper;
using University.InstructorService.API.Dto;
using University.InstructorService.API.Entities;
using University.Shared.DtoCourse;
using University.Shared.DtoLecture;

namespace University.InstructorService.API.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Instructor, InstructorDto>();
        CreateMap<InstructorCreateDto, Instructor>();
        CreateMap<InstructorUpdateDto, Instructor>();
        
        CreateMap<Course, CourseDto>();
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CourseUpdateDto, Course>();

        CreateMap<Lecture, LectureDto>();
        CreateMap<LectureCreateDto, Lecture>();
        CreateMap<LectureUpdateDto, Lecture>();
    }
}