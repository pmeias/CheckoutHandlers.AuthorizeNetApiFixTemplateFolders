using System;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class ReturnedItem
    {
        public string id { get; set; }
        public DateTime dateUTC { get; set; }
        public DateTime dateLocal { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }
}
