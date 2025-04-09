using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;
using System;
using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    internal class batchDetailsType
    {
        public string batchId { get; set; } = "";
        public DateTime settlementTimeUTC { get; set; }
        public DateTime settlementTimeLocal { get; set; }
        public settlementStateEnum settlementState { get; set; }
        public paymentMethodEnum paymentMethod { get; set; }
        public string marketType { get; set; } = "";
        public string product { get; set; } = "";
        public List<BatchStatisticType> statistics { get; set; } = [];
    }
}
