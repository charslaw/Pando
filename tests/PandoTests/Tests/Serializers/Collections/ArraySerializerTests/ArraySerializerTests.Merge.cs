using System;
using Pando.Serializers.Collections;
using Pando.Serializers.Primitives;
using Pando.Vaults;

namespace PandoTests.Tests.Serializers.Collections.ArraySerializerTests;

public static partial class ArraySerializerTests
{
	public class Merge
	{
		[Test]
		public async Task Should_merge_equal_size_arrays_elementwise()
		{
			var serializer = new ArraySerializer<int>(new Int32LittleEndianSerializer());
			var vault = new MemoryNodeVault();

			InitializeBuffers(
				[0, 0, 0, 0],
				[0, 1, 0, 1],
				[0, 0, 2, 2],
				out var baseBuffer,
				out var targetBuffer,
				out var sourceBuffer,
				serializer,
				vault
			);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, vault);

			var actual = serializer.Deserialize(baseBuffer, vault);

			int[] expected = [0, 1, 2, 2];

			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_merge_into_larger_array_when_target_is_larger()
		{
			var serializer = new ArraySerializer<int>(new Int32LittleEndianSerializer());
			var vault = new MemoryNodeVault();

			InitializeBuffers(
				[0, 0, 0, 0],
				[0, 1, 0, 1, 1],
				[0, 0, 2, 2],
				out var baseBuffer,
				out var targetBuffer,
				out var sourceBuffer,
				serializer,
				vault
			);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, vault);

			var actual = serializer.Deserialize(baseBuffer, vault);

			int[] expected = [0, 1, 2, 2, 1];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_merge_into_larger_array_when_source_is_larger()
		{
			var serializer = new ArraySerializer<int>(new Int32LittleEndianSerializer());
			var vault = new MemoryNodeVault();

			InitializeBuffers(
				[0, 0, 0, 0],
				[0, 1, 0, 1],
				[0, 0, 2, 2, 2],
				out var baseBuffer,
				out var targetBuffer,
				out var sourceBuffer,
				serializer,
				vault
			);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, vault);

			var actual = serializer.Deserialize(baseBuffer, vault);

			int[] expected = [0, 1, 2, 2, 2];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		[Test]
		public async Task Should_merge_into_smaller_array_when_base_is_larger()
		{
			var serializer = new ArraySerializer<int>(new Int32LittleEndianSerializer());
			var vault = new MemoryNodeVault();

			InitializeBuffers(
				[0, 0, 0, 0, 0],
				[0, 1, 0, 1],
				[0, 0, 2, 2],
				out var baseBuffer,
				out var targetBuffer,
				out var sourceBuffer,
				serializer,
				vault
			);

			serializer.Merge(baseBuffer, targetBuffer, sourceBuffer, vault);

			var actual = serializer.Deserialize(baseBuffer, vault);

			int[] expected = [0, 1, 2, 2];
			await Assert.That(actual).IsEquivalentTo(expected);
		}

		private static void InitializeBuffers(
			int[] baseArr,
			int[] targetArr,
			int[] sourceArr,
			out Span<byte> baseBuffer,
			out Span<byte> targetBuffer,
			out Span<byte> sourceBuffer,
			ArraySerializer<int> serializer,
			INodeVault nodeVault
		)
		{
			Span<byte> buffer = new byte[sizeof(ulong) * 3];

			baseBuffer = buffer.Slice(0, sizeof(ulong));
			targetBuffer = buffer.Slice(sizeof(ulong), sizeof(ulong));
			sourceBuffer = buffer.Slice(sizeof(ulong) * 2, sizeof(ulong));

			serializer.Serialize(baseArr, baseBuffer, nodeVault);
			serializer.Serialize(targetArr, targetBuffer, nodeVault);
			serializer.Serialize(sourceArr, sourceBuffer, nodeVault);
		}
	}
}
