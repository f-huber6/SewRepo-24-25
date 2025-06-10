using System.ComponentModel.DataAnnotations;

namespace University.TestsService.API.Dto;

public class TestsUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Titel { get; set; }

    [Required]
    public DateTime Datum { get; set; }

    [Required]
    public string StudentId { get; set; }
}