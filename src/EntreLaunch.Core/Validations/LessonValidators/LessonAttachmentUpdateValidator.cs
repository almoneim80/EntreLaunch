using EntreLaunch.DTOs.LessonDtos;

namespace EntreLaunch.Validations.LessonValidators
{
    public class LessonAttachmentUpdateValidator : AbstractValidator<LessonAttachmentUpdateDto>
    {
        public LessonAttachmentUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.FileName))
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileNameRequired"));

            RuleFor(x => x.FileUrl)
                .MustBeValidDocument()
                .When(x => !string.IsNullOrWhiteSpace(x.FileUrl))
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileUrlInvalid"));

            RuleFor(x => x.LessonId)
                .GreaterThan(0)
                .When(x => x.LessonId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonIdMustBePositive"));
        }
    }
}
