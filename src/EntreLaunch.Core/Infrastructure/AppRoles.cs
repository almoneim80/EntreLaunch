namespace EntreLaunch.Infrastructure
{
    public static class AppRoles
    {
        // Grouped roles
        public const string SuperAdmin = Admin;

        public const string AllAdmins = Admin + "," + SubAdmin;

        public const string UserRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string PaymentRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string BlogRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string ClubRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string WheelRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string MyFinancingRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string MyOpportunityRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string MyPartnerRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string MyTeamRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string OpportunityRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string LoyaltyPointsRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string RefundRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string PurchaseRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string MyCommunityRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string SubscriptionRoles = Admin + "," + SubAdmin + "," + Entrepreneur;

        public const string TrainingRoles = Admin + "," + SubAdmin + "," + Trainer + "," + Student;

        public const string TrainingPathRoles = Admin + "," + SubAdmin + "," + Trainer + "," + Student + "," + Entrepreneur;

        public const string ConsultationRoles = Admin + "," + SubAdmin + "," + Entrepreneur + "," + Counselor;

        public const string SimulationRoles = Admin + "," + SubAdmin + "," + Trainer + "," + Student + "," + Guest;

        public const string AllUsersExceptSuperAdmin = User + "," + Entrepreneur + "," + Counselor + "," + Guest + "," + Trainer + "," + Student;

        public const string AllRoles = SubAdmin + "," + Entrepreneur + "," + User + "," + Entrepreneur + "," + Counselor + "," + Guest + "," + Trainer + "," + Student;

        public const string Admin = "Admin";
        public const string User = "User";
        public const string Entrepreneur = "Entrepreneur";
        public const string Counselor = "Counselor";
        public const string Guest = "Guest";
        public const string Trainer = "Trainer";
        public const string Student = "Student";
        public const string SubAdmin = "SubAdmin";

        public static readonly string[] All = [Admin, User, Entrepreneur, Counselor, Guest, Trainer, Student, SubAdmin];
    }
}
