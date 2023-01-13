using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class getCustomerProfileRequest
    {
        public merchantAuthenticationType merchantAuthentication { get; set; }
        public string clientId { get; set; }
        public string refId { get; set; }
        public string merchantCustomerId { get; set; }
    }
}
