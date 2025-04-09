namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class splitTenderPayment
    {
        public string transId { get; set; } = "";
        public string responseCode { get; set; } = "";
        public string responseToCustomer { get; set; } = "";
        public string authCode { get; set; } = "";
        public string accountNumber { get; set; } = "";
        public string accountType { get; set; } = "";
        public string requestedAmount { get; set; } = "";
        public string approvedAmount { get; set; } = "";
        public string balanceOnCard { get; set; } = "";
    }
}
