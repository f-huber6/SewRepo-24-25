using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.StudentService.API.Entities;

[Table("Students")]
public class Student
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName  { get; set; } = string.Empty;

    [Required]
    public DateTime EnrollmentDate { get; set; } = DateTime.Today;

    public DateTime? GraduationDate { get; set; }
}