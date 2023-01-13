namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Enum
{
    internal enum transactionTypeEnum
    {
        authOnlyTransaction,
        authCaptureTransaction,
        captureOnlyTransaction,
        refundTransaction,
        priorAuthCaptureTransaction,
        voidTransaction,
        getDetailsTransaction,
        authOnlyContinueTransaction,
        authCaptureContinueTransaction
    }
}
