using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.LessonValidators;

public class LessonsCreateValidator : AbstractValidator<LessonsCreateDto>
{
    public LessonsCreateValidator(ILocalizationManager localizationManager)
    {
        RuleFor(x => x.Name)
            .MustNotBeDefault()
            .WithMessage(localizationManager.GetLocalizedString("LessonNameRequired"));

        RuleFor(x => x.VideoUrl)
            .MustBeValidVideo()
            .WithMessage(localizationManager.GetLocalizedString("InvalidVideoUrl"));

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage(localizationManager.GetLocalizedString("LessonOrderMustBePositive"));

        RuleFor(x => x.DurationInMinutes)
            .GreaterThan(0)
            .WithMessage(localizationManager.GetLocalizedString("LessonDurationMustBePositive"));

        RuleFor(x => x.Description)
            .MustHaveLengthInRange(10, 500)
            .WithMessage(localizationManager.GetLocalizedString("LessonDescriptionRange"));

        RuleFor(x => x.Attachments)
            .NotNull()
            .WithMessage(localizationManager.GetLocalizedString("AttachmentsRequired"))
            .ForEach(x => x.SetValidator(new LessonsAttachmentsCreateDtoValidator(localizationManager)));
    }
}

public class LessonsAttachmentsCreateDtoValidator : AbstractValidator<LessonsAttachmentsCreateDto>
{
    public LessonsAttachmentsCreateDtoValidator(ILocalizationManager localizationManager)
    {
        RuleFor(x => x.FileName)
            .MustNotBeDefault()
            .WithMessage(localizationManager.GetLocalizedString("AttachmentFileNameRequired"));

        RuleFor(x => x.FileUrl)
            .MustBeValidAttachment()
            .WithMessage(localizationManager.GetLocalizedString("AttachmentFileUrlInvalid"));
    }
}
