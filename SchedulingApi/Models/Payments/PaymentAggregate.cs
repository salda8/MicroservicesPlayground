﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using AppointmentApi.Models;
using Payments.Domain.Payments.Events;
using Payments.Domain.Payments.Providers;
using Payments.Domain.Payments.Snapshots;
using Payments.Domain.Common.Aggregate;
using EventFlow.Snapshots.Stores;
using EventFlow.MongoDB.EventStore;
using EventFlow.EventStores;

namespace Payments.Domain.Payments
{
    public class PaymentAggregate : SnapshotAggregateRootWithSequenceStore<PaymentAggregate, PaymentId, PaymentAggregateSnapshot>
    {
        public const int SnapshotEveryVersion = 10;

        private readonly IPaymentProviderFactory paymentProviderFactory;
        public PaymentState PaymentState { get; } = new PaymentState();

        public int StateMachineState
        {
            get => (int)PaymentState.Status;
            set => PaymentState.Status = (PaymentStatus)value;
        }

        public PaymentAggregate(IPaymentProviderFactory paymentProviderFactory, PaymentId id, ISnapshotPersistence snapshotPersistence, IMongoDbEventSequenceStore sequenceStore, ISnapshotStrategy snapshotStrategy, IEventStore eventStore, ISnapshotStore snapshotStore) : base(snapshotPersistence, sequenceStore, snapshotStrategy, eventStore, snapshotStore, id)
        {
            this.paymentProviderFactory = paymentProviderFactory;
            Register(PaymentState);
        }

        public async Task<Uri> BeginPaymentProcessAsync(Guid orderId, string username, decimal totalPrice)
        {
            var paymentProvider = paymentProviderFactory.GetPaymentProvider();
            var redirectUrl = await paymentProvider.BeginPaymentProcessAsync(new BeginPaymentProcessModel(orderId, username, totalPrice));

            Emit(new PaymentProcessStarted(orderId, username, totalPrice, redirectUrl));

            return PaymentState.RedirectUrl;
        }

        public void CancelPaymentProcess()
        {
            Emit(new PaymentProcessCancelled(PaymentState.OrderId), this.MetadataFor(new { PaymentState.OrderId }));
        }
        public void CompletePaymentProcess()
        {
            Emit(new PaymentProcessCompleted(PaymentState.OrderId), this.MetadataFor(new { PaymentState.OrderId }));
        }

        public void Ping()
        {
            Emit(new PaymentProcessPinged());
        }

        protected override Task<PaymentAggregateSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new PaymentAggregateSnapshot(PaymentState));
        }

        protected override Task LoadSnapshotAsync(PaymentAggregateSnapshot snapshot, ISnapshotMetadata metadata,
            CancellationToken cancellationToken)
        {
            PaymentState.Load(snapshot.PaymentState);
            return Task.CompletedTask;
        }
    }
}