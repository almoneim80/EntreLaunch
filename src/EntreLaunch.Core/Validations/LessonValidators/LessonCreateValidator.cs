using EntreLaunch.DTOs.LessonDtos;

namespace EntreLaunch.Validations.LessonValidators
{
    public class LessonCreateValidator : AbstractValidator<LessonCreateDto>
    {
        public LessonCreateValidator(ILocalizationManager localizationManager)
        {
            // Name
            RuleFor(x => x.Name)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("LessonNameRequired"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(2, 250)
                .WithMessage(localizationManager.GetLocalizedString("LessonNameLength"));

            // VideoUrl
            RuleFor(x => x.VideoUrl)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("VideoUrlRequired"));

            RuleFor(x => x.Name)
                .MustBeValidVideo()
                .WithMessage(localizationManager.GetLocalizedString("VideoUrlInvalid"));

            // Order
            RuleFor(x => x.Order)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("LessonOrderMustBePositive"));

            // DurationInMinutes
            RuleFor(x => x.DurationInMinutes)
                .MustBeInRange(1, 1000)
                .When(x => x.DurationInMinutes.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonDurationRange"));

            // Description
            RuleFor(x => x.Description)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("LessonDescriptionRequired"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(5, 2500)
                .WithMessage(localizationManager.GetLocalizedString("LessonDescriptionLength"));

            // CourseId
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CourseIdMustBePositive"));
        }
    }
}
