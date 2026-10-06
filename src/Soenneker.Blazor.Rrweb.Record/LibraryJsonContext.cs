using System.Text.Json;
using System.Text.Json.Serialization;
using Soenneker.Blazor.Rrweb.Record.Configuration;

namespace Soenneker.Blazor.Rrweb.Record;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(RrwebRecordOptions))]
[JsonSerializable(typeof(JsonElement[]))]
internal partial class LibraryJsonContext : JsonSerializerContext;
