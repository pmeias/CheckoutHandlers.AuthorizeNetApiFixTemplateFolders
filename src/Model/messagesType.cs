using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class messagesType
    {
        public string resultCode { get; set; }
        public List<message> message { get; set; }
    }
}
