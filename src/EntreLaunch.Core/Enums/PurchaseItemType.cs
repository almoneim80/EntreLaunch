namespace EntreLaunch.Enums
{
    public enum PurchaseItemType
    {
        [Description("CertificateShipping")]
        CertificateShipping = 0,

        [Description("OnlineCourse")]
        OnlineCourse = 1,

        [Description("SkillsLibCourse")]
        SkillsLibCourse = 2,

        [Description("OnlineConsultation")]
        OnlineConsultation = 3,

        [Description("TextConsultation")]
        TextConsultation = 4,

        [Description("SpinWheelRetry")]
        SpinWheelRetry = 5,

        [Description("TrainingPath")]
        TrainingPath = 6,
    }
}
