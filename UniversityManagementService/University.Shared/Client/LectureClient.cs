using System.Net.Http.Json;
using University.Shared.DtoCourse;
using University.Shared.DtoLecture;

namespace University.Shared.Client;

public class LectureClient: ILectureClient
{
    private readonly HttpClient _http;
    public LectureClient(HttpClient http) => _http = http;

    public Task<List<LectureDto>> GetAllAsync() =>
        _http.GetFromJsonAsync<List<LectureDto>>("api/lecture/all")!;

    public Task<LectureDto> GetByIdAsync(string lecturesId) =>
        _http.GetFromJsonAsync<LectureDto>($"api/lectures/read/{lecturesId}")!;
}