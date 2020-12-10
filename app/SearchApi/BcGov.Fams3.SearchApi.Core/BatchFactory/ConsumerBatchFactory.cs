using MassTransit;

namespace BcGov.Fams3.SearchApi.Core.BatchFactory
{

    public interface IBatchConsumerFactory<T> where T : class
    {
        IConsumer<Batch<T>> CreateInstance();
    }
}
