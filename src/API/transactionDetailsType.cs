using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;
using System;
using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class transactionDetailsType
    {
        public string transId { get; set; }
        public string refTransId { get; set; }
        public string splitTenderId { get; set; }
        public DateTime submitTimeUTC { get; set; }
        public DateTime submitTimeLocal { get; set; }
        public transactionTypeEnum transactionType { get; set; }
        public transactionStatusEnum transactionStatus { get; set; }
        public int responseCode { get; set; }
        public int responseReasonCode { get; set; }
        public subscriptionPaymentType subscription { get; set; }
        public string responseReasonDescription { get; set; }
        public string authCode { get; set; }
        public string AVSResponse { get; set; }
        public string cardCodeResponse { get; set; }
        public string CAVVResponse { get; set; }
        public FDSFilterActionEnum FDSFilterAction { get; set; }
        public List<FDSFilter> FDSFilters { get; set; }
        public batchDetailsType batch { get; set; }
        public orderExType order { get; set; }
        public decimal requestedAmount { get; set; }
        public decimal authAmount { get; set; }
        public decimal settleAmount { get; set; }
        public extendedAmountType tax { get; set; }
        public extendedAmountType shipping { get; set; }
        public extendedAmountType duty { get; set; }
        public List<lineItem> lineItems { get; set; }
        public decimal prepaidBalanceRemaining { get; set; }
        public bool taxExempt { get; set; }
        public paymentMaskedType payment { get; set; }
        public customerDataType customer { get; set; }
        public customerAddressType billTo { get; set; }
        public nameAndAddressType shipTo { get; set; }
        public bool recurringBilling { get; set; }
        public string customerIP { get; set; }
        public string product { get; set; }
        public string entryMode { get; set; }
        public string marketType { get; set; }
        public string mobileDeviceId { get; set; }
        public string customerSignature { get; set; }
        public List<ReturnedItem> returnedItems { get; set; }
        public solutionType solution { get; set; }
        public customerProfileIdType profile { get; set; }
        public extendedAmountType surcharge { get; set; }
        public string employeeId { get; set; }
        public extendedAmountType tip { get; set; }
        public otherTaxType otherTax { get; set; }
        public nameAndAddressType shipFrom { get; set; }
        public string networkTransId { get; set; }
        public string originalNetworkTransId { get; set; }
        public decimal originalAuthAmount { get; set; }
        public string authorizationIndicator { get; set; }

    }
}
