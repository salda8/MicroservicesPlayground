// using EventFlow.Aggregates;
// using EventFlow.Sagas;
// using EventFlow.ValueObjects;
// using SchedulingApi.Controllers;
// using System;
// using System.Threading;
// using System.Threading.Tasks;
 namespace AppointmentApi.Sagas
{
//     public class AppointmentSagaLocator : BaseIdSagaLocator
//     {
//         public AppointmentSagaLocator() : base(id => new AppointmentSagaId($"{nameof(AppointmentSaga)}-{id}"))
//         {
//         }

//         // public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
//         // {
//         //     var appointmentId = domainEvent.Metadata["aggregate_id"];
//         //     var appointmentSagaId = new AppointmentSagaId($"appointmentSaga-{appointmentId}");

//         //     return Task.FromResult<ISagaId>(appointmentSagaId);
//         // }
//     }

//     public class AppointmentSagaId : SingleValueObject<string>, ISagaId
//     {
//         public AppointmentSagaId(string id) : base(id)
//         {

//         }


//     }

//     public abstract class BaseIdSagaLocator : ISagaLocator
//     {
//         private readonly Func<string, ISagaId> sagaIdGenerator;
//         protected const string MetadataIdKey = "aggregate_id";

//         protected BaseIdSagaLocator(Func<string, ISagaId> sagaIdGenerator)
//         {
//             this.sagaIdGenerator = sagaIdGenerator;
//             //MetadataKey = metadataKey;
//         }

//         public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
//         {
//             var id = domainEvent.Metadata[MetadataIdKey];
//             return Task.FromResult(sagaIdGenerator(id));
//         }
//     }
 }