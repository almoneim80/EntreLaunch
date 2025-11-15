namespace EntreLaunch.Validations;
public class ClubValidator : AbstractValidator<ClubEventCreateDto>
{
    public ClubValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.City).MustNotBeDefault();
        RuleFor(x => x.City).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.City).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2500, allowSpecialCharacters: true);

        RuleFor(x => x.StartDate).MustNotBeDefault();
        RuleFor(x => (DateTimeOffset?)x.StartDate).MustBeValidDate(mustBeFuture: true);
        RuleFor(x => x.StartDate)
            .MustBeEarlierThan(x => x.EndDate, x => x.EndDate.DateTime.ToShortDateString()
            ?? "Unknown End Date");

        RuleFor(x => x.EndDate).MustNotBeDefault();
        RuleFor(x => (DateTimeOffset?)x.EndDate).MustBeValidDate(mustBeFuture: true);
    }
}

public class ClubEventEntreLaunchdateValidator : AbstractValidator<ClubEventEntreLaunchdateDto>
{
    public ClubEventEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.City).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.City).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2500, allowSpecialCharacters: true);

        //RuleFor(x => x.StartDate).MustBeValidDate(mustBeFuture: true);
        //RuleFor(x => x.StartDate).MustBeEarlierThan(x => x.EndDate, x => x.EndDate?.DateTime.ToShortDateString() ?? "Unknown End Date");

        //RuleFor(x => x.EndDate).MustBeValidDate(mustBeFuture: true);
    }
}

public class ClubEventSubscriberCreateValidator : AbstractValidator<ClubSubscribeCreateDto>
{
    public ClubEventSubscriberCreateValidator()
    {
        RuleFor(x => x.EventId).MustNotBeDefault();
    }
}
