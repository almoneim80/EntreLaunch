using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class ProjectPurchaseCreateDtoValidator : AbstractValidator<ProjectPurchaseCreateDto>
    {
        public ProjectPurchaseCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Description)
                .MustNotBeDefault();

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(5, 300)
                .WithMessage(localization.GetLocalizedString("PurchaseDescriptionRequired"));

            RuleFor(x => x.Products)
                .NotNull();

            RuleFor(x => x.Products)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("PurchaseProductsRequired"));

            RuleForEach(x => x.Products).ChildRules(product =>
            {
                product.RuleFor(p => p.ItemName)
                    .MustNotBeDefault();

                product.RuleFor(p => p.ItemName)
                    .MustHaveLengthInRange(2, 100)
                    .WithMessage(localization.GetLocalizedString("PurchaseItemNameRequired"));

                product.RuleFor(p => p.ItemCost)
                    .GreaterThan(0)
                    .WithMessage(localization.GetLocalizedString("PurchaseItemCostPositive"));
            });
        }
    }
}
