namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class prePaidCard
    {
        public string requestedAmount { get; set; }
        public string approvedAmount { get; set; }
        public string balanceOnCard { get; set; }
    }
}
