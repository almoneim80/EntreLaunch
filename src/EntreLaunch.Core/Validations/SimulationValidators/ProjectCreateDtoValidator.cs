using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
    {
        public ProjectCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.ProjectField)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("ProjectFieldRequired"));

            RuleFor(x => x.ProjectType)
                .NotEmpty()
                .WithMessage(localization.GetLocalizedString("ProjectTypeRequired"));
        }
    }
}
