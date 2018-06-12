using AppointmentApi;
using EventFlow;
using EventFlow.Logs;
using EventFlow.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>> getAllQueryHandler;
        private readonly ICommandBus commandBus;
        private readonly IAppointmentService appointmentService;

        public AppointmentController(IAppointmentService appointmentService, ICommandBus commandBus, ILog log, IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>> getAllQueryHandler)
        {
            this.appointmentService = appointmentService;
            this.commandBus = commandBus;
            this.getAllQueryHandler = getAllQueryHandler;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(AppointmentReadModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await getAllQueryHandler.ExecuteQueryAsync(new GetAllAppointmentsQuery(), new CancellationToken());
            if (appointments.Any())
            {
                return Ok(appointments);
            }
            
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> OrderAppointment(AppointmentOrder appointment)
        {
            var appointmentId = new AppointmentId(Guid.NewGuid().ToString());
            await this.commandBus
                .PublishAsync(new AppointmentBookCommand(appointmentId, appointment), new System.Threading.CancellationToken())
                .ConfigureAwait(false);

            appointmentService.AddNewAppointment();
            return Ok();

            //save
            //notify
            //mng approve?
            //send confirmation
        }
    }
}