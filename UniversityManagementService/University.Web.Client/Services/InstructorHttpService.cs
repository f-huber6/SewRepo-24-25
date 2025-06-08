using System.Net.Http.Json;
using University.InstructorService.API.Dto;

namespace University.Client.Services;

public class InstructorHttpService
{
    private readonly HttpClient _http;

    public InstructorHttpService(HttpClient http) => _http = http;

    public Task<List<InstructorDto>> GetAllAsync()    
        => _http.GetFromJsonAsync<List<InstructorDto>>("api/instructor/all")!;

    public Task<InstructorDto> GetByIdAsync(string id) 
        => _http.GetFromJsonAsync<InstructorDto>($"api/instructor/read/{id}")!;

    public Task CreateAsync(InstructorCreateDto dto)     
        => _http.PostAsJsonAsync("api/instructor/create", dto);

    public Task UpdateAsync(string id, InstructorUpdateDto dto) 
        => _http.PutAsJsonAsync($"api/instructor/update/{id}", dto);

    public Task DeleteAsync(string id)               
        => _http.DeleteAsync($"api/instructor/delete/{id}");
}