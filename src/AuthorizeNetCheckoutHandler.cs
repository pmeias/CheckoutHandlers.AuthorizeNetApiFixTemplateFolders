using Dynamicweb.Core;
using Dynamicweb.Ecommerce.Cart;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.API;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum;
using Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model;
using Dynamicweb.Ecommerce.Orders;
using Dynamicweb.Ecommerce.Orders.Gateways;
using Dynamicweb.Extensibility.AddIns;
using Dynamicweb.Extensibility.Editors;
using Dynamicweb.Frontend;
using Dynamicweb.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi
{
    /// <summary>
    /// AuthorizeNet API Checkout Handler
    /// </summary>
    [AddInName("Authorize.Net API"), AddInDescription("AuthorizeNet API Checkout Handler"), AddInUseParameterGrouping(true)]
    public class AuthorizeNetCheckoutHandler : CheckoutHandler, ICancelOrder, IFullReturn, IRemoteCapture, ISavedCard, IParameterOptions
    {
        private static class Tags
        {
            public const string AuthorizeNetJavaScriptUrl = "AuthorizeNet.JavaScriptUrl";
            public const string PublicClientKey = "AuthorizeNet.PublicClientKey";
            public const string ApiLoginId = "AuthorizeNet.ApiLoginId";
            public const string FormAction = "AuthorizeNet.FormAction";
        }

        #region Fields

        /// <summary>
        /// Compact serializer settings. Do not excludes default value and null.
        /// </summary>
        private static readonly JsonSerializerOptions JsonSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull | System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
        };
        private static readonly object locker = new object();

        private RenderFormMode formMode = RenderFormMode.Hosted;
        private TransactionType transactionType = TransactionType.AuthCaptureTransaction;

        private const string SavedCardNamePlaceholder = "NeedToSaveCardWithName:";

        #endregion

        #region AddIn parameters

        [AddInParameter("API login ID"), AddInParameterEditor(typeof(TextParameterEditor), "")]
        public string ApiLoginId { get; set; } = "";

        [AddInParameter("Transaction key"), AddInParameterEditor(typeof(TextParameterEditor), "")]
        public string TransactionKey { get; set; } = "";

        [AddInParameter("Signature key"), AddInParameterEditor(typeof(TextParameterEditor), "TextArea=true")]
        public string SignatureKey { get; set; } = "";

        [AddInLabel("Public client key"), AddInParameter("PublicClientKey"), AddInParameterEditor(typeof(TextParameterEditor), "")]
        public string PublicClientKey { get; set; } = "";

        [AddInParameter("Allow save cards"), AddInParameterEditor(typeof(YesNoParameterEditor), "")]
        public bool AllowSaveCards { get; set; }

        [AddInLabel("The type of credit card transaction"), AddInParameter("TypeOfTransaction"), AddInParameterEditor(typeof(RadioParameterEditor), "SortBy=Key")]
        public string TypeOfTransaction
        {
            get
            {
                return transactionType.ToString();
            }
            set
            {
                switch (value)
                {
                    case nameof(TransactionType.AuthCaptureTransaction):
                        transactionType = TransactionType.AuthCaptureTransaction;
                        break;

                    case nameof(TransactionType.AuthOnlyTransaction):
                        transactionType = TransactionType.AuthOnlyTransaction;
                        break;
                }
            }
        }

        [AddInLabel("Test mode"), AddInParameter("TestMode"), AddInParameterEditor(typeof(YesNoParameterEditor), "")]
        public bool TestMode { get; set; }

        [AddInLabel("Payment form render mode"), AddInParameter("PaymentFormMode"), AddInParameterGroup("Template settings"), AddInParameterEditor(typeof(RadioParameterEditor), "SortBy=Key")]
        public string PaymentFormMode
        {
            get
            {
                return formMode.ToString();
            }
            set
            {
                switch (value)
                {
                    case nameof(RenderFormMode.Hosted):
                        formMode = RenderFormMode.Hosted;
                        break;

                    case nameof(RenderFormMode.Manual):
                        formMode = RenderFormMode.Manual;
                        break;

                    case nameof(RenderFormMode.HostedPartial):
                        formMode = RenderFormMode.HostedPartial;
                        break;
                }
            }
        }

        [AddInLabel("Payment form template"), AddInParameter("PaymentFormTemplate"), AddInParameterGroup("Template settings"), AddInParameterEditor(typeof(TemplateParameterEditor), "folder=templates/eCom7/CheckoutHandler/AuthorizeNet/Post")]
        public string PaymentFormTemplate { get; set; } = "";

        [AddInLabel("Cancel template"), AddInParameter("CancelTemplate"), AddInParameterGroup("Template settings"), AddInParameterEditor(typeof(TemplateParameterEditor), "folder=templates/eCom7/CheckoutHandler/AuthorizeNet/Cancel")]
        public string CancelTemplate { get; set; } = "";

        [AddInLabel("Error template"), AddInParameter("ErrorTemplate"), AddInParameterGroup("Template settings"), AddInParameterEditor(typeof(TemplateParameterEditor), "folder=templates/eCom7/CheckoutHandler/AuthorizeNet/Error")]
        public string ErrorTemplate { get; set; } = "";

        #endregion

        #region CheckoutHandler

        public override OutputResult BeginCheckout(Order order, CheckoutParameters parameters)
        {
            LogEvent(order, "Checkout started");

            if (string.IsNullOrEmpty(ApiLoginId))
            {
                throw new ArgumentNullException(nameof(ApiLoginId), "API login ID is reqired");
            }
            if (string.IsNullOrEmpty(TransactionKey))
            {
                throw new ArgumentNullException(nameof(ApiLoginId), "Transaction key is reqired");
            }

            if (!"USD".Equals(order.CurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Only USD currency is allowed. Order currency: {order.CurrencyCode}");
            }

            if (formMode == RenderFormMode.Hosted)
            {
                return RedirectToHostedForm(order);
            }
            else
            {
                string javaScriptUrl;
                if (formMode == RenderFormMode.Manual)
                {
                    javaScriptUrl = TestMode ? "https://jstest.authorize.net/v1/Accept.js" : "https://js.authorize.net/v1/Accept.js";
                }
                else
                {
                    javaScriptUrl = TestMode ? "https://jstest.authorize.net/v3/AcceptUI.js" : "https://js.authorize.net/v3/AcceptUI.js";
                }

                var template = new Template(PaymentFormTemplate);
                template.SetTag(Tags.ApiLoginId, ApiLoginId);
                template.SetTag(Tags.AuthorizeNetJavaScriptUrl, javaScriptUrl);
                template.SetTag(Tags.FormAction, $"{GetBaseUrl(order)}&action=FormPost");
                template.SetTag(Tags.PublicClientKey, PublicClientKey);

                return new ContentOutputResult() { Content = Render(order, template) };
            }
        }

        public override OutputResult HandleRequest(Order order)
        {
            LogEvent(order, "Redirected to AuthorizeNet CheckoutHandler");

            lock (locker)
            {
                var action = Converter.ToString(Context.Current?.Request["action"]);
                if (string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(order.GatewayResult))
                {
                    Callback(order);
                    return ContentOutputResult.Empty;
                }

                try
                {
                    switch (action)
                    {
                        case "FormPost":
                            return CreatePaymentTransaction(order, null);

                        case "Receipt":
                            return OrderCompleted(order, 0d, null);

                        case "Cancel":
                            return OrderCancelled(order);

                        default:
                            return ContentOutputResult.Empty;
                    }
                }
                catch (ThreadAbortException)
                {
                    return ContentOutputResult.Empty;
                }
                catch (Exception ex)
                {
                    return OnError(order, ex.Message, ex);
                }
            }
        }

        private OutputResult RedirectToHostedForm(Order order)
        {
            LogEvent(order, "Redirect to Hosted Form");

            double orderAmount = Services.Currencies.Round(order.Currency, order.Price.Price);

            // There is a limitation on Authorize.Net hosted payment form
            // You must URL-encode ampersands to do not break their JS :-)
            // They decodes it on their side
            var returnUrlPattern = $"{GetBaseUrl(order)}&action={{0}}".Replace("&", "%26"); // URL-encode ampersands
            var receipt = string.Format(returnUrlPattern, "Receipt");
            var cancel = string.Format(returnUrlPattern, "Cancel");

            hostedPaymentSettings settings = new hostedPaymentSettings
            {
                setting = new List<Setting>
                {
                    new Setting
                    {
                        settingName = SettingEnum.hostedPaymentReturnOptions.ToString(),
                        settingValue = System.Text.Json.JsonSerializer.Serialize(new { url = receipt, cancelUrl = cancel }, JsonSettings)
                    },
                    new Setting
                    {
                        settingName = SettingEnum.hostedPaymentPaymentOptions.ToString(),
                        settingValue = "{\"showBankAccount\": false}" // hide "Bank Account" payment method (NP approved)                        
                    },
                }
            };

            var getHostedPaymentPageRequest = new getHostedPaymentPageRequest
            {
                merchantAuthentication = GetMerchantAuthentication(),
                transactionRequest = CreateTransactionRequest(order, Converter.ToDecimal(orderAmount), null, null, true),
                hostedPaymentSettings = settings
            };
            getHostedPaymentPageResponse? getHostedPaymentPage = PostToAuthorizeNet<getHostedPaymentPageResponse>(JsonSerializer.Serialize(new { getHostedPaymentPageRequest }));
            if (getHostedPaymentPage is null) // server or transport error
            {
                return PaymentError(order, "Failed to get hosted payment page");
            }

            if (getHostedPaymentPage.messages.resultCode == messageTypeEnum.Ok.ToString())
            {
                var formUrl = TestMode ? "https://test.authorize.net/payment/payment" : "https://accept.authorize.net/payment/payment";
                var postValues = new Dictionary<string, string>
                {
                    { "token",  getHostedPaymentPage.token},
                };

                return GetSubmitFormResult(formUrl, postValues);
            }

            string errorMessage = $"Failed to get hosted payment page ({getHostedPaymentPage?.messages.message[0].code}): {getHostedPaymentPage?.messages.message[0].text}";

            return OnError(order, errorMessage);
        }

        private OutputResult CreatePaymentTransaction(Order order, customerProfilePaymentType? profileToCharge)
        {
            LogEvent(order, "Create payment transaction");

            double orderAmount = Services.Currencies.Round(order.Currency, order.Price.Price);

            paymentType? payment = null;
            if (profileToCharge is null)
            {
                payment = new paymentType
                {
                    opaqueData = new opaqueDataType
                    {
                        dataValue = Converter.ToString(Context.Current?.Request["dataValue"]),
                        dataDescriptor = Converter.ToString(Context.Current?.Request["dataDescriptor"]),
                    },
                };
            }

            var createTransactionRequest = new createTransactionRequest
            {
                transactionRequest = CreateTransactionRequest(order, Converter.ToDecimal(orderAmount), payment, profileToCharge, profileToCharge == null),
                merchantAuthentication = GetMerchantAuthentication()
            };

            createTransactionResponse? createTransaction = PostToAuthorizeNet<createTransactionResponse>(JsonSerializer.Serialize(new { createTransactionRequest }));

            if (createTransaction?.transactionResponse?.messages != null && createTransaction.messages?.resultCode == messageTypeEnum.Ok.ToString())
            {
                return OrderCompleted(order, orderAmount, createTransaction);
            }

            return OrderRefused(order, createTransaction?.transactionResponse?.errors[0].errorText);
        }

        private OutputResult OrderCompleted(Order order, double transactionAmount, createTransactionResponse? response)
        {
            LogEvent(order, "State ok");

            string cardName;
            var needSaveCard = NeedSaveCard(order, out cardName);

            order.TransactionAmount = transactionAmount;
            if (response is not null && response.transactionResponse is not null)
            {
                Helper.UpdateTransactionNumber(order, response.transactionResponse.transId);
                order.TransactionStatus = Helper.GetResponseTextByCode(response.transactionResponse.responseCode ?? "");
                order.TransactionCardType = response.transactionResponse.accountType;
                order.TransactionCardNumber = response.transactionResponse.accountNumber;
                if (needSaveCard)
                {
                    SaveCard(order, cardName);
                }
            }
            else if (needSaveCard)
            {
                // Cannot save card here. Will be saved on notification handling (actual for Hosted template)
                order.GatewayPaymentStatus = $"{SavedCardNamePlaceholder}{cardName}";
            }

            if (transactionType == TransactionType.AuthCaptureTransaction)
            {
                order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Success, "Capture successful");
                order.CaptureAmount = transactionAmount;
            }

            if (!order.Complete)
            {
                SetOrderComplete(order);
                CheckoutDone(order);
            }
            else
            {
                Save(order);
            }

            return PassToCart(order);
        }

        private OutputResult OrderCancelled(Order order)
        {
            LogEvent(order, "Order cancelled");

            order.TransactionAmount = 0;
            order.GatewayResult = "Payment has been cancelled";
            order.TransactionStatus = "Cancelled";
            CheckoutDone(order);

            var cancelTemplate = new Template(CancelTemplate);
            cancelTemplate.SetTag("CheckoutHandler:CancelMessage", "Payment has been cancelled before processing was completed");
            var orderRenderer = new Frontend.Renderer();
            orderRenderer.RenderOrderDetails(cancelTemplate, order, true);

            return new ContentOutputResult() { Content = cancelTemplate.Output() };
        }

        private OutputResult OrderRefused(Order order, string? refusalReason)
        {
            LogEvent(order, "Order refused");

            order.TransactionAmount = 0;
            order.GatewayResult = refusalReason;
            order.TransactionStatus = "Refused";
            Save(order);

            return OnError(order, $"Payment was refused. Refusal reason: {refusalReason}");
        }

        private OutputResult PaymentError(Order order, string reason)
        {
            LogEvent(order, "Payment error");

            order.TransactionAmount = 0;
            order.GatewayResult = reason;
            order.TransactionStatus = "Error";
            Save(order);

            return OnError(order, $"There was an error when the payment was being processed. Reason: {reason}");
        }

        private transactionRequestType CreateTransactionRequest(Order order, decimal orderAmount, paymentType? payment, customerProfilePaymentType? profileToCharge, bool includeCustomerData)
        {
            var request = new transactionRequestType
            {
                amount = orderAmount,
                payment = payment,
                lineItems = Helper.CreateLineItems(order),
                order = new orderType { invoiceNumber = order.Id, },
                customerIP = Helper.CropString(order.Ip, 15),
                profile = profileToCharge,

                transactionType =
                    (transactionType == TransactionType.AuthCaptureTransaction
                        ? transactionTypeEnum.authCaptureTransaction
                        : transactionTypeEnum.authOnlyTransaction
                    ).ToString(),
            };

            if (includeCustomerData)
            {
                request.billTo = Helper.CreateBillAddress(order);
                request.customer = new customerDataType
                {
                    id = order.CustomerAccessUserId.ToString(),
                    email = order.CustomerEmail,
                };

                if (!string.IsNullOrEmpty(order.DeliveryAddress) && !string.IsNullOrEmpty(order.DeliveryZip))
                {
                    request.shipTo = Helper.CreateShipAddress(order);
                }
            }

            return request;
        }

        private void Callback(Order order)
        {
            LogEvent(order, "Notification callback started");

            if (string.IsNullOrEmpty(SignatureKey))
            {
                LogError(order, "Notification callback failed with message: Specify Signature key to handle notifications");
                throw new ArgumentNullException("Signature key is empty");
            }

            if (TestMode)
            {
                LogEvent(order, "Notification contents: {0}", order.GatewayResult);
            }

            var gatewayResult = order.GatewayResult;
            order.GatewayResult = string.Empty;
            Services.Orders.UpdateGatewayResult(order, false);

            var hmacSignature = Context.Current?.Request?.Headers["X-ANET-Signature"]?.Replace("sha512=", string.Empty);
            if (!Helper.IsValidHmac(SignatureKey.Trim(), gatewayResult, hmacSignature))
            {
                LogError(order, "Cannot handle notification item: HMAC validation failed");
                return;
            }

            var requestData = Converter.Deserialize<NotificationItem>(gatewayResult);
            if (requestData is null)
            {
                LogError(order, "Cannot handle notification item: request data is null");
                return;
            }

            var eventType = requestData.GetEventType();
            if (!eventType.HasValue)
            {
                LogError(order, "Cannot handle notification item: event type is not defined");
                return;
            }

            LogEvent(order, "Notification event type: {0}", requestData.EventType);

            var payload = requestData.Payload;
            switch (eventType.Value)
            {
                case NotificationEventType.AuthCaptureCreated:
                case NotificationEventType.AuthCreated:
                    if (payload.ResponseCode == 1)
                    {
                        UpdateTransactionData(order, payload);
                        if (string.IsNullOrEmpty(order.TransactionCardNumber))
                        {
                            // There is a case when we do not get transaction information: using Hosted template, user complete the payment and do not click 'Continue' button on Authorize.Net receipt page (e.g. close the tab)
                            // But we always gets notifications (if set up). So, we can try to get missed data here, using GetTransactionDetails call
                            var transactionDetails = GetTransactionDetails(payload.Id);
                            if (transactionDetails is not null)
                            {
                                var cardDetails = transactionDetails.payment.creditCard;
                                if (cardDetails is not null)
                                {
                                    order.TransactionCardType = cardDetails.cardType.ToString();
                                    order.TransactionCardNumber = cardDetails.cardNumber;
                                }
                                if (order.GatewayPaymentStatus?.StartsWith(SavedCardNamePlaceholder) == true)
                                {
                                    var cardName = order.GatewayPaymentStatus.Substring(SavedCardNamePlaceholder.Length);
                                    order.GatewayPaymentStatus = string.Empty;
                                    SaveCard(order, cardName);
                                }
                            }
                        }

                        if (eventType.Value == NotificationEventType.AuthCaptureCreated)
                        {
                            order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Success, "Capture successful");
                            order.CaptureAmount = payload.Amount;
                        }

                        if (!order.Complete)
                        {
                            SetOrderComplete(order);
                        }
                        else
                        {
                            Save(order);
                        }
                    }
                    else
                    {
                        order.TransactionStatus = $"Authorisation failed: {Helper.GetResponseTextByCode(payload.ResponseCode)}";
                        Save(order);
                    }
                    break;

                case NotificationEventType.CaptureCreated:
                case NotificationEventType.PriorAuthCaptureCreated:
                    if (payload.ResponseCode == 1)
                    {
                        UpdateTransactionData(order, payload);
                        order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Success, "Capture successful");
                        order.CaptureAmount = payload.Amount;
                    }
                    else
                    {
                        order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Failed, $"Capture failed: {Helper.GetResponseTextByCode(payload.ResponseCode)}");
                    }
                    Save(order);
                    break;

                case NotificationEventType.RefundCreated:
                    // only captured orders can receive this notification
                    if (order.CaptureInfo.State != OrderCaptureInfo.OrderCaptureState.Success)
                    {
                        if (order.TransactionAmount < 0.001)
                        {
                            order.TransactionAmount = order.Price.Price;
                        }
                        order.CaptureInfo.Message = "Capture successful";
                        order.CaptureInfo.State = OrderCaptureInfo.OrderCaptureState.Success;
                        order.CaptureAmount = order.TransactionAmount;
                        Save(order);
                    }

                    if (order.ReturnOperations?.Any(o => o.State == OrderReturnOperationState.FullyReturned) == true)
                    {
                        break; // refund completed, no additional actions needed
                    }

                    if (payload.ResponseCode == 1)
                    {
                        OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.FullyReturned, "Order refund successful", payload.Amount, order);
                    }
                    else
                    {
                        OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, $"Order refund failed: {Helper.GetResponseTextByCode(payload.ResponseCode)}", payload.Amount, order);
                    }
                    break;

                case NotificationEventType.VoidCreated:
                    if (payload.ResponseCode == 1)
                    {
                        order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Cancel, "Cancel successful");
                    }
                    else
                    {
                        order.CaptureInfo = new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Cancel, $"Cancel order failed: {Helper.GetResponseTextByCode(payload.ResponseCode)}");
                    }
                    Save(order);
                    break;

                default:
                    LogError(order, "Cannot handle notification: event type is unknown ({0})", requestData.EventType);
                    break;
            }

            LogEvent(order, "Notification callback finished");
        }

        private void UpdateTransactionData(Order order, NotificationPayload payload)
        {
            Helper.UpdateTransactionNumber(order, payload.Id);
            order.TransactionAmount = payload.Amount;
            order.TransactionStatus = Helper.GetResponseTextByCode(payload.ResponseCode);
        }

        private transactionDetailsType? GetTransactionDetails(string transactionId)
        {
            var getTransactionDetailsRequest = new getTransactionDetailsRequest
            {
                transrefId = transactionId,
                merchantAuthentication = GetMerchantAuthentication()
            };

            getTransactionDetailsResponse? getTransactionDetails = PostToAuthorizeNet<getTransactionDetailsResponse>(JsonSerializer.Serialize(new { getTransactionDetailsRequest }));
            if (getTransactionDetails is null) // server or transport error
            {
                return null;
            }

            if (getTransactionDetails.messages.resultCode == messageTypeEnum.Ok.ToString())
            {
                return getTransactionDetails.transaction;
            }

            return null;
        }

        #endregion

        #region ICancelOrder

        public bool CancelOrder(Order order)
        {
            LogEvent(order, "Attempting cancel");

            var errorText = Helper.GetOrderError(order);
            if (!string.IsNullOrEmpty(errorText))
            {
                LogError(order, errorText);
                return false;
            }

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),
                refTransId = order.TransactionNumber,
            };

            var createTransactionRequest = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
                merchantAuthentication = GetMerchantAuthentication()
            };

            createTransactionResponse? createTransaction = PostToAuthorizeNet<createTransactionResponse>(JsonSerializer.Serialize(new { createTransactionRequest }));
            if (createTransaction is null)
                return false;

            if (createTransaction.transactionResponse?.messages != null && createTransaction.messages?.resultCode == messageTypeEnum.Ok.ToString())
            {
                LogEvent(order, "Cancel order succeed");
                return true;
            }

            LogError(order, Helper.CreateErrorMessage("Cancel order failed with message", createTransaction));
            return false;
        }

        #endregion

        #region IRemoteCapture

        public OrderCaptureInfo Capture(Order order)
        {
            LogEvent(order, "Attempting capture");

            var errorText = Helper.GetOrderError(order);
            if (!string.IsNullOrEmpty(errorText))
            {
                LogError(order, errorText);
                return new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Failed, errorText);
            }

            decimal orderAmount = Converter.ToDecimal(Services.Currencies.Round(order.Currency, order.Price.Price));

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.priorAuthCaptureTransaction.ToString(),
                amount = orderAmount,
                refTransId = order.TransactionNumber,
            };

            var createTransactionRequest = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
                merchantAuthentication = GetMerchantAuthentication()
            };

            createTransactionResponse? createTransaction = PostToAuthorizeNet<createTransactionResponse>(JsonSerializer.Serialize(new { createTransactionRequest }));
            if (createTransaction is null)
            {
                var text = "Something went wrong in communication with Authorize";
                LogError(order, text);
                return new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Failed, text);
            }

            if (createTransaction.transactionResponse?.messages != null && createTransaction.messages?.resultCode == messageTypeEnum.Ok.ToString())
            {
                LogEvent(order, "Capture successful", DebuggingInfoType.CaptureResult);
                Helper.UpdateTransactionNumber(order, createTransaction.transactionResponse.transId);
                return new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Success, "Capture successful");
            }

            string infoText = Helper.CreateErrorMessage("Payment was unsucceeded with error", createTransaction);
            LogEvent(order, infoText, DebuggingInfoType.CaptureResult);
            order.CaptureInfo.Message = infoText;
            order.CaptureInfo.State = OrderCaptureInfo.OrderCaptureState.Failed;
            Save(order);

            return new OrderCaptureInfo(OrderCaptureInfo.OrderCaptureState.Failed, infoText);
        }

        #endregion

        #region IFullReturn

        public void FullReturn(Order order)
        {
            var errorText = Helper.GetOrderError(order);
            if (!string.IsNullOrEmpty(errorText))
            {
                LogError(order, errorText);

                if (order != null)
                {
                    OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, errorText, 0, order);
                }
                return;
            }

            if (string.IsNullOrEmpty(order.TransactionCardNumber))
            {
                errorText = "Transaction card number must be specified";
                LogError(order, errorText);
                OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, errorText, 0, order);
                return;
            }

            if (order.TransactionAmount < 0.01)
            {
                errorText = "Transaction amount must be specified";
                LogError(order, errorText);
                OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, errorText, 0, order);
                return;
            }

            double refundAmount = order.TransactionAmount;

            if (order.CaptureInfo is null || order.CaptureInfo.State != OrderCaptureInfo.OrderCaptureState.Success || refundAmount < 0.01)
            {
                var message = "Order must be captured before return";
                LogError(order, message);
                OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, message, refundAmount, order);
                return;
            }

            var creditCard = new creditCardType
            {
                cardNumber = order.TransactionCardNumber.Substring(Math.Max(0, order.TransactionCardNumber.Length - 4)), // get last 4 chars (as per documentation)
                expirationDate = "XXXX", // as per documentation: For refunds, use XXXX instead of the card expiration date.
            };
            var paymentType = new paymentType { creditCard = creditCard };
            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),
                payment = paymentType,
                amount = Converter.ToDecimal(refundAmount),
                order = new orderType { invoiceNumber = order.Id, },
                refTransId = order.TransactionNumber
            };

            var createTransactionRequest = new createTransactionRequest
            {
                transactionRequest = transactionRequest,
                merchantAuthentication = GetMerchantAuthentication()
            };

            createTransactionResponse? createTransaction = PostToAuthorizeNet<createTransactionResponse>(JsonSerializer.Serialize(new { createTransactionRequest }));
            if (createTransaction is null)
                return;

            if (createTransaction.transactionResponse?.messages != null && createTransaction.messages?.resultCode == messageTypeEnum.Ok.ToString())
            {
                LogEvent(order, "Refund successful", DebuggingInfoType.ReturnResult);
                OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.FullyReturned, "Authorize.Net has full refunded payment.", refundAmount, order);
                return;
            }

            string infoText = Helper.CreateErrorMessage("Refund was unsucceeded with message", createTransaction);
            LogEvent(order, infoText, DebuggingInfoType.ReturnResult);
            OrderReturnInfo.SaveReturnOperation(OrderReturnOperationState.Failed, infoText, refundAmount, order);
        }

        #endregion

        #region ISavedCard

        public void DeleteSavedCard(int savedCardId)
        {
            var savedCard = Services.PaymentCard.GetById(savedCardId);
            if (savedCard is null)
            {
                return;
            }

            var cardToken = savedCard.Token;
            var userId = savedCard.UserID;

            if (userId <= 0 || string.IsNullOrEmpty(cardToken))
            {
                return;
            }

            var profile = GetCustomerProfile(userId, false);
            if (string.IsNullOrEmpty(profile?.customerProfileId))
            {
                return;
            }

            var deleteCustomerPaymentProfileRequest = new deleteCustomerPaymentProfileRequest
            {
                customerProfileId = profile.customerProfileId,
                customerPaymentProfileId = cardToken,
                merchantAuthentication = GetMerchantAuthentication()
            };
            PostToAuthorizeNet<deleteCustomerPaymentProfileResponse>(JsonSerializer.Serialize(new { deleteCustomerPaymentProfileRequest }));
        }

        public string UseSavedCard(Order order)
        {
            var savedCard = Services.PaymentCard.GetById(order.SavedCardId);
            if (!string.IsNullOrEmpty(savedCard?.Token) && order.CustomerAccessUserId == savedCard.UserID)
            {
                var profile = GetCustomerProfile(order.CustomerAccessUserId, false);
                if (!string.IsNullOrEmpty(profile?.customerProfileId))
                {
                    var profileToCharge = new customerProfilePaymentType
                    {
                        customerProfileId = profile.customerProfileId,
                        paymentProfile = new paymentProfile { paymentProfileId = savedCard.Token },
                    };

                    if (CreatePaymentTransaction(order, profileToCharge) is ContentOutputResult paymentContentOutputResult)
                        return paymentContentOutputResult.Content;
                }
            }

            return BeginCheckout(order) is ContentOutputResult checkoutContentOutput ?
                checkoutContentOutput.Content : string.Empty;
        }

        public bool SavedCardSupported(Order order) => AllowSaveCards;

        private bool NeedSaveCard(Order order, out string cardName)
        {
            if (AllowSaveCards && order.CustomerAccessUserId > 0 && (order.DoSaveCardToken || !string.IsNullOrEmpty(order.SavedCardDraftName)))
            {
                cardName = !string.IsNullOrEmpty(order.SavedCardDraftName) ? order.SavedCardDraftName : order.Id;
                return true;
            }

            cardName = string.Empty;
            return false;
        }

        private void SaveCard(Order order, string cardName)
        {
            if (AllowSaveCards && order.CustomerAccessUserId > 0)
            {
                var profile = GetCustomerProfile(order.CustomerAccessUserId, true);
                if (profile is null)
                {
                    return;
                }

                var cardToken = CreatePaymentProfileFromTransaction(order.TransactionNumber, profile.customerProfileId);
                if (!string.IsNullOrEmpty(cardToken))
                {
                    PaymentCardToken? savedCard = Services.PaymentCard.GetByUserId(order.CustomerAccessUserId).FirstOrDefault(t => t.Token.Equals(cardToken));
                    if (savedCard is null)
                    {
                        savedCard = Services.PaymentCard.CreatePaymentCard(order.CustomerAccessUserId, order.PaymentMethodId, cardName, order.TransactionCardType, order.TransactionCardNumber, cardToken);
                    }

                    order.SavedCardId = savedCard.ID;
                    Save(order);

                    LogEvent(order, "Saved card created: {0}", savedCard.Name);
                }
            }
        }

        private customerProfileMaskedType? GetCustomerProfile(int userId, bool tryCreate)
        {
            var getCustomerProfileRequest = new getCustomerProfileRequest
            {
                merchantCustomerId = userId.ToString(),
                merchantAuthentication = GetMerchantAuthentication()
            };

            getCustomerProfileResponse? getCustomerProfile = PostToAuthorizeNet<getCustomerProfileResponse>(JsonSerializer.Serialize(new { getCustomerProfileRequest }));

            if (getCustomerProfile is null) // server or transport error
            {
                return null;
            }

            if (getCustomerProfile.profile != null && getCustomerProfile.messages.resultCode == messageTypeEnum.Ok.ToString()) // profile was found
            {
                return getCustomerProfile.profile;
            }

            if (tryCreate)
            {
                var createCustomerProfileRequest = new createCustomerProfileRequest
                {
                    profile = new customerProfileType
                    {
                        merchantCustomerId = userId.ToString()
                    },
                    merchantAuthentication = GetMerchantAuthentication()
                };

                createCustomerProfileResponse? createCustomerProfile = PostToAuthorizeNet<createCustomerProfileResponse>(JsonSerializer.Serialize(new { createCustomerProfileRequest }));
                if (createCustomerProfile is null)
                    return null;

                if (createCustomerProfile.messages.resultCode == messageTypeEnum.Ok.ToString())
                {
                    return new customerProfileMaskedType { customerProfileId = createCustomerProfile.customerProfileId };
                }
            }

            return null;
        }

        private string CreatePaymentProfileFromTransaction(string transactionId, string customerProfileId)
        {
            if (string.IsNullOrEmpty(transactionId) || string.IsNullOrEmpty(customerProfileId))
            {
                return string.Empty;
            }

            var createCustomerProfileFromTransactionRequest = new createCustomerProfileFromTransactionRequest
            {
                transId = transactionId,
                customerProfileId = customerProfileId,
                merchantAuthentication = GetMerchantAuthentication()
            };

            createCustomerProfileResponse? createCustomerProfile = PostToAuthorizeNet<createCustomerProfileResponse>(JsonSerializer.Serialize(new { createCustomerProfileFromTransactionRequest }));
            if (createCustomerProfile is null)// server or transport error
            {
                return string.Empty;
            }

            if (createCustomerProfile.messages.resultCode == messageTypeEnum.Ok.ToString())
            {
                return createCustomerProfile.customerPaymentProfileIdList[0];
            }

            return string.Empty;
        }

        #endregion

        #region IDropDownOptions

        public IEnumerable<ParameterOption> GetParameterOptions(string parameterName)
        {
            var result = new List<ParameterOption>();
            switch (parameterName)
            {
                case "PaymentFormMode":
                    result.Add(new("Hosted (Use Authorize.Net hosted form)", nameof(RenderFormMode.Hosted)));
                    result.Add(new("Partial Hosted (Show Authorize.Net hosted form in pop-up on your own template)", nameof(RenderFormMode.HostedPartial)));
                    result.Add(new("Manual (Use your own payment form. SSL required)", nameof(RenderFormMode.Manual)));
                    break;

                case "TypeOfTransaction":
                    result.Add(new("Authorization and Capture", nameof(TransactionType.AuthCaptureTransaction)));
                    result.Add(new("Authorization only", nameof(TransactionType.AuthOnlyTransaction)));
                    break;
            }
            return result;
        }

        #endregion

        #region Private methods

        private merchantAuthenticationType GetMerchantAuthentication()
        {
            return new merchantAuthenticationType()
            {
                name = ApiLoginId,
                transactionKey = TransactionKey
            };
        }
        private T? PostToAuthorizeNet<T>(string jsonObject)
        {
            string url = TestMode ? "https://apitest.authorize.net/xml/v1/request.api" : "https://api.authorize.net/xml/v1/request.api";
            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonObject);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                using (var response = client.PostAsync(url, content).GetAwaiter().GetResult())
                {
                    string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonSerializer.Deserialize<T>(responseText);
                }
            }
        }

        private OutputResult OnError(Order order, string message) => OnError(order, message, null);

        private OutputResult OnError(Order order, string message, Exception? exception)
        {
            if (exception != null)
            {
                LogError(order, exception, message);
            }
            else
            {
                LogError(order, message);
            }

            order.TransactionAmount = 0;
            order.TransactionStatus = "Failed";
            order.Errors.Add(message);
            Save(order);

            Services.Orders.DowngradeToCart(order);
            order.TransactionStatus = string.Empty;
            Common.Context.SetCart(order);

            if (string.IsNullOrWhiteSpace(ErrorTemplate))
            {
                return PassToCart(order);
            }

            var errorTemplate = new Template(ErrorTemplate);
            errorTemplate.SetTag("CheckoutHandler:ErrorMessage", message);

            return new ContentOutputResult() { Content = Render(order, errorTemplate) };
        }

        private void Save(Order order) => Services.Orders.Save(order);

        #endregion

        private enum RenderFormMode { Hosted, HostedPartial, Manual }

        private enum TransactionType { AuthCaptureTransaction, AuthOnlyTransaction }
    }
}
