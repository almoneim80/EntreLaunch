namespace EntreLaunch.Validations.CourseValidators
{
    public class CourseFieldCreateValidator : AbstractValidator<CourseFieldCreateDto>
    {
        public CourseFieldCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault();

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 100)
                .WithMessage(localizationManager.GetLocalizedString("FieldNameLength"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(0, 500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("FieldDescriptionLength"));
        }
    }
}
