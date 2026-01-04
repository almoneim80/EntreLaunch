using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.CourseValidators
{
    public class PathCourseUpdateValidator : AbstractValidator<PathCourseUpdateDto>
    {
        public PathCourseUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 500)
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 2500)
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));

            RuleFor(x => x.Logo)
                .MustBeValidImage().When(x => !string.IsNullOrWhiteSpace(x.Logo))
                .WithMessage(localizationManager.GetLocalizedString("LogoMustBeValidImage"));

            RuleFor(x => x.PathId)
                .GreaterThan(0).When(x => x.PathId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PathIdGreaterThanZero"));

            // Conditional: Certificate validity only if both props present and CertificateExists == true
            When(x => x.CertificateExists == true && x.CertificateValidityInDays.HasValue, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });

            void AddListValidation(Expression<Func<PathCourseUpdateDto, List<string>?>> prop, string msgKey)
            {
                RuleFor(prop)
                    .Must(list => list == null || (list.Count > 0 && list.TrueForAll(item => !string.IsNullOrWhiteSpace(item))))
                    .WithMessage(localizationManager.GetLocalizedString(msgKey));
            }

            AddListValidation(x => x.Audience, "AudienceMustContainValidItems");
            AddListValidation(x => x.Requirements, "RequirementsMustContainValidItems");
            AddListValidation(x => x.Topics, "TopicsMustContainValidItems");
            AddListValidation(x => x.Goals, "GoalsMustContainValidItems");
            AddListValidation(x => x.Outcomes, "OutcomesMustContainValidItems");
        }
    }
}
