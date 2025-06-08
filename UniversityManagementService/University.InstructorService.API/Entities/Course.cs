using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.InstructorService.API.Entities;

[Table("Courses")]
public class Course
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? InstructorId { get; set; }
    
    public Instructor? Instructor { get; set; }
    
    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}