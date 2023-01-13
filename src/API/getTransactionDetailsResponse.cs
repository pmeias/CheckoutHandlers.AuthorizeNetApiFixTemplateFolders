using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getTransactionDetailsResponse
    {
        public transactionDetailsType transaction { get; set; }
        public messagesType messages { get; set; }
    }
}
