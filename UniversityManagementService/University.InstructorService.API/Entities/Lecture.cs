using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.InstructorService.API.Entities;

[Table("Lectures")]
public class Lecture
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required, StringLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    public string? CourseId { get; set; }
    
    public Course? Course { get; set; }
}