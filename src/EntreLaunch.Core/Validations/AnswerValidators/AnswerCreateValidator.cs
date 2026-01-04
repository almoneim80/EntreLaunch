using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.Validations.AnswerValidators
{
    public class AnswerCreateValidator : AbstractValidator<AnswerCreateDto>
    {
        public AnswerCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate QuestionId: must not be 0
            RuleFor(x => x.QuestionId)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("FieldCannotBeZero"));

            // Validate Text: must not be empty and must contain only letters and spaces
            RuleFor(x => x.Text)
                .MustNotBeDefault().WithMessage(localizationManager.GetLocalizedString("TextCannotBeEmpty"))
                .MustHaveLengthInRange(1, 250).WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));

            // Validate IsCorrect: must be a valid boolean value (not null)
            RuleFor(x => x.IsCorrect)
                    .Must(x => x.HasValue && (x.Value == true || x.Value == false))
                    .WithMessage(localizationManager.GetLocalizedString("IsCorrectMustBeTrueOrFalse"));
        }
    }
}
