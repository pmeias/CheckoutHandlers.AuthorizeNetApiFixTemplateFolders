using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class customerPaymentProfileBaseType
    {
        public customerTypeEnum customerType { get; set; }
        public customerAddressType billTo { get; set; } = new();
    }
}
