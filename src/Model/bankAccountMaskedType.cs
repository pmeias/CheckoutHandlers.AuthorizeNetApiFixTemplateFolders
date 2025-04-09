using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class bankAccountMaskedType
    {
        public bankAccountTypeEnum accountType { get; set; }
        public string routingNumber { get; set; } = "";
        public string accountNumber { get; set; } = "";
        public string nameOnAccount { get; set; } = "";
        public echeckTypeEnum echeckType { get; set; }
        public string bankName { get; set; } = "";
    }
}
