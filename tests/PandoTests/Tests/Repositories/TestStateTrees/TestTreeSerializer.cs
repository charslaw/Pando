using System;
using System.Text;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization;
using Pando.Serialization.PrimitiveSerializers;
using Pando.Serialization.Utils;

namespace PandoTests.Tests.Repositories.TestStateTrees;

internal readonly struct TestTreeSerializer : INodeSerializer<TestTree>
{
	private readonly INodeSerializer<string> _nameSerializer;
	private readonly INodeSerializer<TestTree.A> _aSerializer;
	private readonly INodeSerializer<TestTree.B> _bSerializer;

	public int? NodeSize { get; }

	public TestTreeSerializer(
		INodeSerializer<string> nameSerializer,
		INodeSerializer<TestTree.A> aSerializer,
		INodeSerializer<TestTree.B> bSerializer
	)
	{
		_nameSerializer = nameSerializer;
		_aSerializer = aSerializer;
		_bSerializer = bSerializer;
		NodeSize = _nameSerializer.NodeSize + _aSerializer.NodeSize + _bSerializer.NodeSize;
	}

	/// Creates a new TestTreeSerializerDeserializer with the default configuration injected.
	/// This is only used for convenience while testing.
	public static TestTreeSerializer Create() => new(
		new StringSerializer(),
		new DoubleTreeASerializer(),
		new DoubleTreeBSerializer()
	);

	private const int SIZE_OF_HASH = sizeof(ulong);
	private const int SIZE = SIZE_OF_HASH * 3;

	public ulong Serialize(TestTree obj, INodeDataSink dataSink)
	{
		var nameHash = _nameSerializer.Serialize(obj.Name, dataSink);
		var myAHash = _aSerializer.Serialize(obj.MyA, dataSink);
		var myBHash = _bSerializer.Serialize(obj.MyB, dataSink);

		Span<byte> nodeBytes = stackalloc byte[SIZE];
		var writeBuffer = nodeBytes;
		ByteEncoder.CopyBytes(nameHash, SpanHelpers.PopStart(ref writeBuffer, SIZE_OF_HASH));
		ByteEncoder.CopyBytes(myAHash, SpanHelpers.PopStart(ref writeBuffer, SIZE_OF_HASH));
		ByteEncoder.CopyBytes(myBHash, SpanHelpers.PopStart(ref writeBuffer, SIZE_OF_HASH));
		return dataSink.AddNode(nodeBytes);
	}

	public TestTree Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var nameHash = ByteEncoder.GetUInt64(SpanHelpers.PopStart(ref bytes, SIZE_OF_HASH));
		var myAHash = ByteEncoder.GetUInt64(SpanHelpers.PopStart(ref bytes, SIZE_OF_HASH));
		var myBHash = ByteEncoder.GetUInt64(SpanHelpers.PopStart(ref bytes, SIZE_OF_HASH));

		var name = _nameSerializer.DeserializeFromHash(nameHash, dataSource);
		var myA = _aSerializer.DeserializeFromHash(myAHash, dataSource);
		var myB = _bSerializer.DeserializeFromHash(myBHash, dataSource);

		return new TestTree(name, myA, myB);
	}
}

internal readonly struct StringSerializer : INodeSerializer<string>
{
	public int? NodeSize => null;

	public ulong Serialize(string str, INodeDataSink dataSink)
	{
		var nameByteCount = Encoding.UTF8.GetByteCount(str);
		Span<byte> buffer = stackalloc byte[nameByteCount];
		Encoding.UTF8.GetBytes(str, buffer);
		return dataSink.AddNode(buffer);
	}

	public string Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource _)
	{
		return Encoding.UTF8.GetString(bytes);
	}
}

internal readonly struct DoubleTreeASerializer : INodeSerializer<TestTree.A>
{
	public int? NodeSize { get; }

	public DoubleTreeASerializer()
	{
		NodeSize = Int32LittleEndianSerializer.Default.ByteCount;
	}

	public ulong Serialize(TestTree.A obj, INodeDataSink dataSink)
	{
		Span<byte>
			nodeBytes = stackalloc byte[NodeSize!.Value]; // Since we're using a concrete primitive serializer we can know that NodeSize is safe
		var writeBuffer = nodeBytes;
		Int32LittleEndianSerializer.Default.Serialize(obj.Age, ref writeBuffer);
		return dataSink.AddNode(nodeBytes);
	}

	public TestTree.A Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource _)
	{
		var age = Int32LittleEndianSerializer.Default.Deserialize(ref bytes);
		return new TestTree.A(age);
	}
}

internal readonly struct DoubleTreeBSerializer : INodeSerializer<TestTree.B>
{
	public int? NodeSize { get; }

	public DoubleTreeBSerializer()
	{
		NodeSize = DateTimeToBinarySerializer.Default.ByteCount + Int32LittleEndianSerializer.Default.ByteCount;
	}

	public ulong Serialize(TestTree.B obj, INodeDataSink dataSink)
	{
		var timeSerializer = DateTimeToBinarySerializer.Default;
		var timeSize = timeSerializer.ByteCount!.Value;
		var centsSerializer = Int32LittleEndianSerializer.Default;
		var centsSize = centsSerializer.ByteCount!.Value;

		Span<byte> buffer = stackalloc byte[timeSize + centsSize];
		var writeBuffer = buffer;

		timeSerializer.Serialize(obj.Time, ref writeBuffer);
		centsSerializer.Serialize(obj.Cents, ref writeBuffer);

		return dataSink.AddNode(buffer);
	}

	public TestTree.B Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource _)
	{
		var timeSerializer = DateTimeToBinarySerializer.Default;
		var centsSerializer = Int32LittleEndianSerializer.Default;

		var date = timeSerializer.Deserialize(ref bytes);
		var cents = centsSerializer.Deserialize(ref bytes);

		return new TestTree.B(date, cents);
	}
}
