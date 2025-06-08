namespace University.StudentService.API.Dto;

public class StudentUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? GraduationDate { get; set; }
}