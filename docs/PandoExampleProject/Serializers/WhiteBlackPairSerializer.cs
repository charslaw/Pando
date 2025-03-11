using System;
using Pando.Serialization;
using Pando.Vaults;

namespace PandoExampleProject.Serializers;

internal class WhiteBlackPairSerializer<T>(IPandoSerializer<T> tSerializer) : IPandoSerializer<WhiteBlackPair<T>>
{
	private readonly int _tSize = tSerializer.SerializedSize;
	public int SerializedSize { get; } = tSerializer.SerializedSize * 2;

	public void Serialize(WhiteBlackPair<T> value, Span<byte> buffer, INodeVault nodeVault)
	{
		tSerializer.Serialize(value.WhiteValue, buffer[.._tSize], nodeVault);
		tSerializer.Serialize(value.BlackValue, buffer[_tSize..SerializedSize], nodeVault);
	}

	public WhiteBlackPair<T> Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		var whiteValue = tSerializer.Deserialize(buffer[.._tSize], nodeVault);
		var blackValue = tSerializer.Deserialize(buffer[_tSize..SerializedSize], nodeVault);

		return new WhiteBlackPair<T>(whiteValue, blackValue);
	}
}
