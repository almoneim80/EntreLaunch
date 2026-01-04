namespace EntreLaunch.Interfaces.PaymentIntf
{
    public interface IDiscountService
    {
        /// <summary>
        /// check if discount code is valid or not.
        /// </summary>
        Task<bool> ValidateDiscountAsync(string discountCode);

        /// <summary>
        /// Calculate discount amount based on original amount and discount code.
        /// </summary>
        Task<decimal> CalculateDiscountAmountAsync(decimal originalAmount, string discountCode);

        /// <summary>
        /// Apply discount to payment object.
        /// </summary>
        Task ApplyDiscountAsync(Payment payment, string discountCode);
    }
}
