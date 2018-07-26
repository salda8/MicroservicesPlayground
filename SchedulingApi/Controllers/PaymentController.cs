using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Application;

namespace Payments.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentsApplicationService paymentsApplicationService;

        public PaymentController(IPaymentsApplicationService paymentsApplicationService)
        {
            this.paymentsApplicationService = paymentsApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Ping(Guid? orderId)
        {
            if (!orderId.HasValue)
                return BadRequest();

           await paymentsApplicationService.PingAsync(orderId.Value);
            return Ok();
        }
    }
}