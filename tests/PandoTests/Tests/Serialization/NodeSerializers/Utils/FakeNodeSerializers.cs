using System;
using System.Collections.Generic;
using System.Linq;
using Pando.DataSources;
using Pando.Serialization.NodeSerializers;

namespace PandoTests.Tests.Serialization.NodeSerializers.Utils;

public static class FakeNodeSerializers
{
	/// Serializes incoming objects by looking them up in a look up table
	public class SerializeByLookup : INodeSerializer<object>
	{
		private readonly IEnumerable<(object obj, byte[] bytes)> _lut;
		public SerializeByLookup(params (object obj, byte[] bytes)[] entries) { _lut = entries; }

		public int? NodeSize => null;
		public int NodeSizeForObject(object obj) => _lut.First(entry => entry.obj.Equals(obj)).bytes.Length;

		public void Serialize(object obj, Span<byte> writeBuffer, INodeDataSink dataSink)
			=> _lut.First(entry => entry.obj.Equals(obj)).bytes.CopyTo(writeBuffer);

		public object Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource) => null!;
	}

	/// Deserializes incoming bytes by looking them up in a look up table
	public class DeserializeByLookup : INodeSerializer<object>
	{
		private readonly IEnumerable<(byte[] bytes, object obj)> _lut;
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
