using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.Configuration;
using EventFlow.Logs;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.ReadStores;
using MongoDB.Bson.Serialization.Attributes;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi
{

    //public class AppointmentReadStoreManager : ReadStoreManager<IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel>, AppointmentInsertReadModel>
    //{
    //    protected AppointmentReadStoreManager(ILog log, IResolver resolver, IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel> readModelStore, IReadModelDomainEventApplier readModelDomainEventApplier, IReadModelFactory<AppointmentInsertReadModel> readModelFactory)
    //        : base(log, resolver, readModelStore, readModelDomainEventApplier, readModelFactory)
    //    {
    //    }

    //    protected override IReadOnlyCollection<ReadModelUpdate> BuildReadModelUpdates(IReadOnlyCollection<IDomainEvent> domainEvents)
    //    {
    //        // ReadModelFactory.CreateAsync("sdasd");
    //        throw new NotImplementedException();
    //    }

    //    protected override Task<ReadModelEnvelope<AppointmentInsertReadModel>> UpdateAsync(IReadModelContext readModelContext,
    //        IReadOnlyCollection<IDomainEvent> domainEvents, ReadModelEnvelope<AppointmentInsertReadModel> readModelEnvelope, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}