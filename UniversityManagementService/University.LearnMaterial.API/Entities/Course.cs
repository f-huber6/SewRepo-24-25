namespace University.LearnMaterial.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<LearnMaterial> LearnMaterials { get; set; }
}