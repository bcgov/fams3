namespace SearchApi.Web.Configuration
{
    public class QueueRateLimit
    {
        public QueueRateLimit()
        {
            this.PersonSearchCompleted_RateLimit = 1;
            this.PersonSearchCompleted_RateInterval = 10;
            this.PersonSearchCompletedJCA_RateLimit = 1;
            this.PersonSearchCompletedJCA_RateInterval = 5;
            this.PersonSearchFailed_RateLimit = 1;
            this.PersonSearchFailed_RateInterval = 5;
            this.PersonSearchInformation_RateLimit = 1;
            this.PersonSearchInformation_RateInterval = 5;
            this.PersonSearchAccepted_RateLimit = 1;
            this.PersonSearchAccepted_RateInterval = 5;
            this.PersonSearchSubmitted_RateLimit = 1;
            this.PersonSearchSubmitted_RateInterval = 5;
            this.PersonSearchRejected_RateLimit = 1;
            this.PersonSearchRejected_RateInterval = 5;
            this.PersonSearchCompleted_ConcurrencyLimit = 1;
            this.PersonSearchCompletedJCA_ConcurrencyLimit = 1;
            this.PersonSearchAccepted_ConcurrencyLimit = 1;
            this.PersonSearchFailed_ConcurrencyLimit = 1;
            this.PersonSearchInformation_ConcurrencyLimit = 1;
            this.PersonSearchSubmitted_ConcurrencyLimit = 1;
            this.PersonSearchRejected_ConcurrencyLimit = 1;
        }

        /// <summary>
        /// RabbitMq Host
        /// </summary>
        public int PersonSearchCompleted_RateLimit { get; set; }
        public int PersonSearchCompleted_RateInterval { get; set; }
        public int PersonSearchCompleted_ConcurrencyLimit { get; set; }

        public int PersonSearchCompletedJCA_RateLimit { get; set; }
        public int PersonSearchCompletedJCA_RateInterval { get; set; }
        public int PersonSearchCompletedJCA_ConcurrencyLimit { get; set; }

        public int PersonSearchFailed_RateLimit { get; set; }
        public int PersonSearchFailed_RateInterval { get; set; }
        public int PersonSearchFailed_ConcurrencyLimit { get; set; }

        public int PersonSearchInformation_RateLimit { get; set; }
        public int PersonSearchInformation_RateInterval { get; set; }
        public int PersonSearchInformation_ConcurrencyLimit { get; set; }

        public int PersonSearchAccepted_RateLimit { get; set; }
        public int PersonSearchAccepted_RateInterval { get; set; }
        public int PersonSearchAccepted_ConcurrencyLimit { get; set; }

        public int PersonSearchRejected_RateLimit { get; set; }
        public int PersonSearchRejected_RateInterval { get; set; }
        public int PersonSearchRejected_ConcurrencyLimit { get; set; }
        
        public int PersonSearchSubmitted_RateLimit { get; set; }
        public int PersonSearchSubmitted_RateInterval { get; set; }
        public int PersonSearchSubmitted_ConcurrencyLimit { get; set; }

    }
}

