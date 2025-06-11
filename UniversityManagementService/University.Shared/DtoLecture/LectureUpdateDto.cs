namespace University.Shared.DtoLecture;

public class LectureUpdateDto
{
    public string CourseId { get; set; } = default!;
    
    public string RoomNumber { get; set; } = default!;

    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
}