namespace EntreLaunch.DTOs;

public class ProjectCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; } = null!;
    public string ProjectField { get; set; }
    public string ProjectType { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Idea Strength****************/
/*************Idea Strength****************/

public class IdeaPowerCreateDto
{
#nullable disable
    public Category CategoryType { get; set; } // Positive, Negative.
    public string CategoryName { get; set; } // Strengths, Weaknesses, etc.
    public List<FactorData> FactorData { get; set; }
    public int Total { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class FactorData
{
#nullable disable
    public string StrengthFactor { get; set; } // Strength Name, Weakness Name, etc.
    public int? FactorScore { get; set; } // 1..5
}

/*************Business Plan****************/
/*************Business Plan****************/
public class BusinessPlanCreateDto
{
#nullable enable
    public string? BusinessPlanFileUrl { get; set; }

    // Enforcing JSON fields to be non-empty
    public List<string>? BusinessPartners { get; set; }

    public List<string>? ProjectActivities { get; set; }

    public List<string>? ValueProposition { get; set; }

    public List<string>? CustomerRelationships { get; set; }

    public List<string>? CustomerSegments { get; set; }

    public List<string>? RequiredResources { get; set; }

    public List<string>? DistributionChannels { get; set; }

    public List<string>? RevenueStreams { get; set; }

    public List<string>? CostStructure { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Feasibility Study****************/
/*************Feasibility Study****************/
public class FeasibilityStudyCreateDto
{
    public string? ProjectName { get; set; }
    public double? CapitalMin { get; set; }
    public double? CapitalMax { get; set; }
    public bool? IsInterest { get; set; }
    public double? InterestRate { get; set; }
    public double? MarketingCost { get; set; }
    public double? RentCost { get; set; }
    public double? DecorationCost { get; set; }
    public double? EquipmentCost { get; set; }
    public double? GovFees { get; set; }
    public double? InventoryCost { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Purchase****************/
/*************Purchase****************/
public class PurchaseCreateDto
{
#nullable disable
    public List<Product> Products { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class Product
{
#nullable disable
    public string ItemName { get; set; }
    public double ItemCost { get; set; }
}

/*************Marketing****************/
/*************Marketing****************/
public class MarketingCreateDto
{
#nullable enable
    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public List<AdvertisementCreateDto>? Ads { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Product Ad****************/
/*************Product Ad****************/
public class AdvertisementCreateDto
{
    public string? AdUrl { get; set; }
    public string? AdType { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Campaign****************/
/*************Campaign****************/
public class CampaignCreateDto
{
    public string? Name { get; set; }
    public double? Cost { get; set; }
    public DateTimeOffset? EndAt { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/*************Deviation****************/
/*************Deviation****************/
public class DeviationCreateDto
{
    public string? Type { get; set; }
    public double? Amount { get; set; }
    public string? Reason { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class SimulationCreateDto
{
#nullable disable
    public ProjectCreateDto projectDto { get; set; }
    public List<IdeaPowerCreateDto> IdeaPowerhDto { get; set; }
    public BusinessPlanCreateDto BusinessPlanDto { get; set; }
    public FeasibilityStudyCreateDto FeasibilityStudyDto { get; set; }
    public PurchaseCreateDto PurchaseDto { get; set; }
    public List<MarketingCreateDto> MarketingDto { get; set; }
    public List<CampaignCreateDto> MarketingCampaignDto { get; set; }
}

/*************Simulation Details****************/
/*************Simulation Details****************/
public class SimulationDetails
{
#nullable disable
    public int Id { get; set; }
    public string ProjectField { get; set; }
    public string ProjectType { get; set; }
    public SimulationStatus ProjectStatus { get; set; }
    public double IdeaPowerhValue { get; set; }
    public double TotalCampaignValue { get; set; }

    public UserSimulationData userSimulationData { get; set; }
    public List<IdeaPowerCreateDto> IdeaPowerDto { get; set; }
    public BusinessPlanCreateDto BusinessPlanDto { get; set; }
    public FeasibilityStudyCreateDto FeasibilityStudyDto { get; set; }
    public PurchaseCreateDto PurchaseDto { get; set; }
    public List<MarketingCreateDto> MarketingDto { get; set; }
    public List<AdvertisementCreateDto> AdvertisementDto { get; set; }
    public List<CampaignCreateDto> MarketingCampaignDto { get; set; }
    public DeviationCreateDto DeviationAmountDto { get; set; }
}

public class UserSimulationData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class GuestRegisterDto
{
#nullable disable
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
#nullable enable
    public string? Email { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class GuestDetails
{
#nullable disable
    public string Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
}


public class FinalReportDto
{
#nullable disable
    public string ProjectName { get; set; }
    public double CapitalMin { get; set; }
    public double CapitalMax { get; set; }
    public double MarketingCost { get; set; }
    public double PurchasesCost { get; set; }
    public double CampaignCost { get; set; }
    public double DeviationPositive { get; set; }
    public double DeviationNegative { get; set; }
    public double FinalProfit { get; set; }
    public double ProfitPercentage { get; set; }
    public string RiskLevel { get; set; }

    // أي بيانات أخرى تحتاجها في العرض
    public double? InventoryCost { get; set; }
    public bool IsInterest { get; set; }
    public double? InterestRate { get; set; }
    public double? RemainingBudget { get; set; }
    public double? NetSales { get; set; }
}
