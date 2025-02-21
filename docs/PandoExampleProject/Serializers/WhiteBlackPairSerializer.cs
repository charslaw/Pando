using System;
using System.Buffers.Binary;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

internal class WhiteBlackPairSerializer<T>(IPandoSerializer<T> tSerializer) : IPandoSerializer<WhiteBlackPair<T>>
{
	public int SerializedSize => sizeof(ulong);

	public void Serialize(WhiteBlackPair<T> value, Span<byte> buffer, INodeDataStore dataStore)
	{
		var tSize = tSerializer.SerializedSize;
		var totalSize = 2 * tSize;
		Span<byte> childrenBuffer = stackalloc byte[totalSize];

		tSerializer.Serialize(value.WhiteValue, childrenBuffer[..tSize], dataStore);
		tSerializer.Serialize(value.BlackValue, childrenBuffer[tSize..totalSize], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public WhiteBlackPair<T> Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		var tSize = tSerializer.SerializedSize;
		var totalSize = tSize * 2;
		Span<byte> childrenBuffer = stackalloc byte[totalSize];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var whiteValue = tSerializer.Deserialize(childrenBuffer[..tSize], dataStore);
		var blackValue = tSerializer.Deserialize(childrenBuffer[tSize..totalSize], dataStore);

		return new WhiteBlackPair<T>(whiteValue, blackValue);
	}
}
