namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerFilteringService
    {
        /// <summary>
        /// Filter Projects.
        /// </summary>
        Task<GeneralResult> Filtering([FromBody] FilterProjectsDto filter);
    }
}
