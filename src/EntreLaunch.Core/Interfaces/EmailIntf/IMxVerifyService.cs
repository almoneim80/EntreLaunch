namespace EntreLaunch.Interfaces.EmailIntf
{
    public interface IMxVerifyService
    {
        /// <summary>
        /// Verify a mx value.
        /// </summary>
        Task<bool> Verify(string mxValue);
    }
}
