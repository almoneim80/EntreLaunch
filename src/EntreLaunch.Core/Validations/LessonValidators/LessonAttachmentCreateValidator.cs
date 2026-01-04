using EntreLaunch.DTOs.LessonDtos;

namespace EntreLaunch.Validations.LessonValidators
{
    public class LessonAttachmentCreateValidator : AbstractValidator<AttachmentOfLessonCreateDto>
    {
        public LessonAttachmentCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.FileName))
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileNameRequired"));

            RuleFor(x => x.FileUrl)
                .MustBeValidDocument()
                .When(x => !string.IsNullOrWhiteSpace(x.FileUrl))
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileUrlInvalid"));
        }
    }
}
