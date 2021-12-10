using System;
using System.Text;
using Pando;
using Pando.Repositories;
using Pando.Repositories.Utils;

namespace PandoTests.Tests.PandoSave.TestStateTrees;

internal readonly struct TestTreeSerializer : IPandoNodeSerializerDeserializer<TestTree>
{
	private readonly IPandoNodeSerializerDeserializer<string> _nameSerializer;
	private readonly IPandoNodeSerializerDeserializer<TestTree.A> _aSerializer;
	private readonly IPandoNodeSerializerDeserializer<TestTree.B> _bSerializer;

	public TestTreeSerializer(
		IPandoNodeSerializerDeserializer<string> nameSerializer,
		IPandoNodeSerializerDeserializer<TestTree.A> aSerializer,
		IPandoNodeSerializerDeserializer<TestTree.B> bSerializer
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

	public ulong Serialize(TestTree obj, IWritablePandoNodeRepository repository)
	{
		var nameHash = _nameSerializer.Serialize(obj.Name, repository);
		var myAHash = _aSerializer.Serialize(obj.MyA, repository);
		var myBHash = _bSerializer.Serialize(obj.MyB, repository);

		Span<byte> buffer = stackalloc byte[SIZE];
		ByteConverter.CopyBytes(nameHash, buffer[..NAME_HASH_END]);
		ByteConverter.CopyBytes(myAHash, buffer[NAME_HASH_END..MYA_HASH_END]);
		ByteConverter.CopyBytes(myBHash, buffer[MYA_HASH_END..MYB_HASH_END]);
		return repository.AddNode(buffer);
	}

	public TestTree Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
	{
		var nameHash = ByteConverter.GetUInt64(bytes[..NAME_HASH_END]);
		var myAHash = ByteConverter.GetUInt64(bytes[NAME_HASH_END..MYA_HASH_END]);
		var myBHash = ByteConverter.GetUInt64(bytes[MYA_HASH_END..MYB_HASH_END]);

		var name = repository.GetNode(nameHash, _nameSerializer);
		var myA = repository.GetNode(myAHash, _aSerializer);
		var myB = repository.GetNode(myBHash, _bSerializer);

		return new TestTree(name, myA, myB);
	}
}

internal readonly struct StringSerializer : IPandoNodeSerializerDeserializer<string>
{
	public ulong Serialize(string str, IWritablePandoNodeRepository repository)
	{
		var nameByteCount = Encoding.UTF8.GetByteCount(str);
		Span<byte> buffer = stackalloc byte[nameByteCount];
		Encoding.UTF8.GetBytes(str, buffer);
		return repository.AddNode(buffer);
	}

	public string Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
	{
		return Encoding.UTF8.GetString(bytes);
	}
}

internal readonly struct DoubleTreeASerializer : IPandoNodeSerializerDeserializer<TestTree.A>
{
	private const int AGE_SIZE = sizeof(int);

	public ulong Serialize(TestTree.A obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> buffer = stackalloc byte[AGE_SIZE];
		ByteConverter.CopyBytes(obj.Age, buffer);
		return repository.AddNode(buffer);
	}

	public TestTree.A Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
	{
		var age = ByteConverter.GetInt32(bytes);
		return new TestTree.A(age);
	}
}

internal readonly struct DoubleTreeBSerializer : IPandoNodeSerializerDeserializer<TestTree.B>
{
	private const int TIME_END = sizeof(long);
	private const int CENTS_END = TIME_END + sizeof(int);
	private const int SIZE = CENTS_END;

	public ulong Serialize(TestTree.B obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> myBuffer = stackalloc byte[SIZE];
		var timeBinary = obj.Time.ToBinary();

		ByteConverter.CopyBytes(timeBinary, myBuffer[..TIME_END]);
		ByteConverter.CopyBytes(obj.Cents, myBuffer[TIME_END..CENTS_END]);

		return repository.AddNode(myBuffer);
	}

	public TestTree.B Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
	{
		var timeBinary = ByteConverter.GetInt64(bytes[..TIME_END]);
		var date = DateTime.FromBinary(timeBinary);
		var cents = ByteConverter.GetInt32(bytes[TIME_END..CENTS_END]);

		return new TestTree.B(date, cents);
	}
}
