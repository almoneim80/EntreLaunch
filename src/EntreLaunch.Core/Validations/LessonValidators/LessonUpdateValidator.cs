using EntreLaunch.DTOs.LessonDtos;

namespace EntreLaunch.Validations.LessonValidators
{
    public class LessonUpdateValidator : AbstractValidator<LessonUpdateDto>
    {
        public LessonUpdateValidator(ILocalizationManager localizationManager)
        {
            // Name
            RuleFor(x => x.Name)
                .MustHaveLengthInRange(2, 250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("LessonNameLength"));

            // VideoUrl
            RuleFor(x => x.VideoUrl)
                .MustBeValidVideo()
                .When(x => !string.IsNullOrWhiteSpace(x.VideoUrl))
                .WithMessage(localizationManager.GetLocalizedString("VideoUrlInvalid"));

            // DurationInMinutes
            RuleFor(x => x.DurationInMinutes)
                .MustBeInRange(1, 1000)
                .When(x => x.DurationInMinutes.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonDurationRange"));

            // Description
            RuleFor(x => x.Description)
                .MustHaveLengthInRange(5, 2500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("LessonDescriptionLength"));

            // Order
            RuleFor(x => x.Order)
                .GreaterThan(0)
                .When(x => x.Order.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LessonOrderMustBePositive"));

            // CourseId
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .When(x => x.CourseId != 0)
                .WithMessage(localizationManager.GetLocalizedString("CourseIdMustBePositive"));
        }
    }
}
