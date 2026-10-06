using Soenneker.Blazor.Rrweb.Record.Abstract;
using Soenneker.Tests.HostedUnit;
using System;
using System.Threading.Tasks;

using Soenneker.Blazor.Rrweb.Record.Configuration;

namespace Soenneker.Blazor.Rrweb.Record.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class RrwebRecordInteropTests : HostedUnitTest
{
    private readonly IRrwebRecordInterop _blazorlibrary;

    public RrwebRecordInteropTests(Host host) : base(host)
    {
        _blazorlibrary = Resolve<IRrwebRecordInterop>(true);
    }

    [Test]
    public async Task Registrar_resolves_scoped_interop()
    {
        await Assert.That(_blazorlibrary).IsTypeOf<RrwebRecordInterop>();
    }

    [Test]
    public async Task Invalid_checkout_interval_is_rejected_before_loading_resources()
    {
        Func<Task> action = async () => await _blazorlibrary.Start(new RrwebRecordOptions { CheckoutEveryNth = 0 });
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(action);
    }

    [Test]
    public async Task Blank_custom_event_tag_is_rejected()
    {
        Func<Task> action = async () => await _blazorlibrary.AddCustomEvent(" ", default);
        await Assert.ThrowsAsync<ArgumentException>(action);
    }

    [Test]
    public async Task Reading_before_initialization_fails_clearly()
    {
        Func<Task> action = async () => await _blazorlibrary.GetEvents();
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }
}

