using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Validations.AuthenticationValidators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDto>
    {
        private readonly ILocalizationManager? _localization;
        public UserUpdateValidator(ILocalizationManager localizationManager)
        {
            _localization = localizationManager;
        }

        public UserUpdateValidator()
        {
            RuleFor(x => x.FirstName).MustContainOnlyLettersAndSpaces();
            RuleFor(x => x.FirstName).MustHaveLengthInRange(2, 250);

            RuleFor(x => x.LastName).MustContainOnlyLettersAndSpaces();
            RuleFor(x => x.LastName).MustHaveLengthInRange(2, 250);

            RuleFor(x => x.PhoneNumber).MustBeValidPhoneNumber(7, 15);

            RuleFor(x => x.AvatarUrl).MustBeValidAttachment();
            RuleFor(x => x.AvatarUrl).MustBeValidImage();

            RuleFor(x => x.DOB).MustBeValidDate(mustBePast: true);
            RuleFor(x => x.DOB).MustBeValidDate(minDate: DateHelper.UtcNow.AddYears(-100), maxDate: DateHelper.UtcNow.AddYears(-18))
                               .WithMessage(_localization!.GetLocalizedString("DateOfBirthMustBeBetween"));

            RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

            RuleFor(x => x.Specialization).MustContainOnlyLettersAndSpaces();
        }
    }
}
