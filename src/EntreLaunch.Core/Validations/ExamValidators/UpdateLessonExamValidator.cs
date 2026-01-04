using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class UpdateLessonExamValidator : AbstractValidator<UpdateLessonExamDto>
    {
        public UpdateLessonExamValidator(ILocalizationManager localizationManager)
        {
            // Name: optional but must not be empty/whitespace and length between 3–250
            RuleFor(x => x.Name)
                .Must(x => string.IsNullOrWhiteSpace(x) || !string.IsNullOrWhiteSpace(x.Trim()))
                .WithMessage(localizationManager.GetLocalizedString("LessonExamNameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .Length(3, 250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("LessonExamNameLengthRange"));

            // Description: optional, but if provided, must be within length range
            RuleFor(x => x.Description)
                .Length(10, 2000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("LessonExamDescriptionLengthRange"));

            // MinMark: must be positive if provided
            RuleFor(x => x.MinMark)
                .GreaterThanOrEqualTo(0).When(x => x.MinMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonExamMinMarkNonNegative"));

            // MaxMark: must be positive if provided
            RuleFor(x => x.MaxMark)
                .GreaterThanOrEqualTo(0).When(x => x.MaxMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonExamMaxMarkNonNegative"));

            // If both provided: MinMark <= MaxMark
            RuleFor(x => x)
                .Must(dto =>
                    (!dto.MinMark.HasValue || !dto.MaxMark.HasValue) ||
                    dto.MinMark <= dto.MaxMark
                )
                .WithMessage(localizationManager.GetLocalizedString("LessonExamMinMustNotExceedMax"));

            // Duration: must be positive if provided
            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).When(x => x.DurationInMinutes.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonExamDurationMustBePositive"));

            // LessonId: must be > 0 if provided
            RuleFor(x => x.LessonId)
                .GreaterThan(0).When(x => x.LessonId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonIdMustBeGreaterThanZero"));
        }
    }
}
