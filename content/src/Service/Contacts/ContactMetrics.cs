using System;
using JetBrains.Annotations;
using Nexogen.Libraries.Metrics;
using Nexogen.Libraries.Metrics.Extensions;

namespace MyVendor.MyService.Contacts
{
    /// <summary>
    /// Used to report metrics relating to contacts.
    /// </summary>
    [UsedImplicitly]
    public class ContactMetrics : IContactMetrics
    {
        private readonly IHistogram _writeDurations;
        private readonly ICounter _pokes;

        public ContactMetrics(IMetrics metrics)
        {
            _writeDurations = metrics.Histogram()
                                     .Name("myservice_contact_write_duration_seconds")
                                     .Help("Average duration of persistence write operations for contacts")
                                     .Register();
            _pokes = metrics.Counter()
                            .Name("myservice_contact_pokes_total")
                            .Help("Number of times contacts were poked")
                            .Register();
        }

        /// <inheritdoc/>
        public IDisposable Write() => _writeDurations.Timer();

        /// <inheritdoc/>
        public void Poke() => _pokes.Increment();
    }
}
