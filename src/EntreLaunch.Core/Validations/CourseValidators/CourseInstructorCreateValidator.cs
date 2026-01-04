namespace EntreLaunch.Validations.CourseValidators
{
    public class CourseInstructorCreateValidator : AbstractValidator<CourseInstructorCreateDto>
    {
        public CourseInstructorCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CourseIdGreaterThanZero"));

            RuleFor(x => x.UserId)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("UserIdRequired"));
        }
    }
}
