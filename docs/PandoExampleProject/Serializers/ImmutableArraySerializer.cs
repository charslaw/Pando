using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoExampleProject.Serializers;

public class ImmutableArraySerializer<TNode> : INodeSerializer<ImmutableArray<TNode>>
{
	private readonly INodeSerializer<TNode> _elementSerializer;

	public ImmutableArraySerializer(INodeSerializer<TNode> elementSerializer)
	{
		_elementSerializer = elementSerializer;
	}

	public int? NodeSize => null;

	public int NodeSizeForObject(ImmutableArray<TNode> array) => array.Length * sizeof(ulong);

	public void Serialize(ImmutableArray<TNode> array, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		for (int i = 0; i < array.Length; i++)
		{
			var hash = _elementSerializer.SerializeToHash(array[i], dataSink);
			BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(i * sizeof(ulong), sizeof(ulong)), hash);
		}
	}

	public ImmutableArray<TNode> Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var len = readBuffer.Length / sizeof(ulong);
		var arrayBuilder = ImmutableArray.CreateBuilder<TNode>(len);

		for (int i = 0; i < len; i++)
		{
			var hash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(sizeof(ulong) * i, sizeof(ulong)));
			arrayBuilder.Add(_elementSerializer.DeserializeFromHash(hash, dataSource));
		}

		return arrayBuilder.ToImmutable();
	}
}
