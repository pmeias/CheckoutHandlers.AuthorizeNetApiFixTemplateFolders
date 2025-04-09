using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;
using System.Collections.Generic;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API
{
    internal class transactionResponse
    {
        public string responseCode { get; set; } = "";
        public string rawResponseCode { get; set; } = "";
        public string authCode { get; set; } = "";
        public string avsResultCode { get; set; } = "";
        public string cvvResultCode { get; set; } = "";
        public string cavvResultCode { get; set; } = "";
        public string transId { get; set; } = "";
        public string refTransID { get; set; } = "";
        public string transHash { get; set; } = "";
        public string testRequest { get; set; } = "";
        public string accountNumber { get; set; } = "";
        public string entryMode { get; set; } = "";
        public string accountType { get; set; } = "";
        public string splitTenderId { get; set; } = "";
        public prePaidCard prePaidCard { get; set; } = new();
        public List<Message> messages { get; set; } = [];
        public List<Error> errors { get; set; } = [];
        public List<splitTenderPayment> splitTenderPayments { get; set; } = [];
        public List<userField> userFields { get; set; } = [];
        public nameAndAddressType shipTo { get; set; } = new();
        public secureAcceptance secureAcceptance { get; set; } = new();
        public emvResponse emvResponse { get; set; } = new();
        public string transHashSha2 { get; set; } = "";
        public customerProfileIdType profile { get; set; } = new();
        public string networkTransId { get; set; } = "";
    }
}
