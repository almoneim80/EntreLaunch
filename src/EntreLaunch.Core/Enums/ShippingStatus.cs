namespace EntreLaunch.Enums
{
    public enum ShippingStatus
    {
        [Description("Not Required")]
        NotRequired = 0, // مثلاً إذا كانت الشهادة إلكترونية فقط

        [Description("Pending")]
        Pending = 1, // تمت جدولة الشحن، لكن لم يتم بعد

        [Description("In Transit")]
        InTransit = 2, // الشهادة قيد التوصيل

        [Description("Delivered")]
        Delivered = 3, // تم توصيل الشهادة بنجاح

        [Description("Failed")]
        Failed = 4, // فشل في الشحن (مثل خطأ في العنوان)
    }
}
