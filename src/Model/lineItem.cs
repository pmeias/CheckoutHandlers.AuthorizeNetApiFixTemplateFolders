namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class lineItem
    {
        public string itemId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal quantity { get; set; }
        public decimal unitPrice { get; set; }
    }
}
