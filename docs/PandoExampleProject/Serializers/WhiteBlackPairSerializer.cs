using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;
using Pando.Serialization.PrimitiveSerializers;

namespace PandoExampleProject.Serializers;

/// A generic NodeSerializer implementation for WhiteBlackPairs that contain a specific primitive data type.
/// The white and black values of the WhiteBlackPair are serialized by a provided primitive serializer.
///
/// The size of this node depends on the size of the elements stored within it, so the size must be calculated
/// dynamically for variable size primitives.
internal class PrimitiveWhiteBlackPairSerializer<T> : INodeSerializer<WhiteBlackPair<T>>
{
	private readonly IPrimitiveSerializer<T> _innerSerializer;

	public PrimitiveWhiteBlackPairSerializer(IPrimitiveSerializer<T> innerSerializer)
	{
		_innerSerializer = innerSerializer;
		NodeSize = innerSerializer.ByteCount * 2;
	}

	public int? NodeSize { get; }

	public int NodeSizeForObject(WhiteBlackPair<T> obj)
		=> NodeSize
			?? _innerSerializer.ByteCountForValue(obj.WhiteValue)
			+ _innerSerializer.ByteCountForValue(obj.BlackValue);

	public void Serialize(WhiteBlackPair<T> obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		_innerSerializer.Serialize(obj.WhiteValue, ref writeBuffer);
		_innerSerializer.Serialize(obj.BlackValue, ref writeBuffer);
	}

	public WhiteBlackPair<T> Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var whiteValue = _innerSerializer.Deserialize(ref readBuffer);
		var blackValue = _innerSerializer.Deserialize(ref readBuffer);
		return new WhiteBlackPair<T>(whiteValue, blackValue);
	}
}

/// A generic NodeSerializer implementation for WhiteBlackPairs that contain other nodes.
/// In this case we know for certain the size of the binary representation is 16 bytes since it will contain the hashes of the two child nodes.
/// We don't need to know anything about the size of the contained nodes themselves, just that they can be identified by a hash.
internal class NodeWhiteBlackPairSerializer<TNode> : INodeSerializer<WhiteBlackPair<TNode>>
{
	private readonly INodeSerializer<TNode> _memberSerializer;

	public NodeWhiteBlackPairSerializer(INodeSerializer<TNode> memberSerializer)
	{
		_memberSerializer = memberSerializer;
	}

	private const int SIZE = sizeof(ulong) * 2;
	public int? NodeSize => SIZE;
	public int NodeSizeForObject(WhiteBlackPair<TNode> obj) => SIZE;

	public void Serialize(WhiteBlackPair<TNode> obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var (whiteValue, blackValue) = obj;

		var whiteHash = _memberSerializer.SerializeToHash(whiteValue, dataSink);
		var blackHash = _memberSerializer.SerializeToHash(blackValue, dataSink);

		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(0, sizeof(ulong)), whiteHash);
		BinaryPrimitives.WriteUInt64LittleEndian(writeBuffer.Slice(sizeof(ulong), sizeof(ulong)), blackHash);
	}

	public WhiteBlackPair<TNode> Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var whiteHash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(0, sizeof(ulong)));
		var blackHash = BinaryPrimitives.ReadUInt64LittleEndian(readBuffer.Slice(sizeof(ulong), sizeof(ulong)));

		var whiteValue = _memberSerializer.DeserializeFromHash(whiteHash, dataSource);
		var blackValue = _memberSerializer.DeserializeFromHash(blackHash, dataSource);

		return new WhiteBlackPair<TNode>(whiteValue, blackValue);
	}
}
