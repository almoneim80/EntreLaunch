using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.AnswerValidators
{
    public class AnswerUpdateValidator : AbstractValidator<AnswerUpdateDto>
    {
        public AnswerUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.QuestionId)
                .GreaterThan(0).When(x => x.QuestionId > 0).WithMessage(localizationManager.GetLocalizedString("QuestionIdGreaterThanZero"));

            RuleFor(x => x.Text)
                .Length(2, 250).When(x => !string.IsNullOrWhiteSpace(x.Text)).WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));

            RuleFor(x => x.Text)
                .MustHaveLengthInRange(1, 250).When(x => !string.IsNullOrWhiteSpace(x.Text))
                .WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));

            RuleFor(x => x.IsCorrect)
                .Must(x => x == true || x == false).When(x => x.IsCorrect.HasValue).WithMessage(localizationManager.GetLocalizedString("IsCorrectValidValue"));
        }
    }
}
