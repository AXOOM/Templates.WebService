using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyVendor.MyService
{
    /// <summary>
    /// Common base class for controllers that conform to the "collection" REST pattern.
    /// </summary>
    /// <typeparam name="T">The DTO type used to represent elements of the collection.</typeparam>
    public abstract class CollectionController<T> : Controller
    {
        private readonly ICollectionService<T> _service;

        protected CollectionController(ICollectionService<T> service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns all elements.
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet("")]
        public async Task<IEnumerable<T>> ReadAll()
            => await _service.ReadAllAsync();

        /// <summary>
        /// Returns a specific element.
        /// </summary>
        /// <param name="id">The ID of the element to look for.</param>
        /// <response code="200">OK</response>
        /// <response code="404">Specified element not found</response>
        [HttpGet("{id}")]
        public async Task<T> Read([FromRoute] string id)
            => await _service.ReadAsync(id);

        /// <summary>
        /// Creates a new element.
        /// </summary>
        /// <param name="element">The element to create (without an ID).</param>
        /// <returns>The element that was created (with the ID).</returns>
        /// <response code="201">Created</response>
        /// <response code="400">Missing or invalid request body</response>
        [HttpPost("")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<T>> Create([FromBody] T element)
        {
            var result = await _service.CreateAsync(element);

            return CreatedAtAction(
                actionName: nameof(Read),
                routeValues: new {id = GetId(result)},
                result);
        }

        /// <summary>
        /// Updates an existing element.
        /// </summary>
        /// <param name="id">The ID of the element to update (must match the ID in <paramref name="element"/>).</param>
        /// <param name="element">The modified element.</param>
        /// <response code="204">Success</response>
        /// <response code="400">Missing or invalid request body</response>
        /// <response code="404">Specified element not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Set([FromRoute] string id, [FromBody] T element)
        {
            if (GetId(element) != id) throw new InvalidDataException($"ID in URI ({id}) must match the ID in the body ({GetId(element)}).");

            await _service.UpdateAsync(element);

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Deletes an existing element.
        /// </summary>
        /// <param name="id">The ID of the element to delete.</param>
        /// <response code="204">Success</response>
        /// <response code="404">Specified element not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task Delete([FromRoute] string id)
            => await _service.DeleteAsync(id);

        protected static string GetId(T entity)
            => GetIdMethod.Invoke(entity, null)?.ToString() ?? throw new InvalidOperationException("Entity has no ID.");

        // ReSharper disable once StaticMemberInGenericType
        private static readonly MethodInfo GetIdMethod;

        static CollectionController()
        {
            GetIdMethod = typeof(T).GetTypeInfo()
                                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                   .First(x => x.GetMethod != null && x.GetCustomAttribute<KeyAttribute>(inherit: true) != null)
                                   .GetMethod ?? throw new InvalidOperationException("Property marked with [Key] does not have a getter.");
        }
    }
}
