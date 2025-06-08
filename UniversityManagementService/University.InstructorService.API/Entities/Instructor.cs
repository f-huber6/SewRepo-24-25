using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.InstructorService.API.Entities;

[Table("Instructors")]
public class Instructor
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName  { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}