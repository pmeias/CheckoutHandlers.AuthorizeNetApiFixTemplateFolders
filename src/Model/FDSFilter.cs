using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class FDSFilter
    {
        public string name { get; set; }
        public FDSFilterActionEnum action { get; set; }
    }
}
