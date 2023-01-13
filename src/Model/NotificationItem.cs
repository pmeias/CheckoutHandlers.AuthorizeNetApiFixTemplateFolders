using System;
using System.Runtime.Serialization;

namespace Dynamicweb.Ecommerce.CheckoutHandlers.AuthorizeNetApi.Model
{
    [DataContract]
    public partial class NotificationItem
    {
        [DataMember(Name = "notificationId")]
        public string Id { get; set; }

        [DataMember(Name = "eventType")]
        public string EventType { get; set; }

        [DataMember(Name = "eventDate")]
        public DateTime EventDate { get; set; }

        [DataMember(Name = "webhookId")]
        public string WebhookId { get; set; }

        [DataMember(Name = "payload")]
        public NotificationPayload Payload { get; set; }

        public NotificationEventType? GetEventType()
        {
            switch (EventType)
            {
                case "net.authorize.payment.authcapture.created": return NotificationEventType.AuthCaptureCreated;
                case "net.authorize.payment.authorization.created": return NotificationEventType.AuthCreated;
                case "net.authorize.payment.capture.created": return NotificationEventType.CaptureCreated;
                case "net.authorize.payment.priorAuthCapture.created": return NotificationEventType.PriorAuthCaptureCreated;
                case "net.authorize.payment.refund.created": return NotificationEventType.RefundCreated;
                case "net.authorize.payment.void.created": return NotificationEventType.VoidCreated;
                default: return null;
            }
        }
    }
}
