namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class paymentMaskedType
    {
        public creditCardMaskedType creditCard { get; set; }
        public bankAccountMaskedType bankAccount { get; set; }
        public tokenMaskedType tokenInformation { get; set; }
    }
}
