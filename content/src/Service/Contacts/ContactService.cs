using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyVendor.MyService.Contacts
{
    /// <summary>
    /// Manages contacts in an address book.
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly DbContext _context;
        private readonly IContactMetrics _metrics;
        private readonly ILogger<ContactService> _logger;

        public ContactService(DbContext context, IContactMetrics metrics, ILogger<ContactService> logger)
        {
            _context = context;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task<IEnumerable<Contact>> ReadAllAsync()
        {
            var result = await ToDtos(_context.Contacts).ToListAsync();

            _logger.LogTrace("Read all contacts");
            return result;
        }

        public async Task<Contact> ReadAsync(string id)
        {
            var element = await ToDtos(_context.Contacts.Where(x => x.Id == id)).SingleOrDefaultAsync();
            if (element == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            _logger.LogTrace("Read contact {0}", id);
            return element;
        }

        private static IQueryable<Contact> ToDtos(IQueryable<ContactEntity> entities)
            => entities.Select(x => new Contact {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName});

        public async Task<Contact> CreateAsync(Contact element)
        {
            var entity = new ContactEntity();
            FromDtoToEntity(element, entity);

            using (_metrics.Write())
            {
                await _context.Contacts.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            _logger.LogDebug("Created new contact {0}", entity.Id);
            return new Contact {Id = entity.Id, FirstName = element.FirstName, LastName = element.LastName};
        }

        public async Task UpdateAsync(Contact element)
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

        private static void FromDtoToEntity(Contact dto, ContactEntity entity)
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

        public async Task<Note> ReadNoteAsync(string id)
        {
            var note = await _context.Contacts.Where(x => x.Id == id).Select(x => new Note {Content = x.Note}).SingleOrDefaultAsync();
            if (note == null) throw new KeyNotFoundException($"Contact '{id}' not found.");

            _logger.LogTrace("Read note for contact {0}", id);
            return note;
        }

        public async Task SetNoteAsync(string id, Note note)
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
