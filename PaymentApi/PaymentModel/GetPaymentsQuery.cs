using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using EventFlow.ReadStores;
using EventFlow.ReadStores.InMemory;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using EventFlow.ValueObjects;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentApi.PaymentModel
{
    public class GetPaymentQuery : IQuery<IReadOnlyCollection<Payment>>
    {
        public GetPaymentQuery(params PaymentId[] paymentIds) : this((IEnumerable<PaymentId>)paymentIds)
        {

        }

        public GetPaymentQuery(IEnumerable<PaymentId> paymentIds)
        {
            PaymentIds = paymentIds;
        }

        public IEnumerable<PaymentId> PaymentIds { get; }
    }

    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class PaymentId : Identity<PaymentId>
    {
        public PaymentId(string value) : base(value)
        {
        }
    }

    public class Payment
    {
        public String AppointmentId { get; set; }
        public decimal Ammount { get; set; }
    }

    public class PaymentAggregate : SnapshotAggregateRoot<PaymentAggregate, PaymentId, PaymentSnapshot>
    {
        protected PaymentAggregate(PaymentId id, ISnapshotStrategy snapshotStrategy) : base(id, snapshotStrategy)
        {
        }

        protected override Task<PaymentSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        protected override Task LoadSnapshotAsync(PaymentSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    [SnapshotVersion("snapshot", 1)]
    public class PaymentSnapshot : ISnapshot
    {
        public PaymentSnapshot()
        {
        }
    }

    internal class GetPaymentQueryHandler : IQueryHandler<GetPaymentQuery, IReadOnlyCollection<Payment>>
    {
        private readonly IMongoDbReadModelStore<PaymentReadModel> readStore;

        public GetPaymentQueryHandler(IMongoDbReadModelStore<PaymentReadModel> readStore)
        {
            this.readStore = readStore;
        }
        public async Task<IReadOnlyCollection<Payment>> ExecuteQueryAsync(GetPaymentQuery query, CancellationToken cancellationToken)
        {
            var paymentIds = new HashSet<PaymentId>(query.PaymentIds);
            var paymentReadModels = await readStore.FindAsync(x => paymentIds.Contains(x.Id)).ConfigureAwait(false);
            return paymentReadModels.ToEnumerable().Select(x => x.ToPayment()).ToList();


        }
    }

    internal class PaymentReadModel : IMongoDbInsertOnlyReadModel,
    IAmReadModelFor<PaymentAggregate, PaymentId, PaymentReceivedEvent>
    {
        public object _id { get; set; }
        public PaymentId Id { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<PaymentAggregate, PaymentId, PaymentReceivedEvent> domainEvent) => throw new NotImplementedException();
        internal Payment ToPayment() {

            return new Payment()
            {

            };

        }
    }

    [EventVersion("paymentReceived", 1)]
    public class PaymentReceivedEvent : AggregateEvent<PaymentAggregate, PaymentId>
    {

    }

    [EventVersion("appointmentPaymentRequestReceived", 1)]
    public class AppointmentPaymentRequestReceived : AggregateEvent<PaymentAggregate, PaymentId>
    {
        public string AppointmentId { get; set; }
        public decimal Ammount { get; set; }
    }


}
