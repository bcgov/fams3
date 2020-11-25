using BcGov.Fams3.SearchApi.Contracts.IA;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.IA
{
    public class IASearchConsumer : IConsumer<IASearchOrdered>
    {

        private readonly ILogger<IASearchConsumer> _logger;

        public IASearchConsumer(ILogger<IASearchConsumer> logger)
        {
            _logger = logger;
        }
        public async  Task Consume(ConsumeContext<IASearchOrdered> context)
        {
            try
            {
                _logger.LogInformation($"Successfully handling new IA search request [{context.Message.SearchRequestKey}]");

                _logger.LogWarning("Sample Adapter, do not use in PRODUCTION.");

                

                    if ((context.Message.Person.Identifiers.Count() > 0))
                    {
                        throw new Exception("Exception from Sample Adapter, You don't have ID");

                    }
                  
                    await context.Publish(FakeIABuilder.BuildFakeIASeachResult(context.Message.SearchRequestId, context.Message.SearchRequestKey,context.Message.BatchNo));


                
            }
            catch (Exception ex)
            {
               
                    await context.Publish(FakeIABuilder.BuildFakeIAFailed( context.Message.SearchRequestId, context.Message.SearchRequestKey,context.Message.BatchNo, ex.Message, false));
                

            }
        }
    }
}
