using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class QuestionCreateDtoWithChildrenValidator : AbstractValidator<QuestionCreateDtoWithChildren>
    {
        public QuestionCreateDtoWithChildrenValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Text)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("QuestionTextRequired"));

            RuleFor(x => x.Mark)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("QuestionMarkRequired"));

            RuleFor(x => x.Answers)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("AnswersRequired"));

            RuleFor(x => x.Answers)
                .Must(a => a != null && a.Any())
                .WithMessage(localizationManager.GetLocalizedString("AtLeastOneAnswerRequired"));

            RuleForEach(x => x.Answers)
                .SetValidator(new AnswerCreateDtoWithChildrenValidator(localizationManager));
        }
    }
}
