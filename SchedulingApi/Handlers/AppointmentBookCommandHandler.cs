using AppointmentApi;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Core;
using Microsoft.AspNetCore.Mvc;
using EventFlow.MongoDB;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.MongoDB.ReadStores;

namespace SchedulingApi.Controllers
{

    public class AppointmentBookCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentBookCommand>
    {
        private readonly IMongoDbInsertOnlyReadModelStore<AppointmentReadModel> mongoDbInsert;

        public AppointmentBookCommandHandler(IMongoDbInsertOnlyReadModelStore<AppointmentReadModel> mongoDbInsert)
        {
            this.mongoDbInsert = mongoDbInsert;
        }

        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentBookCommand command, CancellationToken cancellationToken)
        {
            var result = aggregate.BookAppointment(command.AggregateId.Value);
            //mongoDbInsert.UpdateAsync()
            return  Task.FromResult(result);
        }
    }
}