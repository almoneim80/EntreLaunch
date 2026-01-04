using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class UpdateCourseExamValidator : AbstractValidator<UpdateCourseExamDto>
    {
        public UpdateCourseExamValidator(ILocalizationManager localizationManager)
        {
            // Name: optional but must not be whitespace and 3-250 length if provided
            RuleFor(x => x.Name)
                .Must(x => string.IsNullOrWhiteSpace(x) || !string.IsNullOrWhiteSpace(x))
                .WithMessage(localizationManager.GetLocalizedString("CourseExamNameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .Length(3, 250).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("CourseExamNameLengthRange"));

            // Description: optional, but must follow length range if provided
            RuleFor(x => x.Description)
                .Length(10, 2000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("CourseExamDescriptionLengthRange"));

            // MinMark: must be positive if provided
            RuleFor(x => x.MinMark)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CourseExamMinMarkNonNegative"));

            // MaxMark: must be positive if provided
            RuleFor(x => x.MaxMark)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxMark.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CourseExamMaxMarkNonNegative"));

            // MinMark <= MaxMark: only if both provided
            RuleFor(x => x)
                .Must(dto =>
                    (!dto.MinMark.HasValue || !dto.MaxMark.HasValue) ||
                    dto.MinMark <= dto.MaxMark
                )
                .WithMessage(localizationManager.GetLocalizedString("CourseExamMinMustNotExceedMax"));

            // DurationInMinutes: must be positive if provided
            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0)
                .When(x => x.DurationInMinutes.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CourseExamDurationMustBePositive"));

            // CourseId: must be >= 1 if provided
            RuleFor(x => x.CourseId)
                .GreaterThanOrEqualTo(1)
                .When(x => x.CourseId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CourseExamCourseIdMustBeValid"));
        }
    }
}
