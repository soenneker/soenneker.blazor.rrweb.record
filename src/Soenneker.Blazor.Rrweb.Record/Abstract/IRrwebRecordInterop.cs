using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.Rrweb.Record.Configuration;

namespace Soenneker.Blazor.Rrweb.Record.Abstract;

/// <summary>Records browser sessions using rrweb. Register as scoped and call after interactive rendering.</summary>
/// <remarks>Only one recorder may run per document. Events remain in browser memory until drained or disposed.
/// Drain regularly for long sessions, persist every batch in order, and keep the initial full snapshot for replay.
/// This service does not upload recordings. Disposal stops recording and releases buffered events.</remarks>
public interface IRrwebRecordInterop : IAsyncDisposable
{
    /// <summary>Loads pinned rrweb resources. The first call selects CDN or bundled local assets for this scope.</summary>
    ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>Starts a new recording and clears the previous buffer. Throws if a recorder is already active.</summary>
    ValueTask Start(RrwebRecordOptions? options = null, bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>Stops capturing events, preserving the buffer. Repeated calls are harmless.</summary>
    ValueTask Stop(CancellationToken cancellationToken = default);

    /// <summary>Returns buffered rrweb events in emission order, optionally removing the returned events from memory.</summary>
    /// <remarks>Cleared batches must be concatenated in order for replay. On Blazor Server configure the circuit's
    /// MaximumReceiveMessageSize for the expected snapshot size; a full DOM snapshot can exceed the default limit.</remarks>
    ValueTask<JsonElement[]> GetEvents(bool clear = false, CancellationToken cancellationToken = default);

    /// <summary>Returns whether this service owns the active recording.</summary>
    ValueTask<bool> IsRecording(CancellationToken cancellationToken = default);

    /// <summary>Adds an application-defined event to the active recording.</summary>
    ValueTask AddCustomEvent(string tag, JsonElement payload, CancellationToken cancellationToken = default);

    /// <summary>Captures a full snapshot during recording, optionally marking it as a checkout.</summary>
    ValueTask TakeFullSnapshot(bool isCheckout = false, CancellationToken cancellationToken = default);
}
