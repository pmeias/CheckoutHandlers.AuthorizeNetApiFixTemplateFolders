using Dynamicweb.Core;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;
using Dynamicweb.Ecommerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi
{
    internal static class Helper
    {
        public static string GetOrderError(Order order)
        {
            var errorText = string.Empty;
            if (order == null)
            {
                errorText = "Order is not set";
            }
            else if (string.IsNullOrEmpty(order.Id))
            {
                errorText = "Order id is not set";
            }
            else if (string.IsNullOrEmpty(order.TransactionNumber))
            {
                errorText = "Transaction number is not set";
            }

            return errorText;
        }

        public static void UpdateTransactionNumber(Order order, string? transactionId)
        {
            if (!string.IsNullOrEmpty(transactionId) && !transactionId.Equals(order.TransactionNumber, StringComparison.OrdinalIgnoreCase))
            {
                order.TransactionNumber = transactionId;
            }
        }

        public static string GetResponseTextByCode(string responseCode)
        {
            int theCode = Converter.ToInt32(responseCode);
            return GetResponseTextByCode(theCode);
        }

        public static string GetResponseTextByCode(int responseCode)
        {
            switch (responseCode)
            {
                case 1: return "Approved";
                case 2: return "Declined";
                case 3: return "Error";
                case 4: return "Held for Review";
                default: return "Unknown";
            }
        }

        public static string CreateErrorMessage(string prefix, createTransactionResponse response)
        {
            string codeText = string.Empty;
            if (response.transactionResponse is null)
                return codeText;

            string errorCode;
            string errorText;
            if (!string.IsNullOrEmpty(response.transactionResponse.responseCode))
            {
                codeText = GetResponseTextByCode(response.transactionResponse.responseCode);
            }
            if (response.transactionResponse.errors is not null)
            {
                errorCode = response.transactionResponse.errors[0].errorCode;
                errorText = response.transactionResponse.errors[0].errorText;
            }
            else
            {
                errorCode = response.messages.message[0].code;
                errorText = response.messages.message[0].text;
            }

            return $"{prefix} {errorCode}/{codeText} - {errorText}";
        }

        public static nameAndAddressType CreateShipAddress(Order order)
        {
            return new nameAndAddressType
            {
                firstName = CropString(order.DeliveryFirstName, 50),
                lastName = CropString(order.DeliverySurname, 50),
                company = CropString(order.DeliveryCompany, 50),
                address = CropString($"{order.DeliveryHouseNumber} {order.DeliveryAddress} {order.DeliveryAddress2}".Trim(), 60),
                city = CropString(order.DeliveryCity, 40),
                state = CropString(order.DeliveryRegion, 40),
                zip = CropString(order.DeliveryZip, 20),
                country = CropString(order.DeliveryCountry, 60),
            };
        }

        public static customerAddressType CreateBillAddress(Order order)
        {
            return new customerAddressType
            {
                firstName = CropString(order.CustomerFirstName, 50),
                lastName = CropString(order.CustomerSurname, 50),
                company = CropString(order.CustomerCompany, 50),
                address = CropString($"{order.CustomerHouseNumber} {order.CustomerAddress}".Trim(), 60),
                city = CropString(order.CustomerCity, 40),
                state = CropString(order.CustomerRegion, 40),
                zip = CropString(order.CustomerZip, 20),
                country = CropString(order.CustomerCountry, 60),
                email = order.CustomerEmail,
                phoneNumber = CropString(order.CustomerPhone, 25),
            };
        }

        public static lineItems CreateLineItems(Order order)
        {
            var result = new List<lineItem>();
            foreach (var line in order.OrderLines)
            {
                if (line.OrderLineType == OrderLineType.Product || line.OrderLineType == OrderLineType.Fixed)
                {
                    result.Add(new lineItem
                    {
                        itemId = line.Id,
                        name = CropString(line.Product.Name, 31),
                        description = CropString(line.Product.ShortDescription, 255),
                        quantity = Converter.ToDecimal(line.Quantity),
                        unitPrice = Converter.ToDecimal(Services.Currencies.Round(order.Currency, line.Price.Price)),
                    });
                }
            }
            return new lineItems { lineItem = result };
        }

        // As per documentation, there is a length limitation for several values. Need to crop them.
        public static string CropString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength);
        }

        public static bool IsValidHmac(string? signatureKey, string? notificationBody, string? incomingHmac)
        {
            if (string.IsNullOrEmpty(signatureKey) || string.IsNullOrEmpty(notificationBody) || string.IsNullOrEmpty(incomingHmac))
            {
                return false;
            }

            var token = GetSHAToken(signatureKey, notificationBody);
            return string.Equals(token, incomingHmac, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetSHAToken(string signatureKey, string notificationBody)
        {
            try
            {
                byte[] key = Encoding.ASCII.GetBytes(signatureKey);
                using (var hmac = new HMACSHA512(key))
                {
                    var hashArray = new HMACSHA512(key).ComputeHash(Encoding.ASCII.GetBytes(notificationBody));
                    return hashArray.Aggregate("", (s, e) => s + string.Format("{0:x2}", e), s => s);
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
