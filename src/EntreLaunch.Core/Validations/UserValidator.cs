namespace EntreLaunch.Validations;

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

        //RuleFor(x => x.PhoneNumber).MustNotBeDefault();
        //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required").Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Password).MustNotBeDefault();

        RuleFor(x => x.ConfirmPassword).MustNotBeDefault();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationManager!.GetLocalizedString("PasswordMismatch"));
    }
}

public class UserCompleteValidator : AbstractValidator<CompleteUserDetailsDto>
{
    public UserCompleteValidator(ILocalizationManager localizationManager)
    {
        RuleFor(x => x.AvatarUrl).MustNotBeDefault();
        RuleFor(x => x.AvatarUrl).MustBeValidImage();
        RuleFor(x => x.AvatarUrl).MustBeValidImage();

        RuleFor(x => x.DOB).MustNotBeDefault();
        RuleFor(x => x.DOB).MustBeValidDate(mustBePast: true);
        RuleFor(x => x.DOB).MustBeValidDate(minDate: DateTime.UtcNow.AddYears(-100), maxDate: DateTime.UtcNow.AddYears(-18))
                           .WithMessage(localizationManager!.GetLocalizedString("DateOfBirthMustBeBetween"));

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.Specialization).MustNotBeDefault();
        RuleFor(x => x.Specialization).MustContainOnlyLettersAndSpaces();

        RuleFor(x => x.CountryCode).MustNotBeDefault();
    }
}

public class UserEntreLaunchdateValidator : AbstractValidator<UserEntreLaunchdateDto>
{
    private readonly ILocalizationManager? _localization;
    public UserEntreLaunchdateValidator(ILocalizationManager localizationManager)
    {
        _localization = localizationManager;
    }
    public UserEntreLaunchdateValidator()
    {
        RuleFor(x => x.FirstName).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.FirstName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.LastName).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.LastName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.PhoneNumber).MustBeValidPhoneNumber(7, 15);

        RuleFor(x => x.AvatarUrl).MustBeValidAttachment();
        RuleFor(x => x.AvatarUrl).MustBeValidImage();

        RuleFor(x => x.DOB).MustBeValidDate(mustBePast: true);
        RuleFor(x => x.DOB).MustBeValidDate(minDate: DateTime.UtcNow.AddYears(-100), maxDate: DateTime.UtcNow.AddYears(-18))
                           .WithMessage(_localization!.GetLocalizedString("DateOfBirthMustBeBetween"));

        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.Specialization).MustContainOnlyLettersAndSpaces();
    }
}

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

        RuleFor(x => x.NewPassword).MustNotBeDefault();
    }
}
