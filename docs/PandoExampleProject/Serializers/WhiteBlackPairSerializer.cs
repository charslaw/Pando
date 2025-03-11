using System;
using Pando.DataSources;
using Pando.Serialization;

namespace PandoExampleProject.Serializers;

internal class WhiteBlackPairSerializer<T>(IPandoSerializer<T> tSerializer) : IPandoSerializer<WhiteBlackPair<T>>
{
	private readonly int _tSize = tSerializer.SerializedSize;
	public int SerializedSize { get; } = tSerializer.SerializedSize * 2;

	public void Serialize(WhiteBlackPair<T> value, Span<byte> buffer, INodeDataStore dataStore)
	{
		tSerializer.Serialize(value.WhiteValue, buffer[.._tSize], dataStore);
		tSerializer.Serialize(value.BlackValue, buffer[_tSize..SerializedSize], dataStore);
	}

	public WhiteBlackPair<T> Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		var whiteValue = tSerializer.Deserialize(buffer[.._tSize], dataStore);
		var blackValue = tSerializer.Deserialize(buffer[_tSize..SerializedSize], dataStore);

		return new WhiteBlackPair<T>(whiteValue, blackValue);
	}
}
