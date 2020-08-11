using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchRequestAdaptor.Publisher;
using Serilog;

namespace SearchRequest.Adaptor.Notifier
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly ISearchRequestEventPublisher _publisher;
    
        public NotificationController(ISearchRequestEventPublisher searchRequestEventPublisher, ILogger<NotificationController> logger)
        {
            _publisher = searchRequestEventPublisher;
            _logger = logger;
        }
    }
}
