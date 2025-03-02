using System;
using System.Buffers.Binary;
using Pando.Serialization.Utils;

namespace PandoTests.Tests.Serialization.Utils.MergeUtilsTests;

public class MergeInline
{
	[Test]
	[Arguments(100, 200, 100, 200)] // only target value changed; use target value
	[Arguments(100, 100, 300, 300)] // only source value changed; use source value
	[Arguments(100, 200, 300, 300)] // both changed, use source value
	public async Task Should_merge_values_correctly(int baseValue, int targetValue, int sourceValue, int expectedValue)
	{
		Span<byte> buffer = stackalloc byte[sizeof(int) * 3];
		var baseBuffer = buffer.Slice(0, sizeof(int));
		var targetBuffer = buffer.Slice(sizeof(int), sizeof(int));
		var sourceBuffer = buffer.Slice(sizeof(int) * 2, sizeof(int));
		BinaryPrimitives.WriteInt32LittleEndian(baseBuffer, baseValue);
		BinaryPrimitives.WriteInt32LittleEndian(targetBuffer, targetValue);
		BinaryPrimitives.WriteInt32LittleEndian(sourceBuffer, sourceValue);

		MergeUtils.MergeInline(baseBuffer, targetBuffer, sourceBuffer);

		await Assert.That(expectedValue).IsEqualTo(BinaryPrimitives.ReadInt32LittleEndian(baseBuffer));
	}
}
