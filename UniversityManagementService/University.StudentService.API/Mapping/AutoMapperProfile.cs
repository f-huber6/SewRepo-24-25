using AutoMapper;
using University.StudentService.API.Dto;
using University.StudentService.API.Entities;

namespace University.StudentService.API.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<StudentCreateDto, Student>();
        CreateMap<StudentUpdateDto, Student>();
    }
}