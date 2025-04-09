namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class paymentMaskedType
    {
        public creditCardMaskedType creditCard { get; set; } = new();
        public bankAccountMaskedType bankAccount { get; set; } = new();
        public tokenMaskedType tokenInformation { get; set; } = new();
    }
}
