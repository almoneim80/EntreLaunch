namespace EntreLaunch.Interfaces.ConsultationIntf
{
    public interface IConsultation :
        ICounselorService,
        IConsultationBookingService,
        ITicketService
    {
        // No members here. This interface combines all operations for convenience in older layers or legacy services.
    }
}
