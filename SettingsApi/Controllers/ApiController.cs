using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDbGenericRepository.Models;
using SettingsApi.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettingsApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public abstract class ApiController<TDocument> : ControllerBase where TDocument : class, IDocument
    {
        protected string createdAtActionName;

        protected readonly IGenericMongoRepository genericRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiController{TDocument}"/> class.
        /// </summary>
        /// <param name="genericRepository">The generic repository.</param>
        protected ApiController(IGenericMongoRepository genericRepository)
        {
            this.genericRepository = genericRepository;
        }

        /// <summary>
        /// Pass the intended method name to createdAtActionName
        /// argument.
        /// Use of the "nameof(..)" keyword is advised.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="createdAtActionName">Name of the created at action.</param>
        protected ApiController(IGenericMongoRepository repository, string createdAtActionName)
        {
            this.genericRepository = repository;
            this.createdAtActionName = createdAtActionName;
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query()
        {
            return Ok(await genericRepository.GetAllAsync<TDocument>(_ => true));
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Find(Guid id)
        {
            var record = await genericRepository.GetByIdAsync<TDocument>(id);
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
        public async Task<IActionResult> Create([FromBody] TDocument record)
        {
            await genericRepository.AddOneAsync(record);

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
        public async Task<IActionResult> ReplaceDocument(string id, [FromBody] TDocument record)
        {
            if (id != record.Id.ToString())
                return BadRequest();
            
            var rec = await genericRepository.ReplaceOneAndGetAsync<TDocument>(id, record,null);

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
            return Ok(await genericRepository.UpdateFields<TDocument>(id, updates,null));
           
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var document = genericRepository.GetById<TDocument>(id);

            if (await genericRepository.DeleteOneAsync(document) == 0)
                return BadRequest();

            return NoContent();
        }
    }
}