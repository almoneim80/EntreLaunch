using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.ExamValidators
{
    public class AnswerCreateDtoWithChildrenValidator : AbstractValidator<AnswerCreateDtoWithChildren>
    {
        public AnswerCreateDtoWithChildrenValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Text)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("AnswerTextRequired"));

            RuleFor(x => x.IsCorrect)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("AnswerIsCorrectRequired"));
        }
    }
}
