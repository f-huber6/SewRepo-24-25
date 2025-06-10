using AutoMapper;
using BlazorApp1.Entities;
using University.TestsService.API.Dto;

namespace BlazorApp1.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Test,TestsDto>();
        CreateMap<TestsCreateDto, Test>();
        CreateMap<TestsUpdateDto, Test>();
    }
}

