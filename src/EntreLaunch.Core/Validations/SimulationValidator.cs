using FluentValidation;

namespace EntreLaunch.Validations;

public class SimulationCreateValidator : AbstractValidator<Simulation>
{
    public SimulationCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.ProjectField).MustNotBeDefault();
        RuleFor(x => x.ProjectField).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.ProjectField).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.ProjectType).MustNotBeDefault();
        RuleFor(x => x.ProjectType).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.ProjectType).MustHaveLengthInRange(2, 250);
    }
}

/***************Purchase******************/
/***************Purchase******************/

public class PurchaseCreateValidator : AbstractValidator<SimulationPurchase>
{
    public PurchaseCreateValidator()
    {
        RuleFor(x => x.ItemName).MustNotBeDefault();
        RuleFor(x => x.ItemName).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.ItemName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.ItemCost).MustNotBeDefault();
        RuleFor(x => x.ItemCost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);
    }
}

/***************Product Ad******************/
/***************Product Ad******************/
public class ProductAdCreateValidator : AbstractValidator<SimulationAdvertisement>
{
    public ProductAdCreateValidator()
    {
        RuleFor(x => x.AdUrl).MustNotBeDefault();
        RuleFor(x => x.AdUrl).MustBeValidAttachment();

        RuleFor(x => x.AdType).MustNotBeDefault();
        RuleFor(x => x.AdType).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.AdType).MustHaveLengthInRange(2, 250);
    }
}

/***************Marketing******************/
/***************Marketing******************/
public class MarketingCreateValidator : AbstractValidator<SimulationMarketing>
{
    public MarketingCreateValidator()
    {
        RuleFor(x => x.ProductName).MustNotBeDefault();
        RuleFor(x => x.ProductName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Quantity).MustNotBeDefault();
        RuleFor(x => x.Quantity).MustBeInRange(1, int.MaxValue);

        RuleFor(x => x.UnitPrice).MustNotBeDefault();
        RuleFor(x => x.UnitPrice).MustBeInRange(1, double.MaxValue);
    }
}

/***************Marketing Campaign******************/
/***************Marketing Campaign******************/
public class MarketingCampaignCreateValidator : AbstractValidator<SimulationCampaign>
{
    public MarketingCampaignCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Cost).MustNotBeDefault();
        RuleFor(x => x.Cost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.EndAt).MustNotBeDefault();
        RuleFor(x => x.EndAt).MustBeValidDate();
        RuleFor(x => x.EndAt).MustBeValidDate(mustBeFuture: true);
    }
}

/***************Idea Strength******************/
/***************Idea Strength******************/
public class IdeaStrengthCreateValidator : AbstractValidator<SimulationIdeaPower>
{
    public IdeaStrengthCreateValidator()
    {
        RuleFor(x => x.CategoryName).MustNotBeDefault();

        RuleFor(x => x.StrengthFactor).MustNotBeDefault();
        RuleFor(x => x.StrengthFactor).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.FactorScore).MustNotBeDefault();
        RuleFor(x => x.FactorScore).MustBeInRange(1, 5);
    }
}

/***************Feasibility Study******************/
/***************Feasibility Study******************/

public class FeasibilityStudyCreateValidator : AbstractValidator<SimulationFeasibilityStudy>
{
    public FeasibilityStudyCreateValidator()
    {
        RuleFor(x => x.ProjectName).MustNotBeDefault();
        RuleFor(x => x.ProjectName).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.ProjectName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.MaxCapital).MustNotBeDefault();
        RuleFor(x => x.MaxCapital).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.MinCapital).MustNotBeDefault();
        RuleFor(x => x.MinCapital).MustBeInRange(0.01d, double.MaxValue);
        RuleFor(x => x.MinCapital).MustBeLessThanOrEqualTo(x => x.MaxCapital);

        RuleFor(x => x.InterestRate).MustNotBeDefault();
        RuleFor(x => x.InterestRate).MustBeInRange(1, 100);

        RuleFor(x => x.MarketingCost).MustNotBeDefault();
        RuleFor(x => x.MarketingCost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.RentCost).MustNotBeDefault();
        RuleFor(x => x.RentCost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.DecorationCost).MustNotBeDefault();
        RuleFor(x => x.DecorationCost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.EquipmentCost).MustNotBeDefault();
        RuleFor(x => x.EquipmentCost).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.GovFees).MustNotBeDefault();
        RuleFor(x => x.GovFees).MustBeInRange(0.01d, double.MaxValue);

        RuleFor(x => x.InventoryCost).MustNotBeDefault();
        RuleFor(x => x.InventoryCost).MustBeInRange(0.01d, double.MaxValue);
    }
}

/***************Business Plan******************/
/***************Business Plan******************/
public class BusinessPlanCreateValidation : AbstractValidator<SimulationBusinessPlan>
{
    public BusinessPlanCreateValidation()
    {
        RuleFor(x => x.BusinessPlanFileUrl).MustBeValidAttachment();
        RuleFor(x => x.BusinessPlanFileUrl).MustBeValidAttachment();

        RuleFor(x => x.BusinessPartners).MustNotBeDefault();

        RuleFor(x => x.ProjectActivities).MustNotBeDefault();

        RuleFor(x => x.ValueProposition).MustNotBeDefault();

        RuleFor(x => x.CustomerRelationships).MustNotBeDefault();

        RuleFor(x => x.CustomerSegments).MustNotBeDefault();

        RuleFor(x => x.RequiredResources).MustNotBeDefault();

        RuleFor(x => x.DistributionChannels).MustNotBeDefault();

        RuleFor(x => x.RevenueStreams).MustNotBeDefault();

        RuleFor(x => x.CostStructure).MustNotBeDefault();
    }
}
