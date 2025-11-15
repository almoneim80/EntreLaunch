namespace EntreLaunch.Entities;

public class PortfolioAttachment : SharedData
{
    public int PortfolioId { get; set; }
    public virtual EmployeePortfolio Portfolio { get; set; } = null!;

    public string? Url { get; set; }
}
