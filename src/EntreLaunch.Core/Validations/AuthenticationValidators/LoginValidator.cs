using EntreLaunch.DTOs.AuthenticationDtos;

namespace EntreLaunch.Validations.AuthenticationValidators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizationManager.GetLocalizedString("EmailRequired"));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(localizationManager.GetLocalizedString("EmailInvalid"));

            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage(localizationManager!.GetLocalizedString("InvalidEmail"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizationManager.GetLocalizedString("PasswordRequired"));

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage(localizationManager.GetLocalizedString("PasswordMinLength"));
        }
    }
}
