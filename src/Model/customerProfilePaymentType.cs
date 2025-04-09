namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class customerProfilePaymentType
    {
        public string customerProfileId { get; set; } = "";
        public paymentProfile paymentProfile { get; set; } = new();
    }
}
