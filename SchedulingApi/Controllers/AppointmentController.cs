using AppointmentApi;
using AppointmentApi.AppointmentModel.Commands;
using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Logs;
using EventFlow.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>> getAllQueryHandler;
        private readonly ICommandBus commandBus;
        private readonly IAppointmentService appointmentService;

        public AppointmentController(IAppointmentService appointmentService, ICommandBus commandBus, IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>> getAllQueryHandler)
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
        [HttpPost("{id}/location")]
        public async Task<IActionResult> SetLocation([FromBody]Location location, [FromRoute] string id)
        {
            await this.commandBus
                .PublishAsync(new AppointmentSetLocationCommand(new AppointmentId(id), location), new System.Threading.CancellationToken())
                ;
            return Ok();
        }

        [HttpPost("{id}/schedule")]
        public async Task<IActionResult> SetSchedule([FromBody]Schedule schedule, [FromRoute]string id)
        {

            await this.commandBus
                .PublishAsync(new AppointmentSetScheduleCommand(schedule, new AppointmentId(id)), new System.Threading.CancellationToken())
                ;
            return Ok();
        }

        [HttpPost("{id}/carservice")]
        public async Task<IActionResult> SetCarService([FromBody]CarService carService, [FromRoute]string id)
        {

            await this.commandBus
                .PublishAsync(new AppointmentSetCarServiceCommand(new AppointmentId(id), carService), new System.Threading.CancellationToken())
                ;
            return Ok();
        }


        [HttpGet("order")]
        public async Task<IActionResult> OrderAppointment()
        {
            var appointmentId = AppointmentId.New;


            await this.commandBus
                .PublishAsync(new AppointmentOrderCreateCommand(appointmentId), new System.Threading.CancellationToken())
                ;


            return CreatedAtAction(nameof(OrderAppointment), new { id = appointmentId });

            //save
            //notify
            //mng approve?
            //send confirmation
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(AppointmentReadModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> BookAppointment([FromRoute]string id)
        {
            var appointmentId = new AppointmentId(id);


            IExecutionResult result = await this.commandBus
                .PublishAsync(new AppointmentBookCommand(appointmentId), new System.Threading.CancellationToken())
                ;
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(BookAppointment), new { id = appointmentId });
            }

            return Conflict(result.ToString());



            //save
            //notify
            //mng approve?
            //send confirmation
        }
    }
}