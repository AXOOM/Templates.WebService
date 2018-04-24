using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Axoom.MyService.Contacts
{
    /// <summary>
    /// Manages contacts in an address book.
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly MyServiceDbContext _context;
        private readonly IContactMetrics _metrics;
        private readonly ILogger<ContactService> _logger;

        public ContactService(MyServiceDbContext context, IContactMetrics metrics, ILogger<ContactService> logger)
        {
            _context = context;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task<IEnumerable<ContactDto>> ReadAllAsync()
        {
            var result = await ToDtos(_context.Contacts).ToListAsync();

            _logger.LogTrace("Read all contacts");
            return result;
        }

        public async Task<ContactDto> ReadAsync(string id)
        {
            var element = await ToDtos(_context.Contacts.Where(x => x.Id == id)).SingleOrDefaultAsync();
            if (element == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            _logger.LogTrace("Read contact {0}", id);
            return element;
        }

        private static IQueryable<ContactDto> ToDtos(IQueryable<ContactEntity> entities)
            => entities.Select(x => new ContactDto {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName});

        public async Task<ContactDto> CreateAsync(ContactDto element)
        {
            var entity = new ContactEntity();
            FromDtoToEntity(element, entity);

            using (_metrics.Write())
            {
                await _context.Contacts.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            _logger.LogDebug("Created new contact {0}", entity.Id);
            return new ContactDto {Id = entity.Id, FirstName = element.FirstName, LastName = element.LastName};
        }

        public async Task UpdateAsync(ContactDto element)
        {
            var entity = await _context.Contacts.FindAsync(element.Id);
            if (entity == null) throw new KeyNotFoundException($"Contact '{element.Id}' not found.");

            FromDtoToEntity(element, entity);

            using (_metrics.Write())
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }

            _logger.LogDebug("Updated contact {0}", element.Id);
        }

        private static void FromDtoToEntity(ContactDto dto, ContactEntity entity)
        {
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Contacts.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            _context.Contacts.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Deleted contact {0}", id);
        }

        public async Task<NoteDto> ReadNoteAsync(string id)
        {
            var note = await _context.Contacts.Where(x => x.Id == id).Select(x => new NoteDto {Content = x.Note}).SingleOrDefaultAsync();
            if (note == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            _logger.LogTrace("Read note for contact {0}", id);
            return note;
        }

        public async Task SetNoteAsync(string id, NoteDto note)
        {
            var entity = await _context.Contacts.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            entity.Note = note.Content;

            using (_metrics.Write())
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }

            _logger.LogDebug("Set note for contact {0}", id);
        }

        public async Task PokeAsync(string id)
        {
            var entity = await _context.Contacts.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            entity.Pokes.Add(new PokeEntity {Timestamp = DateTime.UtcNow});
            await _context.SaveChangesAsync();

            _metrics.Poke();
            _logger.LogDebug("Poked contact {0}", id);
        }
    }
}
