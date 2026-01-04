using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class CampaignCreateDtoValidator : AbstractValidator<CampaignCreateDto>
    {
        public CampaignCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault();

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(2, 100)
                .WithMessage(localization.GetLocalizedString("CampaignNameRequired"));

            RuleFor(x => x.Cost)
                .NotNull();

            RuleFor(x => x.Cost)
               .GreaterThan(0)
                .WithMessage(localization.GetLocalizedString("CampaignCostPositive"));

            RuleFor(x => x.EndAt)
                .MustBeValidDate(mustBeFuture: true)
                .WithMessage(localization.GetLocalizedString("CampaignEndDateFuture"));
        }
    }
}
