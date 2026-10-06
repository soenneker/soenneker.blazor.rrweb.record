using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Blazor.Rrweb.Record.Abstract;
using Soenneker.Blazor.Rrweb.Record.Configuration;

namespace Soenneker.Blazor.Rrweb.Record;

public sealed class RrwebRecordInterop : IRrwebRecordInterop
{
    private const string _modulePath = "./_content/Soenneker.Blazor.Rrweb.Record/js/rrwebrecordinterop.js";
    private readonly IResourceLoader _resourceLoader;
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly AsyncInitializer<bool> _initializer;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IJSObjectReference? _interop;
    private bool _disposed;

    public RrwebRecordInterop(IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;
        _initializer = new AsyncInitializer<bool>(InitializeResources);
    }

    private async ValueTask InitializeResources(bool useCdn, CancellationToken cancellationToken)
    {
        await _resourceLoader.LoadScriptAndWaitForVariable(useCdn
                ? "https://cdn.jsdelivr.net/npm/@rrweb/record@2.1.7/dist/record.umd.min.cjs"
                : "_content/Soenneker.Blazor.Rrweb.Record/js/rrweb-record.min.js",
            "rrwebRecord", useCdn ? "sha384-IpxzJjdtg6YdkOPtTQCrD+rvnylSZAVqNCrgLDEjBav6r5bDRY+SfA3OExSLSojq" : null, cancellationToken: cancellationToken);
        IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
        _interop = await module.InvokeAsync<IJSObjectReference>("createInterop", cancellationToken);
    }

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            await _initializer.Init(useCdn, cancellationToken);
        }
        finally { _gate.Release(); }
    }

    public async ValueTask Start(RrwebRecordOptions? options = null, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        options ??= new RrwebRecordOptions();
        if (options.CheckoutEveryNth is <= 0 || options.CheckoutEveryNms is <= 0)
            throw new ArgumentOutOfRangeException(nameof(options), "Checkout intervals must be positive.");
        await Initialize(useCdn, cancellationToken);
        await InvokeVoid("start", cancellationToken, JsonSerializer.Serialize(options, LibraryJsonContext.Default.RrwebRecordOptions));
    }

    public ValueTask Stop(CancellationToken cancellationToken = default) => InvokeVoid("stop", cancellationToken);

    public async ValueTask<JsonElement[]> GetEvents(bool clear = false, CancellationToken cancellationToken = default)
    {
        string json = await Invoke<string>("getEvents", cancellationToken, clear);
        return JsonSerializer.Deserialize(json, LibraryJsonContext.Default.JsonElementArray)!;
    }

    public ValueTask<bool> IsRecording(CancellationToken cancellationToken = default) => Invoke<bool>("isRecording", cancellationToken);

    public ValueTask AddCustomEvent(string tag, JsonElement payload, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        return InvokeVoid("addCustomEvent", cancellationToken, tag, payload.GetRawText());
    }

    public ValueTask TakeFullSnapshot(bool isCheckout = false, CancellationToken cancellationToken = default)
        => InvokeVoid("takeFullSnapshot", cancellationToken, isCheckout);

    private async ValueTask InvokeVoid(string method, CancellationToken cancellationToken, params object?[] args)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_interop is null)
                throw new InvalidOperationException("Initialize must be called before this operation.");
            await _interop.InvokeVoidAsync(method, cancellationToken, args);
        }
        finally { _gate.Release(); }
    }

    private async ValueTask<T> Invoke<T>(string method, CancellationToken cancellationToken, params object?[] args)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_interop is null)
                throw new InvalidOperationException("Initialize must be called before this operation.");
            return await _interop.InvokeAsync<T>(method, cancellationToken, args);
        }
        finally { _gate.Release(); }
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                if (_interop is not null)
                {
                    try { await _interop.InvokeVoidAsync("dispose"); }
                    finally { await _interop.DisposeAsync(); }
                }
            }
            catch (JSDisconnectedException) { }
            finally { await _initializer.DisposeAsync(); }
            // The module import service owns its shared cached module reference.
        }
        finally { _gate.Release(); }
    }
}

