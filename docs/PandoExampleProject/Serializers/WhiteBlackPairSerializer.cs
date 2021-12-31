using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

/// A specific implementation of a WhiteBlackPair serializer for TimeSpan
/// Implementing a generic serializer for containers of primitive types is tough because it can be difficult to know how large the type is.
/// It is made more difficult for TimeSpan because you have to convert it to a primitive value first (via the Ticks property)
internal class WhiteBlackPairTimespanSerializer : INodeSerializer<WhiteBlackPair<TimeSpan>>
{
	private const int SIZE = sizeof(long) * 2;
	public int? NodeSize => SIZE;

	public ulong Serialize(WhiteBlackPair<TimeSpan> obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[SIZE];
		BinaryPrimitives.WriteInt64LittleEndian(buffer.Slice(0, sizeof(long)), obj.WhiteValue.Ticks);
		BinaryPrimitives.WriteInt64LittleEndian(buffer.Slice(sizeof(long), sizeof(long)), obj.BlackValue.Ticks);
		return dataSink.AddNode(buffer);
	}

	public WhiteBlackPair<TimeSpan> Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var whiteTicks = BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(0, sizeof(long)));
		var blackTicks = BinaryPrimitives.ReadInt64LittleEndian(bytes.Slice(sizeof(long), sizeof(long)));
		return new WhiteBlackPair<TimeSpan>(TimeSpan.FromTicks(whiteTicks), TimeSpan.FromTicks(blackTicks));
	}
}

/// A generic implementation of a WhiteBlackPair serializer for when the WhiteBlackPair is a branch.
/// In this case, we know for certain the size of the contents of the node, since the node simply contains hashes, which are always 8 bytes,
/// even though we don't know anything about the size of the contained nodes themselves.
/// The contained nodes could be either blobs or branches; it doesn't matter to us.
internal class WhiteBlackPairBranchSerializer<TNode> : INodeSerializer<WhiteBlackPair<TNode>>
{
	private readonly INodeSerializer<TNode> _memberSerializer;

	public int? NodeSize { get; }

	public WhiteBlackPairBranchSerializer(INodeSerializer<TNode> memberSerializer)
	{
		_memberSerializer = memberSerializer;
		NodeSize = _memberSerializer.NodeSize * 2;
	}

	public ulong Serialize(WhiteBlackPair<TNode> obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong) * 2];

		var whiteHash = _memberSerializer.Serialize(obj.WhiteValue, dataSink);
		var blackHash = _memberSerializer.Serialize(obj.BlackValue, dataSink);

		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(0, sizeof(ulong)), whiteHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(sizeof(ulong), sizeof(ulong)), blackHash);

		return dataSink.AddNode(buffer);
	}

	public WhiteBlackPair<TNode> Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var whiteHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(0, sizeof(ulong)));
		var blackHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong), sizeof(ulong)));

		var whiteValue = dataSource.GetNode(whiteHash, _memberSerializer);
		var blackValue = dataSource.GetNode(blackHash, _memberSerializer);

		return new WhiteBlackPair<TNode>(whiteValue, blackValue);
	}
}
