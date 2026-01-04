using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class IdeaPowerCreateDtoValidator : AbstractValidator<IdeaPowerCreateDto>
    {
        public IdeaPowerCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.CategoryType)
                .IsInEnum()
                .WithMessage(localization.GetLocalizedString("InvalidCategoryType"));

            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("CategoryNameRequired"));

            RuleFor(x => x.FactorData)
                .NotNull().WithMessage(localization.GetLocalizedString("FactorDataRequired"))
                .Must(list => list.Any()).WithMessage(localization.GetLocalizedString("FactorDataRequired"));

            RuleForEach(x => x.FactorData)
                .SetValidator(new FactorDataValidator(localization));
        }
    }
}
