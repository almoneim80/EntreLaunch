using FluentValidation;

namespace EntreLaunch.Validations;

public class WheelPlayerCreateValidator : AbstractValidator<WheelPlayerCreateDto>
{
    public WheelPlayerCreateValidator()
    {
        RuleFor(x => x.PlayerId).MustNotBeDefault();

        RuleFor(x => x.AwardId).MustNotBeDefault();

        RuleFor(x => x.PlayedAt).MustNotBeDefault();
        RuleFor(x => x.PlayedAt).MustBeValidDate();
    }
}
