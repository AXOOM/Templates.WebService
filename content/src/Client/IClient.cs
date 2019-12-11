using MyVendor.MyService.Contacts;
using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;

namespace MyVendor.MyService
{
    public interface IClient : IEndpoint
    {
        /// <summary>
        /// Provides access to contacts in an address book.
        /// </summary>
        ICollectionEndpoint<Contact, ContactElementEndpoint> Contacts { get; }
    }
}
