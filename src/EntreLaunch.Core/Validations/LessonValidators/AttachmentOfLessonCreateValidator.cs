using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.LessonValidators
{
    public class AttachmentOfLessonCreateValidator : AbstractValidator<LessonsAttachmentsCreateDto>
    {
        public AttachmentOfLessonCreateValidator(ILocalizationManager localizationManager)
        {
            // FileName: required, min/max length
            RuleFor(x => x.FileName)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileNameRequired"));

            RuleFor(x => x.FileName)
                .MustHaveLengthInRange(2, 200)
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileNameLength"));

            // FileUrl: required and valid
            RuleFor(x => x.FileUrl)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileUrlRequired"));

            RuleFor(x => x.FileUrl)
               .MustBeValidAttachment()
                .WithMessage(localizationManager.GetLocalizedString("AttachmentFileUrlInvalid"));
        }
    }
}
