using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Validations.AuthenticationValidators
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.FirstName).MustNotBeDefault();
            RuleFor(x => x.FirstName).MustContainOnlyLettersAndSpaces();
            RuleFor(x => x.FirstName).MustHaveLengthInRange(2, 250);

            RuleFor(x => x.LastName).MustNotBeDefault();
            RuleFor(x => x.LastName).MustContainOnlyLettersAndSpaces();
            RuleFor(x => x.LastName).MustHaveLengthInRange(2, 250);

            RuleFor(x => x.Email).MustNotBeDefault();
            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage(localizationManager!.GetLocalizedString("InvalidEmail"));

            RuleFor(x => x.PhoneNumber).MustNotBeDefault();
            RuleFor(x => x.PhoneNumber).MustBeValidPhoneNumber(7, 15);

            RuleFor(x => x.Password).MustNotBeDefault();

            RuleFor(x => x.ConfirmPassword).MustNotBeDefault();
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationManager!.GetLocalizedString("PasswordMismatch"));
        }
    }
}
