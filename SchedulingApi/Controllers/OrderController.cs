using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Application;

namespace Payments.WebApi.Controllers
{
    public class AddProductToOrderModel
    {
        public Guid OrderId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }

    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrdersApplicationService ordersApplicationService;

        public OrderController(IOrdersApplicationService ordersApplicationService)
        {
            this.ordersApplicationService = ordersApplicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest();

            var orderId = await ordersApplicationService.CreateOrderAsync(username);
            return Ok(orderId);
        }

        [HttpPut]
        public async Task<IActionResult> AddProduct([FromBody]AddProductToOrderModel request)
        {
            await ordersApplicationService.AddProductAsync(request.OrderId, request.Name, request.Count, request.Price);
            return Ok();
        }

        [HttpPatch()]
        public async Task<IActionResult> BeginPayment(Guid orderId)
        {
            await ordersApplicationService.ProcessToPaymentAsync(orderId);
            return Ok();
        }
    }
}