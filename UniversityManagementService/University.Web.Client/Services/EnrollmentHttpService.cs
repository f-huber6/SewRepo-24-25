using System.Net.Http.Json;
using University.Shared.DtoEnrollment;
using University.Shared.DtoLecture;

namespace University.Web.Client.Services;

public class EnrollmentHttpService
{
    readonly HttpClient _http;
    public EnrollmentHttpService(HttpClient http) => _http = http;
    
    public Task<HttpResponseMessage> EnrollStudentAsync(string studentId, string lectureId)
        => _http.PostAsync($"api/enrollments/assign?studentId={studentId}&lectureId={lectureId}", null);
    
    public Task<List<LectureDto>> GetLecturesForStudentAsync(string? studentId)
        => _http.GetFromJsonAsync<List<LectureDto>>($"api/enrollments/student/{studentId}")!;
    
    public Task<HttpResponseMessage> RemoveAssignmentAsync(string studentId, string lectureId)
        => _http.DeleteAsync($"api/enrollments/remove?studentId={studentId}&lectureId={lectureId}");
    
    public Task<List<EnrollmentDto>> GetAllAssignmentsAsync() =>
        _http.GetFromJsonAsync<List<EnrollmentDto>>("api/enrollments/all")!;

}