namespace University.TestsService.API.Dto;

public class TestsDto
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public DateTime Datum { get; set; }

        public string StudentId { get; set; }
        public string StudentFullName { get; set; } 
    }

}