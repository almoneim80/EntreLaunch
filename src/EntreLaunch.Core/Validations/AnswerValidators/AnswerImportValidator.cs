using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.AnswerValidators
{
    public class AnswerImportValidator : AbstractValidator<AnswerImportDto>
    {
        public AnswerImportValidator(ILocalizationManager localizationManager)
        {
            // Validate QuestionId: must be greater than 0
            RuleFor(x => x.QuestionId)
                .GreaterThan(0).When(x => x.QuestionId > 0)
                .WithMessage(localizationManager.GetLocalizedString("QuestionIdGreaterThanZero"));

            // Validate Text: must not be empty and must contain only letters and spaces
            RuleFor(x => x.Text)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Text))
                .WithMessage(localizationManager.GetLocalizedString("TextCannotBeEmpty"));

            RuleFor(x => x.Text)
                .MustHaveLengthInRange(1, 250).When(x => !string.IsNullOrWhiteSpace(x.Text))
                .WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));

            // Validate IsCorrect: must be a valid boolean value (not null)
            RuleFor(x => x.IsCorrect)
                .Must(x => x.HasValue && (x.Value == true || x.Value == false))
                .When(x => x.IsCorrect.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("IsCorrectMustBeTrueOrFalse"));
        }
    }
}
