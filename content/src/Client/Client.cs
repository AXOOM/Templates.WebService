using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MyVendor.MyService.Contacts;
using TypedRest.Endpoints;
using TypedRest.Endpoints.Generic;

namespace MyVendor.MyService
{
    /// <summary>
    /// Provides a type-safe client for the My Service REST API.
    /// </summary>
    public class Client : OAuthEntryEndpoint, IClient
    {
        /// <summary>
        /// Creates a new My Service Client.
        /// </summary>
        /// <param name="options">Options for connecting to the My Service API.</param>
        public Client(IOptions<MyServiceClientOptions> options)
            : base(options.Value.Uri, options.Value.OAuth)
        {}

        /// <summary>
        /// Creates a new My Service Client using a custom <see cref="HttpMessageHandler"/>. This is usually used for custom authentication schemes (e.g. client certificates) or testing.
        /// </summary>
        /// <param name="uri">The base URI of the My Service API.</param>
        /// <param name="httpMessageHandler">The HTTP message handler used to communicate with the remote element.</param>
        internal Client(Uri uri, HttpMessageHandler httpMessageHandler)
            : base(uri, oAuthOptions: null, httpMessageHandler)
        {}

        /// <summary>
        /// Provides access to contacts in an address book.
        /// </summary>
        public ICollectionEndpoint<Contact, ContactElementEndpoint> Contacts
            => new CollectionEndpoint<Contact, ContactElementEndpoint>(this, relativeUri: "contacts");
    }
}
