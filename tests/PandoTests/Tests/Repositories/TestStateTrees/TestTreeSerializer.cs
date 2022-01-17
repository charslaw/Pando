using System;
using System.Text;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization;
using Pando.Serialization.PrimitiveSerializers;

namespace PandoTests.Tests.Repositories.TestStateTrees;

internal readonly struct TestTreeSerializer : INodeSerializer<TestTree>
{
	private readonly IPrimitiveSerializer<string> _nameSerializer = new StringSerializer(Encoding.UTF8);
	private readonly INodeSerializer<TestTree.A> _aSerializer = new DoubleTreeASerializer();
	private readonly INodeSerializer<TestTree.B> _bSerializer = new DoubleTreeBSerializer();

	public int? NodeSize => null;
	public int NodeSizeForObject(TestTree obj) => 2 * sizeof(ulong) + _nameSerializer.ByteCountForValue(obj.Name);

	public void Serialize(TestTree obj, Span<byte> writeBuffer, INodeDataSink dataSink)
	{
		var myAHash = _aSerializer.SerializeToHash(obj.MyA, dataSink);
		var myBHash = _bSerializer.SerializeToHash(obj.MyB, dataSink);

		_nameSerializer.Serialize(obj.Name, ref writeBuffer);
		ByteEncoder.CopyBytes(myAHash, writeBuffer.Slice(0, sizeof(ulong)));
		ByteEncoder.CopyBytes(myBHash, writeBuffer.Slice(sizeof(ulong), sizeof(ulong)));
	}

	public TestTree Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource dataSource)
	{
		var name = _nameSerializer.Deserialize(ref readBuffer);
		var myAHash = ByteEncoder.GetUInt64(readBuffer.Slice(0, sizeof(ulong)));
		var myBHash = ByteEncoder.GetUInt64(readBuffer.Slice(sizeof(ulong), sizeof(ulong)));

		var myA = _aSerializer.DeserializeFromHash(myAHash, dataSource);
		var myB = _bSerializer.DeserializeFromHash(myBHash, dataSource);

		return new TestTree(name, myA, myB);
	}
}

internal readonly struct DoubleTreeASerializer : INodeSerializer<TestTree.A>
{
	private readonly int _size;

	public DoubleTreeASerializer()
	{
		_size = Int32LittleEndianSerializer.Default.ByteCount!.Value;
	}

	public int? NodeSize => _size;
	public int NodeSizeForObject(TestTree.A obj) => _size;

	public void Serialize(TestTree.A obj, Span<byte> buffer, INodeDataSink _)
	{
		Int32LittleEndianSerializer.Default.Serialize(obj.Age, ref buffer);
	}

	public TestTree.A Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource _)
	{
		var age = Int32LittleEndianSerializer.Default.Deserialize(ref readBuffer);
		return new TestTree.A(age);
	}
}

internal readonly struct DoubleTreeBSerializer : INodeSerializer<TestTree.B>
{
	public DoubleTreeBSerializer()
	{
		NodeSize = DateTimeToBinarySerializer.Default.ByteCount + Int32LittleEndianSerializer.Default.ByteCount;
	}

	public int? NodeSize { get; }

	public int NodeSizeForObject(TestTree.B obj)
		=> NodeSize ?? DateTimeToBinarySerializer.Default.ByteCountForValue(obj.Time) + Int32LittleEndianSerializer.Default.ByteCount!.Value;

	public void Serialize(TestTree.B obj, Span<byte> writeBuffer, INodeDataSink _)
	{
		DateTimeToBinarySerializer.Default.Serialize(obj.Time, ref writeBuffer);
		Int32LittleEndianSerializer.Default.Serialize(obj.Cents, ref writeBuffer);
	}

	public TestTree.B Deserialize(ReadOnlySpan<byte> readBuffer, INodeDataSource _)
	{
		var date = DateTimeToBinarySerializer.Default.Deserialize(ref readBuffer);
		var cents = Int32LittleEndianSerializer.Default.Deserialize(ref readBuffer);

		return new TestTree.B(date, cents);
	}
}
