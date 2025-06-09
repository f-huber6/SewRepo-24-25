namespace University.LearnMaterial.Entities;

public class LearnMaterial
{
    public int Id { get; set; }
    public string Titel { get; set; }
    public string Description { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }
    
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
}


