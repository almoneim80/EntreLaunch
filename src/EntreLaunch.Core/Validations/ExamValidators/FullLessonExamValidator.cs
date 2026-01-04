using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class FullLessonExamValidator : AbstractValidator<FullLessonExamDto>
    {
        public FullLessonExamValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("ExamNameRequired"));

            RuleFor(x => x.Description)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("ExamDescriptionRequired"));

            RuleFor(x => x.MinMark)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("ExamMinMarkRequired"));

            RuleFor(x => x.MaxMark)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("ExamMaxMarkRequired"));

            RuleFor(x => x.MaxMark)
                .Must((dto, max) => dto.MinMark.HasValue && max.HasValue && dto.MinMark.Value <= max.Value)
                .WithMessage(localizationManager.GetLocalizedString("ExamMarkRangeInvalid"));

            RuleFor(x => x.DurationInMinutes)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("ExamDurationRequired"));

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("ExamDurationPositive"));

            RuleFor(x => x.Questions)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("QuestionsRequired"));

            RuleFor(x => x.Questions)
                .Must(q => q.Any())
                .WithMessage(localizationManager.GetLocalizedString("AtLeastOneQuestionRequired"));

            RuleForEach(x => x.Questions)
                .SetValidator(new QuestionCreateDtoWithChildrenValidator(localizationManager));
        }
    }
}
