﻿using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace BcGov.Fams3.Utils.Logger
{
    public static class EnrichersExtensions
    {
        public static LoggerConfiguration WithPropertySearchRequestKey(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new SearchRequestKeyEnricher(propertyName));
        }

        public static LoggerConfiguration WithPropertyDataPartner(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new DataPartnerEnricher(propertyName));
        }

        public static LoggerConfiguration WithPropertyRequestRef(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new RequestRefEnricher(propertyName));
        }

        public static LoggerConfiguration WithPropertyAgencyCode(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new AgencyCodeEnricher(propertyName));
        }
    }

}
