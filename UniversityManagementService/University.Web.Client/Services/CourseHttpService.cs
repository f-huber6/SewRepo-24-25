using System.Net.Http.Json;
using University.Shared.DtoCourse;

namespace University.Web.Client.Services;

public class CourseHttpService
{
    readonly HttpClient _http;
    public CourseHttpService(HttpClient http) => _http = http;

    public Task<List<CourseDto>> GetAllAsync() =>
        _http.GetFromJsonAsync<List<CourseDto>>("api/course/all")!;

    public Task<CourseDto> GetByIdAsync(string id) =>
        _http.GetFromJsonAsync<CourseDto>($"api/course/read/{id}")!;

    public Task CreateAsync(CourseCreateDto dto) =>
        _http.PostAsJsonAsync("api/course/create", dto);

    public Task UpdateAsync(string id, CourseUpdateDto dto) =>
        _http.PutAsJsonAsync($"api/course/update/{id}", dto);

    public Task DeleteAsync(string id) =>
        _http.DeleteAsync($"api/course/delete/{id}");
}