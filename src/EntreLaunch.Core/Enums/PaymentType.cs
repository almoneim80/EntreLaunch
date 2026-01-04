namespace EntreLaunch.Enums
{
    public enum PaymentType
    {
        // monthly payment without TargetId 
        [Description("MyTeam")]
        MyTeam = 1,

        [Description("MyFinance")]
        MyFinance = 2,

        [Description("MyPartner")]
        MyPartner = 3,

        [Description("MyOpportunity")]
        MyOpportunity = 4,

        [Description("Club")]
        Club = 5,

        // day payment without TargetId 
        [Description("TextConsultation")]
        TextConsultation = 6,

        [Description("SpinWheelRetry")]
        SpinWheelRetry = 7,

        // day payment with TargetId and no re-payment with no payment for same TargetId
        [Description("TrainingPath")]
        TrainingPath = 8,

        [Description("OnlineConsultation")]
        OnlineConsultation = 9,

        [Description("OnlineCourse")]
        OnlineCourse = 10,

        [Description("SkillsLibCourse")]
        SkillsLibCourse = 11,

        // other
        [Description("CertificateShipping")]
        CertificateShipping = 12,
    }
}
