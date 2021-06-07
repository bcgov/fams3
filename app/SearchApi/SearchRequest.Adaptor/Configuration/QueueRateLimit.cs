namespace SearchRequestAdaptor.Configuration
{
    public class QueueRateLimit
    {
        public QueueRateLimit()
        {
            this.SearchRequestOrdered_RateLimit = 1;
            this.SearchRequestOrdered_RateInterval = 5;
            this.SearchRequestOrdered_ConcurrencyLimit = 1;

            this.NotificationAcknowledged_RateLimit = 1;
            this.NotificationAcknowledged_RateInterval = 5;
            this.NotificationAcknowledged_ConcurrencyLimit = 1;

        }

        /// <summary>
        /// RabbitMq Host
        /// </summary>
        public int SearchRequestOrdered_RateLimit { get; set; }
        public int SearchRequestOrdered_RateInterval { get; set; }
        public int SearchRequestOrdered_ConcurrencyLimit { get; set; }

        public int NotificationAcknowledged_RateLimit { get; set; }
        public int NotificationAcknowledged_RateInterval { get; set; }
        public int NotificationAcknowledged_ConcurrencyLimit { get; set; }

    }
}

