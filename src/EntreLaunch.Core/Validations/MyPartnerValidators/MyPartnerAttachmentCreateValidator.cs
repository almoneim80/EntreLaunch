using EntreLaunch.DTOs.MyPartnerDtos;

namespace EntreLaunch.Validations.MyPartnerValidators
{
    public class MyPartnerAttachmentCreateValidator : AbstractValidator<MyPartnerAttachmentCreateDto>
    {
        public MyPartnerAttachmentCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Url)
                .MustNotBeDefault();

            RuleFor(x => x.Url)
               .MustBeValidAttachment()
                .WithMessage(localizationManager.GetLocalizedString("AttachmentUrlInvalid"));
        }
    }
}
