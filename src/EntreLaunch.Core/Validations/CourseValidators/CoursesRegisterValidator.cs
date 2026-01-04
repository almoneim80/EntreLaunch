using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.CourseValidators
{
    public class CoursesRegisterValidator : AbstractValidator<CoursesRegisterDto>
    {
        public CoursesRegisterValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CourseIdMustBePositive"));

            RuleFor(x => x.FirstName)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("FirstNameRequired"));

            RuleFor(x => x.FirstName)
                .MustHaveLengthInRange(2, 100)
                .WithMessage(localizationManager.GetLocalizedString("FirstNameLengthRange"));

            RuleFor(x => x.LastName)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("LastNameRequired"));

            RuleFor(x => x.LastName)
                .MustHaveLengthInRange(2, 100)
                .WithMessage(localizationManager.GetLocalizedString("LastNameLengthRange"));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationManager.GetLocalizedString("EmailRequired"));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationManager.GetLocalizedString("InvalidEmailFormat"));

            RuleFor(x => x.EnrolledAt)
                .MustBeValidDate(mustBePast: true)
                .WithMessage(localizationManager.GetLocalizedString("EnrollmentDateMustBePast"));
        }
    }
}
