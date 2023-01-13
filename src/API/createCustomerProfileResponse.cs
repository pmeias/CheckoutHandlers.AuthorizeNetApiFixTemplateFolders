using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;
using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class createCustomerProfileResponse
    {
        public messagesType messages { get; set; }
        public string customerProfileId { get; set; }
        public List<string> customerPaymentProfileIdList { get; set; }
    }
}
