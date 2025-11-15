using FluentValidation;
namespace EntreLaunch.Validations;

public class ConsultationCreateValidator : AbstractValidator<ConsultationCreateDto>
{
    public ConsultationCreateValidator()
    {
        RuleFor(x => x.CounselorId).MustNotBeDefault();

        RuleFor(x => x.ClientId).MustNotBeDefault();

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);
    }
}

public class ConsultationEntreLaunchdateValidator : AbstractValidator<ConsultationEntreLaunchdateDto>
{
    public ConsultationEntreLaunchdateValidator()
    {
        RuleFor(x => x.AdvisorId).MustNotBeDefault();

        RuleFor(x => x.ClientId).MustNotBeDefault();

        //RuleFor(x => x.ConsultationTimeId).MustNotBeDefault();

        //RuleFor(x => x.Type).MustHaveLengthInRange(2, 250);
        //RuleFor(x => x.Type).MustContainOnlyLettersAndSpaces();

        //RuleFor(x => x.Type).MustHaveLengthInRange(2, 250);
        //RuleFor(x => x.Type).MustContainOnlyLettersAndSpaces();

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);
    }
}

public class TicketAttachmentCreateValidator : AbstractValidator<TicketAttachmentCreateDto>
{
    public TicketAttachmentCreateValidator()
    {
        RuleFor(x => x.TicketId).MustNotBeDefault();

        RuleFor(x => x.SenderId).MustNotBeDefault();

        RuleFor(x => x.Url).MustNotBeDefault();
        RuleFor(x => x.Url).MustBeValidAttachment();
        RuleFor(x => x.Url).MustBeValidDocument();

        RuleFor(x => x.SendTime).MustNotBeDefault();
        RuleFor(x => x.SendTime).MustBeValidDate();
    }
}

public class ConsultationTimeCreateValidator : AbstractValidator<ConsultationTimeCreateDto>
{
    public ConsultationTimeCreateValidator()
    {
        RuleFor(x => x.CounselorId).MustNotBeDefault();

        RuleFor(x => x.DateTimeSlot).MustNotBeDefault();
    }
}

public class TicketMessageCreateValidator : AbstractValidator<TicketMessageCreateDto>
{
    public TicketMessageCreateValidator()
    {
        RuleFor(x => x.TicketId).MustNotBeDefault();

        RuleFor(x => x.SenderId).MustNotBeDefault();

        RuleFor(x => x.Content).MustNotBeDefault();
        RuleFor(x => x.Content).MustHaveLengthInRange(1, 1000);
    }
}

public class TicketMessageEntreLaunchdateValidator : AbstractValidator<TicketMessageEntreLaunchdateDto>
{
    public TicketMessageEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Content).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Content).MustHaveLengthInRange(1, 1000);
    }
}

public class TicketCreateValidator : AbstractValidator<TicketCreateDto>
{
    public TicketCreateValidator()
    {
        RuleFor(x => x.CreatorId).MustNotBeDefault();
        RuleFor(x => x.ConsultationId).MustNotBeDefault();
    }
}

public class CounselorCreateValidator : AbstractValidator<CreateCounselorRequestDto>
{
    public CounselorCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.Qualification).MustNotBeDefault();
        RuleFor(x => x.Qualification).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.City).MustNotBeDefault();
        RuleFor(x => x.City).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.SpecializationExperience).MustNotBeDefault();

        RuleFor(x => x.ConsultingExperience).MustNotBeDefault();

        RuleFor(x => x.DailyHours).MustNotBeDefault();

        RuleFor(x => x.SocialMediaAccounts).MustNotBeDefault();
    }
}
