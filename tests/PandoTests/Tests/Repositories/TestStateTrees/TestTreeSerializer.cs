using System;
using System.Text;
using Pando.DataSources;
using Pando.DataSources.Utils;
using Pando.Serialization;

namespace PandoTests.Tests.Repositories.TestStateTrees;

internal readonly struct TestTreeSerializer : INodeSerializer<TestTree>
{
	private readonly INodeSerializer<string> _nameSerializer;
	private readonly INodeSerializer<TestTree.A> _aSerializer;
	private readonly INodeSerializer<TestTree.B> _bSerializer;

	public TestTreeSerializer(
		INodeSerializer<string> nameSerializer,
		INodeSerializer<TestTree.A> aSerializer,
		INodeSerializer<TestTree.B> bSerializer
	)
	{
		_nameSerializer = nameSerializer;
		_aSerializer = aSerializer;
		_bSerializer = bSerializer;
	}

	/// Creates a new TestTreeSerializerDeserializer with the default configuration injected.
	/// This is only used for convenience while testing.
	public static TestTreeSerializer Create() => new(
		new StringSerializer(),
		new DoubleTreeASerializer(),
		new DoubleTreeBSerializer()
	);

	private const int NAME_HASH_END = sizeof(ulong);
	private const int MYA_HASH_END = NAME_HASH_END + sizeof(ulong);
	private const int MYB_HASH_END = MYA_HASH_END + sizeof(ulong);

	private const int SIZE = MYB_HASH_END;

	public ulong Serialize(TestTree obj, INodeDataSink dataSink)
	{
		var nameHash = _nameSerializer.Serialize(obj.Name, dataSink);
		var myAHash = _aSerializer.Serialize(obj.MyA, dataSink);
		var myBHash = _bSerializer.Serialize(obj.MyB, dataSink);

		Span<byte> buffer = stackalloc byte[SIZE];
		ByteEncoder.CopyBytes(nameHash, buffer[..NAME_HASH_END]);
		ByteEncoder.CopyBytes(myAHash, buffer[NAME_HASH_END..MYA_HASH_END]);
		ByteEncoder.CopyBytes(myBHash, buffer[MYA_HASH_END..MYB_HASH_END]);
		return dataSink.AddNode(buffer);
	}

	public TestTree Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource)
	{
		var nameHash = ByteEncoder.GetUInt64(bytes[..NAME_HASH_END]);
		var myAHash = ByteEncoder.GetUInt64(bytes[NAME_HASH_END..MYA_HASH_END]);
		var myBHash = ByteEncoder.GetUInt64(bytes[MYA_HASH_END..MYB_HASH_END]);

		var name = dataSource.GetNode(nameHash, _nameSerializer);
		var myA = dataSource.GetNode(myAHash, _aSerializer);
		var myB = dataSource.GetNode(myBHash, _bSerializer);

		return new TestTree(name, myA, myB);
	}
}

internal readonly struct StringSerializer : INodeSerializer<string>
{
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
	private const int AGE_SIZE = sizeof(int);

	public ulong Serialize(TestTree.A obj, INodeDataSink dataSink)
	{
		Span<byte> buffer = stackalloc byte[AGE_SIZE];
		ByteEncoder.CopyBytes(obj.Age, buffer);
		return dataSink.AddNode(buffer);
	}

	public TestTree.A Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource _)
	{
		var age = ByteEncoder.GetInt32(bytes);
		return new TestTree.A(age);
	}
}

internal readonly struct DoubleTreeBSerializer : INodeSerializer<TestTree.B>
{
	private const int TIME_END = sizeof(long);
	private const int CENTS_END = TIME_END + sizeof(int);
	private const int SIZE = CENTS_END;

	public ulong Serialize(TestTree.B obj, INodeDataSink dataSink)
	{
		Span<byte> myBuffer = stackalloc byte[SIZE];
		var timeBinary = obj.Time.ToBinary();

		ByteEncoder.CopyBytes(timeBinary, myBuffer[..TIME_END]);
		ByteEncoder.CopyBytes(obj.Cents, myBuffer[TIME_END..CENTS_END]);

		return dataSink.AddNode(myBuffer);
	}

	public TestTree.B Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource _)
	{
		var timeBinary = ByteEncoder.GetInt64(bytes[..TIME_END]);
		var date = DateTime.FromBinary(timeBinary);
		var cents = ByteEncoder.GetInt32(bytes[TIME_END..CENTS_END]);

		return new TestTree.B(date, cents);
	}
}
