using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

public class ImmutableArraySerializer<TNode> : INodeSerializer<ImmutableArray<TNode>>
{
	public int? NodeSize => null;

	private readonly INodeSerializer<TNode> _elementSerializer;

	public ImmutableArraySerializer(INodeSerializer<TNode> elementSerializer)
	{
		_elementSerializer = elementSerializer;
	}

	public ulong Serialize(ImmutableArray<TNode> obj, INodeDataSink dataSink)
	{
		var len = obj.Length;
		Span<byte> buffer = stackalloc byte[sizeof(ulong) * len];

		for (int i = 0; i < len; i++)
		{
			var hash = _elementSerializer.Serialize(obj[i], dataSink);
			BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(i * sizeof(ulong), sizeof(ulong)), hash);
		}

		return dataSink.AddNode(buffer);
	}

	public ImmutableArray<TNode> Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var len = bytes.Length / sizeof(ulong);
		var arrayBuilder = ImmutableArray.CreateBuilder<TNode>(len);

		for (int i = 0; i < len; i++)
		{
			var hash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong) * i, sizeof(ulong)));
			arrayBuilder.Add(dataSource.GetNode(hash, _elementSerializer));
		}

		return arrayBuilder.ToImmutable();
	}
}
