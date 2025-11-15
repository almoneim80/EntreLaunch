namespace EntreLaunch.Entities
{
    /// <summary>
    /// Class OpportunityRequest.
    /// </summary>
    public class OpportunityRequest : SharedData
    {
        public int OpportunityId { get; set; }
        public string? City { get; set; }
        public double? ShareCapital { get; set; }
        public decimal? LoanRatio { get; set; }
        public int ManagementExperince { get; set; }
        public bool HaveFranchiseProjects { get; set; }
        public int FranchiseExperince { get; set; }
        public bool FeasibillityStudyBring { get; set; }
        public string userId { get; set; } = null!;
        public OpportunityType Type { get; set; }
        public OpportunityRequestStatus Status { get; set; } = OpportunityRequestStatus.Pending;

        public virtual Opportunity Opportunity { get; set; } = null!;
        public virtual User user { get; set; } = null!;
    }
}
