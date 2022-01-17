using Pando.Serialization.PrimitiveSerializers;
using Xunit;

// Rider doesn't detect subclasses of BaseSerializerTest as being used, because they inherit their test methods
// ReSharper disable UnusedType.Global

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

public partial class PrimitiveSerializerTests
{
	public class NullableInt64SerializerTests : BasePrimitiveSerializerTest<long?>, ISerializerTestData<long?>
	{
		protected override IPrimitiveSerializer<long?> Serializer => new NullableSerializer<long>(new SimpleLongSerializer());

		public static TheoryData<long?, byte[]> SerializationTestData => new()
		{
			{ null, new byte[] { 0 } },
			{ long.MaxValue, new byte[] { 0x01, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } },
		};

		public static TheoryData<int?> ByteCountTestData => new() { null };
	}
}
