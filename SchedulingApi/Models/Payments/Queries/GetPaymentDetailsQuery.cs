using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Core;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using Payments.Domain.Payments.ReadModels;

namespace Payments.Domain.Payments.Queries
{
    public class GetPaymentDetailsQuery : IQuery<PaymentDetailsReadModel>
    {
        public GetPaymentDetailsQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }

    public class GetPaymentDetailsQueryHandler : IQueryHandler<GetPaymentDetailsQuery, PaymentDetailsReadModel>
    {
        private readonly IMongoDbReadModelStore<PaymentDetailsReadModel> readStore;

        public GetPaymentDetailsQueryHandler(IMongoDbReadModelStore<PaymentDetailsReadModel> readStore)
        {
            this.readStore = readStore;
        }

        public async Task<PaymentDetailsReadModel> ExecuteQueryAsync(GetPaymentDetailsQuery query, CancellationToken cancellationToken)
        {
            var result = await readStore.FindAsync(x => x.OrderId == query.OrderId).ConfigureAwait(false);
            return result.Current.SingleOrDefault();
        }
    }
}