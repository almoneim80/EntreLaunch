using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Validations.LessonValidators;

namespace EntreLaunch.Validations.CourseValidators
{
    public class PathCourseCreateValidator : AbstractValidator<PathCourseCreateDto>
    {
        public PathCourseCreateValidator(ILocalizationManager localizationManager)
        {
            // Name
            RuleFor(x => x.Name)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 500)
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            // Description
            RuleFor(x => x.Description)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 2500)
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));

            // PathId
            RuleFor(x => x.PathId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("PathIdGreaterThanZero"));

            // Logo
            RuleFor(x => x.Logo)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("LogoCannotBeEmpty"));

            RuleFor(x => x.Logo)
                .MustBeValidImage()
                .WithMessage(localizationManager.GetLocalizedString("LogoMustBeValidImage"));

            // Certificate validity
            When(x => x.CertificateExists, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });

            // Lists validation
            Action<IRuleBuilderInitial<PathCourseCreateDto, List<string>>> listRule = rule =>
                rule
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("ListCannotBeNull"))
                .Must(lst => lst.Count > 0)
                .WithMessage(localizationManager.GetLocalizedString("ListMustContainAtLeastOneItem"))
                .Must(lst => lst.TrueForAll(item => !string.IsNullOrWhiteSpace(item)))
                .WithMessage(localizationManager.GetLocalizedString("ListItemsCannotBeEmpty"));

            listRule(RuleFor(x => x.Audience));
            listRule(RuleFor(x => x.Requirements));
            listRule(RuleFor(x => x.Topics));
            listRule(RuleFor(x => x.Goals));
            listRule(RuleFor(x => x.Outcomes));

            // Lessons
            RuleFor(x => x.Lessons)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("LessonsCannotBeNull"));

            RuleFor(x => x.Lessons)
                .Must(lst => lst.Count > 0)
                .WithMessage(localizationManager.GetLocalizedString("LessonsMustContainAtLeastOneItem"))
                .ForEach(lesson => lesson.SetValidator(new LessonsCreateValidator(localizationManager)));
        }
    }
}
