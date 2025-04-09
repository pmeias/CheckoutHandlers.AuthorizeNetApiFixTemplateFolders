using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class createTransactionResponse
    {
        public transactionResponse transactionResponse { get; set; } = new();
        public messagesType messages { get; set; } = new();
    }
}
