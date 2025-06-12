using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.EnrollmentService.API.Entities;

[Table("Enrollments")]
public class Enrollment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required, StringLength(100)]
    public string StudentId { get; set; } = string.Empty;
    
    [Required, StringLength(100)]
    public string LectureId { get; set; } = string.Empty;
    
    [Required]
    public DateTime EnrollmentDate { get; set; }
}