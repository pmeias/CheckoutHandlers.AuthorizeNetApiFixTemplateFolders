using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class deleteCustomerPaymentProfileRequest
    {
        public merchantAuthenticationType merchantAuthentication { get; set; }
        public string clientId { get; set; }
        public string refId { get; set; }
        public string customerProfileId { get; set; }
        public string customerPaymentProfileId { get; set; }
    }
}
