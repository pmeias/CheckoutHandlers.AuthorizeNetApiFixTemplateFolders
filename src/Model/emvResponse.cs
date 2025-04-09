namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class emvResponse
    {
        public string tlvData { get; set; } = "";
        public emvTag tags { get; set; } = new();
    }
}
