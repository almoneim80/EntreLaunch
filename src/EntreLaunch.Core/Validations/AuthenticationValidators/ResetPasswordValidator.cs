namespace EntreLaunch.Validations.AuthenticationValidators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        private readonly ILocalizationManager? _localization;
        public ResetPasswordValidator(ILocalizationManager localizationManager)
        {
            _localization = localizationManager;
        }

        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email).MustNotBeDefault();
            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage(_localization!.GetLocalizedString("InvalidEmail"));

            RuleFor(x => x.NewPassword).MustNotBeDefault()
            .MinimumLength(6).WithMessage(_localization.GetLocalizedString("PasswordMinLength"));

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(_localization.GetLocalizedString("TokenRequired"));
        }
    }
}
