using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyVendor.MyService.Contacts
{
    /// <summary>
    /// Provides access to contacts in an address book.
    /// </summary>
    [ApiController, Route("contacts"), Authorize]
    public class ContactsController : CollectionController<Contact>
    {
        private readonly IContactService _service;

        public ContactsController(IContactService service)
            : base(service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns the note for a specific contact.
        /// </summary>
        /// <param name="id">The ID of the contact to get the note for.</param>
        /// <response code="200">OK</response>
        /// <response code="404">Specified contact not found</response>
        [HttpGet("{id}/note")]
        [ScopeAuthorize(Scopes.Notes)]
        public async Task<Note> ReadNote([FromRoute] string id)
            => await _service.ReadNoteAsync(id);

        /// <summary>
        /// Sets a note for a specific contact.
        /// </summary>
        /// <param name="id">The ID of the contact to set the note for.</param>
        /// <param name="note">The note to set</param>
        /// <response code="200">OK</response>
        /// <response code="400">Missing or invalid request body</response>
        /// <response code="404">Specified contact not found</response>
        [HttpPut("{id}/note")]
        [ScopeAuthorize(Scopes.Notes)]
        public async Task<IActionResult> SetNote([FromRoute] string id, [FromBody] Note note)
        {
            await _service.SetNoteAsync(id, note);

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Pokes a contact.
        /// </summary>
        /// <param name="id">The ID of the contact to poke.</param>
        /// <response code="204">Success</response>
        /// <response code="404">Specified contact not found</response>
        [HttpPost("{id}/poke")]
        [ProducesResponseType(204)]
        [ScopeAuthorize(Scopes.Poke)]
        public async Task<IActionResult> Poke([FromRoute] string id)
        {
            await _service.PokeAsync(id);

            return NoContent();
        }
    }
}
