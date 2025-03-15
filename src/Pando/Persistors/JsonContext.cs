using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pando.Repositories;

namespace Pando.Persistors;

[JsonSourceGenerationOptions(
	Converters = [typeof(NodeIdConverter), typeof(SnapshotIdConverter), typeof(ByteArrayJsonArrayConverter)],
	WriteIndented = true
)]
[JsonSerializable(typeof(Dictionary<NodeId, byte[]>))]
[JsonSerializable(typeof(Dictionary<SnapshotId, (SnapshotId, SnapshotId, NodeId)>))]
internal partial class JsonContext : JsonSerializerContext;

internal class NodeIdConverter : JsonConverter<NodeId>
{
	public override NodeId ReadAsPropertyName(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options
	)
	{
		return Read(ref reader, typeToConvert, options);
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, NodeId value, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[NodeId.SIZE * 2]; // hex representation is 2 chars per byte
		value.CopyHashStringTo(ref buffer);
		writer.WritePropertyName(buffer);
	}

	public override NodeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[NodeId.SIZE * 2]; // hex representation is 2 chars per byte
		reader.CopyString(buffer);
		return NodeId.FromHashString(buffer);
	}

	public override void Write(Utf8JsonWriter writer, NodeId value, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[NodeId.SIZE * 2]; // hex representation is 2 chars per byte
		value.CopyHashStringTo(ref buffer);
		writer.WriteStringValue(buffer);
	}
}

internal class SnapshotIdConverter : JsonConverter<SnapshotId>
{
	public override SnapshotId ReadAsPropertyName(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options
	)
	{
		return Read(ref reader, typeToConvert, options);
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, SnapshotId value, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[SnapshotId.SIZE * 2]; // hex representation is 2 chars per byte
		value.CopyHashStringTo(ref buffer);
		writer.WritePropertyName(buffer);
	}

	public override SnapshotId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[SnapshotId.SIZE * 2]; // hex representation is 2 chars per byte
		reader.CopyString(buffer);
		return SnapshotId.FromHashString(buffer);
	}

	public override void Write(Utf8JsonWriter writer, SnapshotId value, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[SnapshotId.SIZE * 2]; // hex representation is 2 chars per byte
		value.CopyHashStringTo(ref buffer);
		writer.WriteStringValue(buffer);
	}
}

internal class ByteArrayJsonArrayConverter : JsonConverter<byte[]>
{
	public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var str = reader.GetString()!;
		return Convert.FromHexString(str);
	}

	public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
	{
		Span<char> buffer = stackalloc char[value.Length * 2];
		if (Convert.TryToHexString(value, buffer, out var charsWritten))
		{
			buffer = buffer[..charsWritten];
		}
		else
		{
			throw new InvalidOperationException(
				"Allocated buffer was not big enough for hex string representation of byte array."
			);
		}
		writer.WriteStringValue(buffer);
	}
}
