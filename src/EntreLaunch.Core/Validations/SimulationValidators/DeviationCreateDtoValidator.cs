using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class DeviationCreateDtoValidator : AbstractValidator<DeviationCreateDto>
    {
        public DeviationCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Type)
                .MustNotBeDefault()
                .WithMessage(localization.GetLocalizedString("DeviationTypeRequired"));

            RuleFor(x => x.Amount)
                .NotNull();

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage(localization.GetLocalizedString("DeviationAmountPositive"));

            RuleFor(x => x.Reason)
                .MustHaveLengthInRange(5, 500)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason))
                .WithMessage(localization.GetLocalizedString("DeviationReasonLength"));
        }
    }
}
