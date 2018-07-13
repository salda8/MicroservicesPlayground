using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SettingsApi.Models;
using SettingsApi.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettingsApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class EventFlowEventController : ControllerBase
    {
        public string createdAtActionName;

        public readonly IGenericMongoRepository genericRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFlowEventController"/> class.
        /// </summary>
        /// <param name="genericRepository">The generic repository.</param>
        public EventFlowEventController(IGenericMongoRepository genericRepository)
        {
            this.genericRepository = genericRepository;
            createdAtActionName = nameof(EventFlowEventController);
        }


        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query()
        {
            return Ok(await genericRepository.GetAllAsync<EventFlowEvent>(_ => true).ConfigureAwait(false));
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Find(Guid id)
        {
            var record = await genericRepository.GetByIdAsync<EventFlowEvent>(id).ConfigureAwait(false);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        /// <summary>
        /// Creates the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] EventFlowEvent record)
        {
            await genericRepository.AddOneAsync(record).ConfigureAwait(false);

            // This approach assumes you will be passing a valid action name to the controller.
            return CreatedAtAction(createdAtActionName, new { id = record.Id }, record);
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplaceDocument(string id, [FromBody] EventFlowEvent record)
        {
            if (id != record.Id)
                return BadRequest();
            
            var rec = await genericRepository.ReplaceOneAndGetAsync<EventFlowEvent>(id, record).ConfigureAwait(false);

            return Ok(rec);
        }

        /// <summary>
        /// Updates the fields.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="updates">The updates.</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFields(String id, [FromBody]Dictionary<string, object> updates)
        {
            return Ok(await genericRepository.UpdateFields<EventFlowEvent>(id, updates).ConfigureAwait(false));
           
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var document = genericRepository.GetById<EventFlowEvent>(id);

            if (await genericRepository.DeleteOneAsync(document).ConfigureAwait(false) == 0)
                return BadRequest();

            return NoContent();
        }
    }
}