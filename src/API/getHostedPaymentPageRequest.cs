using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getHostedPaymentPageRequest
    {
        public merchantAuthenticationType merchantAuthentication { get; set; } = new();
        public string clientId { get; set; } = "";
        public string refId { get; set; } = "";
        public transactionRequestType transactionRequest { get; set; } = new();
        public hostedPaymentSettings hostedPaymentSettings { get; set; } = new();
    }
}
