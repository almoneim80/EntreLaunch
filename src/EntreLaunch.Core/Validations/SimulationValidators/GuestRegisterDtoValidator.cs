using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class GuestRegisterDtoValidator : AbstractValidator<GuestRegisterDto>
    {
        public GuestRegisterDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault()
                .WithMessage(localization.GetLocalizedString("GuestNameRequired"));

            RuleFor(x => x.Name)
                .MustContainOnlyLettersAndSpaces()
                .WithMessage(localization.GetLocalizedString("GuestNameLettersOnly"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(2, 100)
                .WithMessage(localization.GetLocalizedString("GuestNameLength"));

            RuleFor(x => x.PhoneNumber)
                .MustNotBeDefault()
                .WithMessage(localization.GetLocalizedString("GuestPhoneRequired"));

            RuleFor(x => x.PhoneNumber)
                .MustBeValidPhoneNumber(8, 15)
                .WithMessage(localization.GetLocalizedString("GuestPhoneInvalid"));

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(localization.GetLocalizedString("GuestEmailInvalid"));
        }
    }
}
