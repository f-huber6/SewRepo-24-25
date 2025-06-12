using University.Shared.DtoLecture;
using University.StudentService.API.Dto;

namespace University.Shared.Client;

public interface IStudentClient
{
    Task<List<StudentDto>> GetAllAsync();
    Task EnrollStudentAsync(string studentId, string lectureId);
    Task<List<LectureDto>> GetLecturesForStudentAsync(string? studentId);
}