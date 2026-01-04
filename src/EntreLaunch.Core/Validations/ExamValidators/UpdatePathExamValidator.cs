using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class UpdatePathExamValidator : AbstractValidator<UpdatePathExamDto>
    {
        public UpdatePathExamValidator(ILocalizationManager localizationManager)
        {
            // Name: optional but must be non-whitespace if provided
            RuleFor(x => x.Name)
                .Must(name => string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(name))
                .WithMessage(localizationManager.GetLocalizedString("PathExamNameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .Length(3, 250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("PathExamNameLengthRange"));

            // Description: optional, if provided then within limits
            RuleFor(x => x.Description)
                .Length(10, 2000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("PathExamDescriptionLengthRange"));

            // MinMark
            RuleFor(x => x.MinMark)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PathExamMinMarkNonNegative"));

            // MaxMark
            RuleFor(x => x.MaxMark)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PathExamMaxMarkNonNegative"));

            // Min <= Max
            RuleFor(x => x)
                .Must(dto =>
                    (!dto.MinMark.HasValue || !dto.MaxMark.HasValue) ||
                    dto.MinMark <= dto.MaxMark)
                .WithMessage(localizationManager.GetLocalizedString("PathExamMinMustNotExceedMax"));

            // Duration
            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0)
                .When(x => x.DurationInMinutes.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PathExamDurationMustBePositive"));
        }
    }
}
