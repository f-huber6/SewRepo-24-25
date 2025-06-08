using System.Net.Http.Json;
using University.StudentService.API.Dto;

namespace University.Client.Services;

public class StudentHttpService
{
    private readonly HttpClient _http;

    public StudentHttpService(HttpClient http) => _http = http;

    public Task<List<StudentDto>> GetAllAsync()    
        => _http.GetFromJsonAsync<List<StudentDto>>("api/student/all")!;

    public Task<StudentDto> GetByIdAsync(string id) 
        => _http.GetFromJsonAsync<StudentDto>($"api/student/read/{id}")!;

    public Task CreateAsync(StudentCreateDto dto)     
        => _http.PostAsJsonAsync("api/student/create", dto);

    public Task UpdateAsync(string? id, StudentUpdateDto dto) 
        => _http.PutAsJsonAsync($"api/student/update/{id}", dto);

    public Task DeleteAsync(string? id)               
        => _http.DeleteAsync($"api/student/delete/{id}");
}