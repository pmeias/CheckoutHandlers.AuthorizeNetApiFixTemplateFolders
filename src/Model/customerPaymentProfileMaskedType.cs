using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;
using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class customerPaymentProfileMaskedType
    {
        public customerTypeEnum customerType { get; set; }
        public customerAddressType billTo { get; set; } = new();
        public string customerProfileId { get; set; } = "";
        public string customerPaymentProfileId { get; set; } = "";
        public bool defaultPaymentProfile { get; set; }
        public paymentMaskedType payment { get; set; } = new();
        public driversLicenseMaskedType driversLicense { get; set; } = new();
        public string taxId { get; set; } = "";
        public List<string> subscriptionIds { get; set; } = [];
        public string originalNetworkTransId { get; set; } = "";
        public decimal originalAuthAmount { get; set; }
    }
}
