namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class tokenMaskedType
    {
        public string tokenSource { get; set; } = "";
        public string tokenNumber { get; set; } = "";
        public string expirationDate { get; set; } = "";
        public string tokenRequestorId { get; set; } = "";
    }
}
