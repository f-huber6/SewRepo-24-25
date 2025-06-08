using AutoMapper;
using University.InstructorService.API.Dto;
using University.InstructorService.API.Entities;

namespace University.InstructorService.API.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Instructor, InstructorDto>();
        CreateMap<InstructorCreateDto, Instructor>();
        CreateMap<InstructorUpdateDto, Instructor>();
    }
}