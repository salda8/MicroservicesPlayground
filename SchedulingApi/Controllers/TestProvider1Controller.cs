using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Application;

namespace Payments.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestProvider1Controller : ControllerBase
    {
        private readonly IPaymentsApplicationService paymentsApplicationService;

        public TestProvider1Controller(IPaymentsApplicationService paymentsApplicationService)
        {
            this.paymentsApplicationService = paymentsApplicationService;
        }

        [HttpPatch]
        public async Task<IActionResult> Cancel(Guid? orderId)
        {
            if (!orderId.HasValue)
                return BadRequest();

            await paymentsApplicationService.CancelPaymentProcessAsync(orderId.Value);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Complete(Guid? orderId)
        {
            if (!orderId.HasValue)
                return BadRequest();

            await paymentsApplicationService.CompletePaymentProcessAsync(orderId.Value);
            return Ok();
        }
    }
}