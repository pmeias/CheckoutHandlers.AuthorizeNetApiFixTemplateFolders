namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    public enum NotificationEventType
    {
        /// <summary>
        /// Notifies you that an authorization and capture transaction was created.
        /// </summary>
        AuthCaptureCreated,

        /// <summary>
        /// Notifies you that an authorization transaction was created.
        /// </summary>
        AuthCreated,

        /// <summary>
        /// Notifies you that a capture transaction was created.
        /// </summary>
        CaptureCreated,

        /// <summary>
        /// Notifies you that a previous authorization was captured.
        /// </summary>
        PriorAuthCaptureCreated,

        /// <summary>
        /// Notifies you that a successfully settled transaction was refunded.
        /// </summary>
        RefundCreated,

        /// <summary>
        /// Notifies you that an unsettled transaction was voided.
        /// </summary>
        VoidCreated,
    }
}
