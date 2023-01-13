using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getHostedPaymentPageRequest
    {
        public merchantAuthenticationType merchantAuthentication { get; set; }
        public string clientId { get; set; }
        public string refId { get; set; }
        public transactionRequestType transactionRequest { get; set; }
        public hostedPaymentSettings hostedPaymentSettings { get; set; }
    }
}
