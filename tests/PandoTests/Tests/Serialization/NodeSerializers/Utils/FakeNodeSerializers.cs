using System;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoTests.Tests.Serialization.NodeSerializers.Utils;

public static class FakeNodeSerializers
{
	/// Uses a pre-determined look up table to convert a given object into it's "binary representation"
	/// The association between object and "binary representation" is provided in the constructor.
	public class SerializeByLookup : INodeSerializer<object>
	{
		private readonly IEnumerable<(object obj, byte[] bytes)> _lut;

		/// <param name="entries">each entry maps a node object to the binary representation that should be returned for that object.</param>
		public SerializeByLookup(params (object obj, byte[] bytes)[] entries) { _lut = entries; }

		public int? NodeSize => null;
		public int NodeSizeForObject(object obj) => _lut.First(entry => entry.obj.Equals(obj)).bytes.Length;

		public void Serialize(object obj, Span<byte> writeBuffer, INodeDataSink dataSink)
			=> _lut.First(entry => entry.obj.Equals(obj)).bytes.CopyTo(writeBuffer);

		public object Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) => null!;
	}

	/// Uses a pre-determined look up table to "deserialize" a given node "binary representation" into the corresponding object.
	/// The association between "binary representation" and object is provided in the constructor.
	public class DeserializeByLookup : INodeSerializer<object>
	{
		private readonly IEnumerable<(byte[] bytes, object obj)> _lut;

		/// <param name="entries">each entry maps a node binary representation to the object that should be returned for that binary representation.</param>
		public DeserializeByLookup(params (byte[] bytes, object obj)[] entries) { _lut = entries; }

		public int? NodeSize => null;
		public int NodeSizeForObject(object obj) => 0;
		public void Serialize(object obj, Span<byte> writeBuffer, INodeDataSink dataSink) { }

		public object Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
		{
			var bytesArray = readBuffer.ToArray();
			return _lut.First(entry => entry.bytes.SequenceEqual(bytesArray)).obj;
		}
	}
}
