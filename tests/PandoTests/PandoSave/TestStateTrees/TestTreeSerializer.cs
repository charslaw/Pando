using System;
using System.Text;
using Pando;
using Pando.Repositories;

namespace PandoTests.PandoSave.TestStateTrees
{
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

		private const int HASH_SIZE = sizeof(ulong);

		private const int NAME_HASH_OFFSET = 0;
		private const int NAME_HASH_END_OFFSET = NAME_HASH_OFFSET + HASH_SIZE;

		private const int MYA_HASH_OFFSET = NAME_HASH_END_OFFSET;
		private const int MYA_HASH_END_OFFSET = MYA_HASH_OFFSET + HASH_SIZE;

		private const int MYB_HASH_OFFSET = MYA_HASH_END_OFFSET;
		private const int MYB_HASH_END_OFFSET = MYB_HASH_OFFSET + HASH_SIZE;

		private const int SIZE = HASH_SIZE * 3;

		public ulong Serialize(TestTree obj, IWritablePandoNodeRepository repository)
		{
			var nameHash = _nameSerializer.Serialize(obj.Name, repository);
			var myAHash = _aSerializer.Serialize(obj.MyA, repository);
			var myBHash = _bSerializer.Serialize(obj.MyB, repository);

			Span<byte> buffer = stackalloc byte[SIZE];
			PandoUtils.BitConverter.CopyBytes(nameHash, buffer[NAME_HASH_OFFSET..NAME_HASH_END_OFFSET]);
			PandoUtils.BitConverter.CopyBytes(myAHash, buffer[MYA_HASH_OFFSET..MYA_HASH_END_OFFSET]);
			PandoUtils.BitConverter.CopyBytes(myBHash, buffer[MYB_HASH_OFFSET..MYB_HASH_END_OFFSET]);

			return repository.AddNode(buffer);
		}

		public TestTree Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
		{
			var nameHash = PandoUtils.BitConverter.ToUInt64(bytes[NAME_HASH_OFFSET..NAME_HASH_END_OFFSET]);
			var myAHash = PandoUtils.BitConverter.ToUInt64(bytes[MYA_HASH_OFFSET..MYA_HASH_END_OFFSET]);
			var myBHash = PandoUtils.BitConverter.ToUInt64(bytes[MYB_HASH_OFFSET..MYB_HASH_END_OFFSET]);

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
			PandoUtils.BitConverter.CopyBytes(obj.Age, buffer);
			return repository.AddNode(buffer);
		}

		public TestTree.A Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
		{
			var age = PandoUtils.BitConverter.ToInt32(bytes);
			return new TestTree.A(age);
		}
	}

	internal class DoubleTreeBSerializer : IPandoNodeSerializer<TestTree.B>
	{
		private const int TIME_SIZE = sizeof(long);
		private const int TIME_OFFSET = 0;
		private const int TIME_END_OFFSET = TIME_OFFSET + TIME_SIZE;

		private const int CENTS_SIZE = sizeof(int);
		private const int CENTS_OFFSET = TIME_END_OFFSET;
		private const int CENTS_END_OFFSET = CENTS_OFFSET + CENTS_SIZE;

		private const int SIZE = TIME_SIZE + CENTS_SIZE;

		public ulong Serialize(TestTree.B obj, IWritablePandoNodeRepository repository)
		{
			Span<byte> myBuffer = stackalloc byte[SIZE];
			var timeBinary = obj.Time.ToBinary();
			PandoUtils.BitConverter.CopyBytes(timeBinary, myBuffer[..TIME_END_OFFSET]);
			PandoUtils.BitConverter.CopyBytes(obj.Cents, myBuffer[CENTS_OFFSET..CENTS_END_OFFSET]);

			return repository.AddNode(myBuffer);
		}

		public TestTree.B Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository _)
		{
			var timeBinary = PandoUtils.BitConverter.ToInt64(bytes[..TIME_END_OFFSET]);
			var date = DateTime.FromBinary(timeBinary);
			var cents = PandoUtils.BitConverter.ToInt32(bytes[CENTS_OFFSET..CENTS_END_OFFSET]);

			return new TestTree.B(date, cents);
		}
	}
}
