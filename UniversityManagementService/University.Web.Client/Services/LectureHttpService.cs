using System.Net.Http.Json;
using University.Shared.DtoLecture;

namespace University.Web.Client.Services;

public class LectureHttpService
{
    readonly HttpClient _http;
    public LectureHttpService(HttpClient http) => _http = http;

    public Task<List<LectureDto>> GetAllAsync() =>
        _http.GetFromJsonAsync<List<LectureDto>>("api/lecture/all")!;

    public Task<LectureDto> GetByIdAsync(string id) =>
        _http.GetFromJsonAsync<LectureDto>($"api/lecture/read/{id}")!;

    public Task CreateAsync(LectureCreateDto dto) =>
        _http.PostAsJsonAsync("api/lecture/create", dto);

    public Task UpdateAsync(string id, LectureUpdateDto dto) =>
        _http.PutAsJsonAsync($"api/lecture/update/{id}", dto);

    public Task DeleteAsync(string id) =>
        _http.DeleteAsync($"api/lecture/delete/{id}");
}