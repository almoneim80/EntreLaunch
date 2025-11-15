namespace EntreLaunch.Entities;

public class TrainingPath : SharedData
{
    public string? Name { get; set; }
    public decimal Price { get; set; } = 0;
    public string? Description { get; set; }

    public virtual ICollection<Course>? Courses { get; set; }
    public virtual ICollection<Exam>? Exams { get; set; }
}
