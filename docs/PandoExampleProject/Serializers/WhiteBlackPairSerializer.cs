using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

internal class WhiteBlackPairSerializer<T>(IPandoSerializer<T> tSerializer) : IPandoSerializer<WhiteBlackPair<T>>
{
	public int SerializedSize => sizeof(ulong);

	public void Serialize(WhiteBlackPair<T> value, Span<byte> buffer, INodeDataSink dataSink)
	{
		var tSize = tSerializer.SerializedSize;
		var totalSize = 2 * tSize;
		Span<byte> childrenBuffer = stackalloc byte[totalSize];

		tSerializer.Serialize(value.WhiteValue, childrenBuffer[..tSize], dataSink);
		tSerializer.Serialize(value.BlackValue, childrenBuffer[tSize..totalSize], dataSink);

		var nodeHash = dataSink.AddNode(childrenBuffer);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, nodeHash);
	}

	public WhiteBlackPair<T> Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var nodeHash = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		var tSize = tSerializer.SerializedSize;
		var totalSize = tSize * 2;
		Span<byte> childrenBuffer = stackalloc byte[totalSize];
		dataSource.CopyNodeBytesTo(nodeHash, childrenBuffer);

		var whiteValue = tSerializer.Deserialize(childrenBuffer[..tSize], dataSource);
		var blackValue = tSerializer.Deserialize(childrenBuffer[tSize..totalSize], dataSource);

		return new WhiteBlackPair<T>(whiteValue, blackValue);
	}
}
