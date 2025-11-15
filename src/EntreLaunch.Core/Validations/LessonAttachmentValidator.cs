using FluentValidation;

namespace EntreLaunch.Validations;

public class LessonAttachmentCreateValidator : AbstractValidator<LessonAttachmentCreateDto>
{
    public LessonAttachmentCreateValidator()
    {
        RuleFor(x => x.FileName).MustNotBeDefault();
        RuleFor(x => x.FileName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.FileUrl).MustNotBeDefault();
        RuleFor(x => x.FileUrl).MustBeValidAttachment();
        RuleFor(x => x.FileUrl).MustBeValidDocument();

        RuleFor(x => x.LessonId).MustNotBeDefault();
    }
}

public class LessonAttachmentEntreLaunchdateValidator : AbstractValidator<LessonAttachmentEntreLaunchdateDto>
{
    public LessonAttachmentEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Url).MustBeValidAttachment();
        //RuleFor(x => x.Url).MustBeValidImage();

        //RuleFor(x => x.LessonId).MustNotBeDefault();
    }
}
