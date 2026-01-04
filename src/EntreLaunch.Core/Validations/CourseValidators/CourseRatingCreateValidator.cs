namespace EntreLaunch.Validations.CourseValidators
{
    public class CourseRatingCreateValidator : AbstractValidator<CourseRatingCreateDto>
    {
        public CourseRatingCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CourseIdGreaterThanZero"));

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage(localizationManager.GetLocalizedString("RatingBetweenOneAndFive"));

            RuleFor(x => x.Review)
                .MustHaveLengthInRange(3, 500)
                .When(x => !string.IsNullOrWhiteSpace(x.Review))
                .WithMessage(localizationManager.GetLocalizedString("ReviewLengthRange"));
        }
    }
}
