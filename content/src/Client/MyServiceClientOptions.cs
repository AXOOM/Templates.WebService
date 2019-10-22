using System;
using TypedRest;

namespace MyVendor.MyService
{
    /// <summary>
    /// Connection options for <see cref="Client"/>.
    /// </summary>
    public class MyServiceClientOptions
    {
        /// <summary>
        /// The base URI of the My Service API.
        /// </summary>
        public Uri Uri { get; set; } = default!;

        /// <summary>
        /// Options for OAuth 2.0 / OpenID Connect authentication.
        /// </summary>
        public OAuthOptions? OAuth { get; set; }
    }
}
