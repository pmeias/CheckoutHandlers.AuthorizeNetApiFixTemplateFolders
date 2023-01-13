using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getCustomerProfileResponse
    {
        public messagesType messages { get; set; }
        public customerProfileMaskedType profile { get; set; }
    }
}
