using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getHostedPaymentPageResponse
    {
        public string token { get; set; } = "";
        public messagesType messages { get; set; } = new();
    }
}
