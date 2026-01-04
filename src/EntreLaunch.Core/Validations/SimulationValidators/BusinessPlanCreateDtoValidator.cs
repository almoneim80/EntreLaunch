using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class BusinessPlanCreateDtoValidator : AbstractValidator<BusinessPlanCreateDto>
    {
        public BusinessPlanCreateDtoValidator(ILocalizationManager localization)
        {
            RuleForEachList(x => x.BusinessPartners, "BusinessPartnersEmpty", localization);
            RuleForEachList(x => x.ProjectActivities, "ProjectActivitiesEmpty", localization);
            RuleForEachList(x => x.ValueProposition, "ValuePropositionEmpty", localization);
            RuleForEachList(x => x.CustomerRelationships, "CustomerRelationshipsEmpty", localization);
            RuleForEachList(x => x.CustomerSegments, "CustomerSegmentsEmpty", localization);
            RuleForEachList(x => x.RequiredResources, "RequiredResourcesEmpty", localization);
            RuleForEachList(x => x.DistributionChannels, "DistributionChannelsEmpty", localization);
            RuleForEachList(x => x.RevenueStreams, "RevenueStreamsEmpty", localization);
            RuleForEachList(x => x.CostStructure, "CostStructureEmpty", localization);
        }

        private void RuleForEachList(Expression<Func<BusinessPlanCreateDto, List<string>?>> property, string messageKey, ILocalizationManager localization)
        {
            RuleFor(property)
                .Must(list => list == null || list.TrueForAll(item => !string.IsNullOrWhiteSpace(item)))
                .WithMessage(localization.GetLocalizedString(messageKey));
        }
    }
}
