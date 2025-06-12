using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.StudentService.API.Entities;

namespace BlazorApp1.Entities;
[Table("Tests")]
public class Test
{
    [Key]
    public int Id { get; set; }
    public string Titel { get; set; }
    public DateTime Datum { get; set; }
    
    public string StudentId { get; set; }
    public Student Student { get; set; }
}