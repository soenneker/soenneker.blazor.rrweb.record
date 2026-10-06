namespace Soenneker.Blazor.Rrweb.Record.Configuration;

/// <summary>Serializable options for rrweb record.</summary>
public sealed class RrwebRecordOptions
{
    /// <summary>Masks all input values by default. Set false to use rrweb input masking defaults.</summary>
    public bool MaskAllInputs { get; set; } = true;

    /// <summary>CSS class whose elements are replaced by placeholders.</summary>
    public string BlockClass { get; set; } = "rr-block";

    /// <summary>Additional CSS selector whose elements are blocked.</summary>
    public string? BlockSelector { get; set; }

    /// <summary>CSS class whose input events are ignored.</summary>
    public string IgnoreClass { get; set; } = "rr-ignore";

    /// <summary>Additional CSS selector whose input events are ignored.</summary>
    public string? IgnoreSelector { get; set; }

    /// <summary>CSS class whose text is masked.</summary>
    public string MaskTextClass { get; set; } = "rr-mask";

    /// <summary>Additional CSS selector whose text is masked.</summary>
    public string? MaskTextSelector { get; set; }

    /// <summary>Captures stylesheet contents for replay.</summary>
    public bool InlineStylesheet { get; set; } = true;

    /// <summary>Collects fonts loaded through the FontFace API.</summary>
    public bool CollectFonts { get; set; } = false;

    /// <summary>Enables recording of canvas operations.</summary>
    public bool RecordCanvas { get; set; } = false;

    /// <summary>Records cross-origin frames when rrweb is also installed inside them.</summary>
    public bool RecordCrossOriginIframes { get; set; } = false;

    /// <summary>Captures a new full snapshot after this many incremental events.</summary>
    public int? CheckoutEveryNth { get; set; }

    /// <summary>Captures a new full snapshot after this many milliseconds.</summary>
    public int? CheckoutEveryNms { get; set; }
}
