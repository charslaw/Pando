using System;
using System.Buffers.Binary;
using System.Text;
using Pando;
using Pando.Repositories;

namespace PandoTests.PandoSave.TestStateTrees;

internal class TestTreeSerializer : IPandoNodeSerializer<TestTree>
{
	private readonly IPandoNodeSerializer<string> _nameSerializer;
	private readonly IPandoNodeSerializer<TestTree.A> _aSerializer;
	private readonly IPandoNodeSerializer<TestTree.B> _bSerializer;

	public TestTreeSerializer(IPandoNodeSerializer<string> nameSerializer, IPandoNodeSerializer<TestTree.A> aSerializer,
		IPandoNodeSerializer<TestTree.B> bSerializer)
	{
		_nameSerializer = nameSerializer;
		_aSerializer = aSerializer;
		_bSerializer = bSerializer;
	}

	/// Creates a new TestTreeSerializer with the default configuration injected.
	public static TestTreeSerializer Create() => new(new StringSerializer(), new DoubleTreeASerializer(), new DoubleTreeBSerializer());

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
		BinaryPrimitives.WriteUInt64LittleEndian(buffer[..NAME_HASH_END], nameHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer[NAME_HASH_END..MYA_HASH_END], myAHash);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer[MYA_HASH_END..MYB_HASH_END], myBHash);
		return repository.AddNode(buffer);
	}

	public TestTree Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
	{
		var nameHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..NAME_HASH_END]);
		var myAHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes[NAME_HASH_END..MYA_HASH_END]);
		var myBHash = BinaryPrimitives.ReadUInt64LittleEndian(bytes[MYA_HASH_END..MYB_HASH_END]);

		var name = repository.GetNode(nameHash, nameBytes => _nameSerializer.Deserialize(nameBytes, repository));
		var myA = repository.GetNode(myAHash, aBytes => _aSerializer.Deserialize(aBytes, repository));
		var myB = repository.GetNode(myBHash, bBytes => _bSerializer.Deserialize(bBytes, repository));

		return new TestTree(name, myA, myB);
	}
}

internal class StringSerializer : IPandoNodeSerializer<string>
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

internal class DoubleTreeASerializer : IPandoNodeSerializer<TestTree.A>
{
	private const int AGE_SIZE = sizeof(int);

	public ulong Serialize(TestTree.A obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> buffer = stackalloc byte[AGE_SIZE];
		BinaryPrimitives.WriteInt32LittleEndian(buffer, obj.Age);
		return repository.AddNode(buffer);
	}

	public TestTree.A Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
	{
		var age = BinaryPrimitives.ReadInt32LittleEndian(bytes);
		return new TestTree.A(age);
	}
}

internal class DoubleTreeBSerializer : IPandoNodeSerializer<TestTree.B>
{
	private const int TIME_END = sizeof(long);
	private const int CENTS_END = TIME_END + sizeof(int);
	private const int SIZE = CENTS_END;

	public ulong Serialize(TestTree.B obj, IWritablePandoNodeRepository repository)
	{
		Span<byte> myBuffer = stackalloc byte[SIZE];
		var timeBinary = obj.Time.ToBinary();

		BinaryPrimitives.WriteInt64LittleEndian(myBuffer[..TIME_END], timeBinary);
		BinaryPrimitives.WriteInt32LittleEndian(myBuffer[TIME_END..CENTS_END], obj.Cents);

		return repository.AddNode(myBuffer);
	}

	public TestTree.B Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
	{
		var timeBinary = BinaryPrimitives.ReadInt64LittleEndian(bytes[..TIME_END]);
		var date = DateTime.FromBinary(timeBinary);
		var cents = BinaryPrimitives.ReadInt32LittleEndian(bytes[TIME_END..CENTS_END]);

		return new TestTree.B(date, cents);
	}
}
