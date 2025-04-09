namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class otherTaxType
    {
        public decimal nationalTaxAmount { get; set; }
        public decimal localTaxAmount { get; set; }
        public decimal alternateTaxAmount { get; set; }
        public string alternateTaxId { get; set; } = "";
        public decimal vatTaxRate { get; set; }
        public decimal vatTaxAmount { get; set; }
    }
}
