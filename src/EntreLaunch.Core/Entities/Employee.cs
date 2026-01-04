namespace EntreLaunch.Entities;

public class Employee : SharedData
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public string? WorkField { get; set; }
    public string? JobTitle { get; set; }
    public string? EmployeeDefinition { get; set; }
    public List<string>? Skills { get; set; }
    public EmployeeStaus Status { get; set; }

    public virtual ICollection<EmployeePortfolio>? Portfolios { get; set; }
}
