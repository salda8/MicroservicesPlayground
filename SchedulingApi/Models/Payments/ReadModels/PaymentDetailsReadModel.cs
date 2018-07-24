using System;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.ReadStores;
using MongoDB.Bson.Serialization.Attributes;
using Payments.Domain.Payments.Events;

namespace Payments.Domain.Payments.ReadModels
{
    [MongoDbCollectionName("paymentDetails")]
    public class PaymentDetailsReadModel : IMongoDbInsertOnlyReadModel, IMongoDbReadModel,
        IAmReadModelFor<PaymentAggregate, PaymentId, PaymentProcessStarted>,
        IAmReadModelFor<PaymentAggregate, PaymentId, PaymentProcessCancelled>,
        IAmReadModelFor<PaymentAggregate, PaymentId, PaymentProcessCompleted>
    {
        public string PaymentId { get; private set; }
        public Guid OrderId { get; private set; }
        public string Username { get; private set; }
        public decimal TotalPrice { get; private set; }
        public PaymentStatus Status { get; private set; }


        [BsonId]
        public object _id { get; set; }
        public long? _version { get; set; }

        string IMongoDbReadModel._id => (string)_id;

        public void Apply(IReadModelContext context, IDomainEvent<PaymentAggregate, PaymentId, PaymentProcessStarted> domainEvent)
        {
            PaymentId = domainEvent.AggregateIdentity.Value;
            Status = domainEvent.AggregateEvent.Status;
            TotalPrice = domainEvent.AggregateEvent.TotalPrice;
            Username = domainEvent.AggregateEvent.Username;
            OrderId = domainEvent.AggregateEvent.OrderId;
        }

        public void Apply(IReadModelContext context, IDomainEvent<PaymentAggregate, PaymentId, PaymentProcessCancelled> domainEvent)
        {
            Status = domainEvent.AggregateEvent.Status;
        }

        public void Apply(IReadModelContext context, IDomainEvent<PaymentAggregate, PaymentId, PaymentProcessCompleted> domainEvent)
        {
            Status = domainEvent.AggregateEvent.Status;
        }
    }
}