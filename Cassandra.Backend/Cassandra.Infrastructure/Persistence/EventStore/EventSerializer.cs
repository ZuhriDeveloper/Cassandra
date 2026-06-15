using System.Text.Json;
using System.Text.Json.Serialization;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;

namespace Cassandra.Infrastructure.Persistence.EventStore;

public static class EventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new DealerIdConverter(),
        },
    };

    public static string Serialize(IDomainEvent evt) =>
        JsonSerializer.Serialize(evt, evt.GetType(), Options);

    public static IDomainEvent Deserialize(string eventType, string data)
    {
        var type = EventTypeRegistry.Resolve(eventType);
        return (IDomainEvent)JsonSerializer.Deserialize(data, type, Options)!;
    }

    // ── Custom converters for strongly-typed IDs ──────────────────────────────

    private sealed class DealerIdConverter : JsonConverter<DealerId>
    {
        public override DealerId Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions o)
            => DealerId.From(reader.GetGuid());
        public override void Write(Utf8JsonWriter writer, DealerId value, JsonSerializerOptions o)
            => writer.WriteStringValue(value.Value);
    }
}
