namespace Spmco.Common.Notification
{
    public class MassTransitNotification
    {
        public Guid TransactionId { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string CacheKey { get; set; }
        public string Listener { get; set; }
        public string SendDate { get; set; }
        public string NotificationType { get; set; }
    }
}