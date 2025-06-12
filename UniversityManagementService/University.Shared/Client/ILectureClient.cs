using University.Shared.DtoLecture;

namespace University.Shared.Client;

public interface ILectureClient
{
    Task<List<LectureDto>> GetAllAsync();
    Task<LectureDto> GetByIdAsync(string lectureId);
}