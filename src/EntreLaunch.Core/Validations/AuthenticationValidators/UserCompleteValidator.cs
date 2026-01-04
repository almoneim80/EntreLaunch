using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Validations.AuthenticationValidators
{
    public class UserCompleteValidator : AbstractValidator<CompleteUserDetailsDto>
    {
        public UserCompleteValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.AvatarUrl).MustNotBeDefault();
            RuleFor(x => x.AvatarUrl).MustBeValidImage();
            RuleFor(x => x.AvatarUrl).MustBeValidImage();

            RuleFor(x => x.DOB).MustNotBeDefault();
            RuleFor(x => x.DOB).MustBeValidDate(mustBePast: true);
            RuleFor(x => x.DOB).MustBeValidDate(minDate: DateHelper.UtcNow.AddYears(-100), maxDate: DateHelper.UtcNow.AddYears(-18))
                               .WithMessage(localizationManager!.GetLocalizedString("DateOfBirthMustBeBetween"));

            RuleFor(x => x.Description).MustNotBeDefault();
            RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

            RuleFor(x => x.Specialization).MustNotBeDefault();
            RuleFor(x => x.Specialization).MustContainOnlyLettersAndSpaces();

            RuleFor(x => x.CountryCode).MustNotBeDefault();
        }
    }
}
