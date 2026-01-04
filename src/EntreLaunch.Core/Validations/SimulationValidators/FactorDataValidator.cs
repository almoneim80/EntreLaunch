using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class FactorDataValidator : AbstractValidator<FactorData>
    {
        public FactorDataValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.StrengthFactor)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("StrengthFactorRequired"));

            RuleFor(x => x.FactorScore)
                .InclusiveBetween(1, 5)
                .WithMessage(localization.GetLocalizedString("FactorScoreRange"));
        }
    }
}
