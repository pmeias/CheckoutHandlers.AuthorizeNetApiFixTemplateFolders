using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class creditCardMaskedType
    {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public cardTypeEnum cardType { get; set; }
        public cardArt cardArt { get; set; }
        public string issuerNumber { get; set; }
        public bool isPaymentToken { get; set; }
    }
}
