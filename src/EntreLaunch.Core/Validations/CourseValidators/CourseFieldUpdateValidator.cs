namespace EntreLaunch.Validations.CourseValidators
{
    public class CourseFieldUpdateValidator : AbstractValidator<CourseFieldUpdateDto>
    {
        public CourseFieldUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("FieldNameLength"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(0, 500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("FieldDescriptionLength"));
        }
    }
}
