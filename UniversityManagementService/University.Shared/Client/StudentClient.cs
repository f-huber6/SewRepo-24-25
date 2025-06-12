using System.Net.Http.Json;
using University.Shared.DtoLecture;
using University.StudentService.API.Dto;

namespace University.Shared.Client;

public class StudentClient: IStudentClient
{
    private readonly HttpClient _http;
    public StudentClient(HttpClient http) => _http = http;

    public Task<List<StudentDto>> GetAllAsync() =>
        _http.GetFromJsonAsync<List<StudentDto>>("api/student/all")!;

    public Task EnrollStudentAsync(string studentId, string lectureId) =>
        _http.PostAsync($"api/student/{studentId}/enroll/{lectureId}", null);

    public Task<List<LectureDto>> GetLecturesForStudentAsync(string? studentId) =>
        _http.GetFromJsonAsync<List<LectureDto>>($"api/student/{studentId}/lectures")!;
}