using EntreLaunch.DTOs.MyPartnerDtos;

namespace EntreLaunch.Validations.MyPartnerValidators
{
    public class MyPartnerUpdateValidator : AbstractValidator<MyPartnerUpdateDto>
    {
        public MyPartnerUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Activity)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Activity))
                .WithMessage(localizationManager.GetLocalizedString("ActivityCannotBeEmpty"));

            RuleFor(x => x.City)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityCannotBeEmpty"));

            RuleFor(x => x.Sector)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Sector))
                .WithMessage(localizationManager.GetLocalizedString("SectorCannotBeEmpty"));

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0).When(x => x.Cost.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CostMustBeNonNegative"));

            RuleFor(x => x.Idea)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Idea))
                .WithMessage(localizationManager.GetLocalizedString("IdeaCannotBeEmpty"));

            RuleFor(x => x.AcceptRequirements)
                .Must(lst => lst != null && lst.Any()).When(x => x.AcceptRequirements != null)
                .WithMessage(localizationManager.GetLocalizedString("AcceptRequirementsNotEmpty"));

            RuleFor(x => x.AcceptRequirements)
                .Must(lst => lst != null && lst.TrueForAll(item => !string.IsNullOrWhiteSpace(item)))
                .WithMessage(localizationManager.GetLocalizedString("AcceptRequirementsItemCannotBeEmpty"));

            RuleFor(x => x.CapitalFrom)
                .GreaterThanOrEqualTo(0).When(x => x.CapitalFrom.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CapitalFromNonNegative"));

            RuleFor(x => x.CapitalTo)
                .GreaterThanOrEqualTo(0).When(x => x.CapitalTo.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("CapitalToNonNegative"));

            RuleFor(x => x)
                .Must(dto =>
                    !dto.CapitalFrom.HasValue ||
                    !dto.CapitalTo.HasValue ||
                    dto.CapitalTo.Value >= dto.CapitalFrom.Value)
                .WithMessage(localizationManager.GetLocalizedString("CapitalRangeInvalid"));

            RuleFor(x => x.Contact)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Contact))
                .WithMessage(localizationManager.GetLocalizedString("ContactCannotBeEmpty"));
        }
    }
}
