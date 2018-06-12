using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CarServicesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarservicesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAllAvailableCarServices()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetCarServices(int id)
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}