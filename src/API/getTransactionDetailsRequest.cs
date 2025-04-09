using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getTransactionDetailsRequest
    {
        public merchantAuthenticationType merchantAuthentication { get; set; } = new();
        public string clientId { get; set; } = "";
        public string refId { get; set; } = "";
        public string transrefId { get; set; } = "";
    }
}
