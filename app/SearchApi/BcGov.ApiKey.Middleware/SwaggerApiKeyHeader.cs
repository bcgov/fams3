using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace BcGov.ApiKey.Middleware
{
    public class SwaggerApiKeyHeader : IOperationProcessor
    {
        bool IOperationProcessor.Process(OperationProcessorContext context)
        {
            context.OperationDescription.Operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = ApiKeyMiddleware.HEADER_APIKEYNAME,
                Kind = OpenApiParameterKind.Header,
                Type = NJsonSchema.JsonObjectType.String,
                IsRequired = true
            });

            return true;
        }
    }
}
