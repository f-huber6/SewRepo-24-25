namespace University.StudentService.API.Dto;

public class StudentDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? GraduationDate { get; set; }
}