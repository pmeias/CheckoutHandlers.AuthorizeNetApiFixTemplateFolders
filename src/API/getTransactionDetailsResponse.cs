using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getTransactionDetailsResponse
    {
        public transactionDetailsType transaction { get; set; } = new();
        public messagesType messages { get; set; } = new();
    }
}
