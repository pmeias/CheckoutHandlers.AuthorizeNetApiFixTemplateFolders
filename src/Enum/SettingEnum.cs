namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum
{
    internal enum SettingEnum
    {
        emailCustomer, //true/false. Used by createTransaction method.
        merchantEmail, //string. Used by createTransaction method.
        allowPartialAuth, //true/false. Used by createTransaction method.
        headerEmailReceipt, //string. Used by createTransaction method.
        footerEmailReceipt, //string. Used by createTransaction method.
        recurringBilling, //true/false. Used by createTransaction method.
        duplicateWindow, //number. Used by createTransaction method.
        testRequest, //true/false. Used by createTransaction method.
        hostedProfileReturnUrl, //string. Used by getHostedProfilePage method.
        hostedProfileReturnUrlText, //string. Used by getHostedProfilePage method.
        hostedProfilePageBorderVisible, //true/false. Used by getHostedProfilePage method.
        hostedProfileIFrameCommunicatorUrl, //string. Used by getHostedProfilePage method.
        hostedProfileHeadingBgColor, //#e0e0e0. Used by getHostedProfilePage method.
        hostedProfileValidationMode, // liveMode/testMode liveMode: generates a transaction to the processor in the amount of 0.01 or 0.00. is immediately voided, if successful. testMode: performs field validation only, all fields are validated except unrestricted field definitions (viz. telephone number) do not generate errors. If a validation transaction is unsuccessful, the profile is not created, and the merchant receives an error. 
        hostedProfileBillingAddressRequired, //true/false. If true, sets First Name, Last Name, Address, City, State, and Zip Code as required fields in order for a payment profile to be created or updated within a hosted CIM form.
        hostedProfileCardCodeRequired, //true/false. If true, sets the Card Code field as required in order for a payment profile to be created or updated within a hosted CIM form.
        hostedProfileBillingAddressOptions, // showBillingAddress/showNone showBillingAddress: Allow merchant to show billing address. showNone : Hide billing address and billing name. 
        hostedProfileManageOptions, // showAll/showPayment/ShowShipping showAll: Shipping and Payment profiles are shown on the manage page, this is the default. showPayment : Only Payment profiles are shown on the manage page. showShipping : Only Shippiung profiles are shown on the manage page. 
        hostedPaymentIFrameCommunicatorUrl, //JSON string. Used by getHostedPaymentPage method.
        hostedPaymentButtonOptions, //JSON string. Used by getHostedPaymentPage method.
        hostedPaymentReturnOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentOrderOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentPaymentOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentBillingAddressOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentShippingAddressOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentSecurityOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentCustomerOptions, //JSON string. Used by getHostedPaymentPage method
        hostedPaymentStyleOptions, //JSON string. Used by getHostedPaymentPage method
        typeEmailReceipt, //JSON string. Used by sendCustomerTransactionReceipt method
        hostedProfilePaymentOptions, // showAll/showCreditCard/showBankAccount showAll: both CreditCard and BankAccount sections will be shown on Add payment page, this is the default. showCreditCard :only CreditCard payment form will be shown on Add payment page. showBankAccount :only BankAccount payment form will be shown on Add payment page. 
        hostedProfileSaveButtonText, //string. Used by getHostedProfilePage method to accept button text configuration.
        hostedPaymentVisaCheckoutOptions //string. Used by getHostedPaymentPage method to accept VisaCheckout configuration.
    }
}
