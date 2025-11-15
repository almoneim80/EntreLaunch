using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("employee_portfolios")]
public class EmployeePortfolio : SharedData, IBaseEntity
{
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    [Searchable]
    public string? ProjectTitle { get; set; }
    [Searchable]
    public decimal? CostFrom { get; set; }
    [Searchable]
    public decimal? CostTo { get; set; }
    [Searchable]
    public string? About { get; set; }

    public string? Logo { get; set; }

    public virtual ICollection<PortfolioAttachment>? PortfolioAttachments { get; set; }
}
