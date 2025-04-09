using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class lineItems
    {
        public List<lineItem> lineItem { get; set; } = [];
    }
}
