using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class BatchStatisticType
    {
        public accountTypeEnum accountType { get; set; }
        public decimal chargeAmount { get; set; }
        public int chargeCount { get; set; }
        public decimal refundAmount { get; set; }
        public int refundCount { get; set; }
        public int voidCount { get; set; }
        public int declineCount { get; set; }
        public int errorCount { get; set; }
        public decimal returnedItemAmount { get; set; }
        public int returnedItemCount { get; set; }
        public decimal chargebackAmount { get; set; }
        public int chargebackCount { get; set; }
        public int correctionNoticeCount { get; set; }
        public decimal chargeChargeBackAmount { get; set; }
        public int chargeChargeBackCount { get; set; }
        public decimal refundChargeBackAmount { get; set; }
        public int refundChargeBackCount { get; set; }
        public decimal chargeReturnedItemsAmount { get; set; }
        public int chargeReturnedItemsCount { get; set; }
        public decimal refundReturnedItemsAmount { get; set; }
        public int refundReturnedItemsCount { get; set; }
    }
}
