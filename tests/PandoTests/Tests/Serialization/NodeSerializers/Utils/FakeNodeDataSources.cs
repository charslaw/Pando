using System;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources;

namespace PandoTests.Tests.Serialization.NodeSerializers.Utils;

public static class FakeNodeDataSources
{
	/// Used to verify that AddNode will get called the correct number of times with the correct values
	public class AddNodeSpy : INodeDataSink
	{
		public int CallCount { get; private set; }
		public List<byte[]> CallArgs { get; } = new();

		public ulong AddNode(ReadOnlySpan<byte> bytes)
		{
			CallArgs.Add(bytes.ToArray());
			CallCount++;

			return default;
		}
	}

	/// Returns a pre-determined hash based on the given "binary representation".
	/// The association between "binary representation" and hash is provided in the constructor.
	public class AddNodeByLookup : INodeDataSink
	{
		private readonly IEnumerable<(byte[] bytes, ulong hash)> _lut;

		/// <param name="entries">each entry maps a node binary representation to the hash that should be returned for that representation.</param>
		public AddNodeByLookup(params (byte[] bytes, ulong hash)[] entries) { _lut = entries; }

		public ulong AddNode(ReadOnlySpan<byte> bytes)
		{
			var bytesArray = bytes.ToArray();
			return _lut.First(entry => entry.bytes.SequenceEqual(bytesArray)).hash;
		}
	}

	/// Updates the given output buffer with a pre-determined "binary representation" based on the given hash.
	/// The association between hash and "binary representation" is provided in the constructor.
	public class CopyNodeBytesByLookup : INodeDataSource
	{
		private readonly IEnumerable<(ulong hash, byte[] bytes)> _lut;

		/// <param name="entries">each entry maps a hash to the node binary representation that should be returned for that hash.</param>
		public CopyNodeBytesByLookup(params (ulong hash, byte[] bytes)[] entries) { _lut = entries; }

		public bool HasNode(ulong hash) => _lut.Any(entry => entry.hash == hash);
		public int GetSizeOfNode(ulong hash) => _lut.First(entry => entry.hash == hash).bytes.Length;
		public void CopyNodeBytesTo(ulong hash, ref Span<byte> outputBytes) => _lut.First(entry => entry.hash == hash).bytes.CopyTo(outputBytes);
	}
}
