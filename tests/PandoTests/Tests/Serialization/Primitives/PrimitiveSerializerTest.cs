using System;
using System.Collections.Generic;
using Pando.Serialization;

namespace PandoTests.Tests.Serialization.Primitives;

/// A base class declaring a number of tests that all primitive serializers should pass
public abstract class PrimitiveSerializerTest<T>
{
	/// Used to overallocate serialization/deserialization buffers so that we can verify that the serializers chop off the appropriate number of bytes.
	protected const int EXTRA_BUFFER_SPACE = 4;

	public abstract IEnumerable<Func<(T, byte[])>> SerializationTestData();

	public abstract IPandoSerializer<T> CreateSerializer();

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Serialize"/> writes the correct bytes to the given buffer.
	/// </summary>
	[Test]
	[MethodDataSource(nameof(SerializationTestData))]
	public virtual async Task Serialize_should_produce_correct_bytes(T inputValue, byte[] expectedBytes)
	{
		Span<byte> nodeBytes = stackalloc byte[expectedBytes.Length];

		CreateSerializer().Serialize(inputValue, nodeBytes, null!);

		var serializationResult = nodeBytes[..expectedBytes.Length].ToArray();
		await Assert.That(serializationResult).IsEquivalentTo(expectedBytes);
	}

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Serialize"/> will throw an
	/// <see cref="ArgumentOutOfRangeException"/> if the given buffer is smaller than required for the given value.<br />
	/// </summary>
	[Test]
	[MethodDataSource(nameof(SerializationTestData))]
	public virtual async Task Serialize_should_throw_when_buffer_is_too_small(T inputValue, byte[] expectedBytes)
	{
		await Assert
			.That(() =>
				{
					Span<byte> undersizedBuffer = new byte[expectedBytes.Length - 1];
					CreateSerializer().Serialize(inputValue, undersizedBuffer, null!);
				}
			)
			.ThrowsExactly<ArgumentOutOfRangeException>();
	}

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Deserialize"/> reads the correct value from the given buffer.
	/// </summary>
	[Test]
	[MethodDataSource(nameof(SerializationTestData))]
	public virtual async Task Deserialize_should_produce_correct_value(T expectedValue, byte[] inputBytes)
	{
		// Over-allocate read buffer to ensure that the serializer doesn't get greedy with reading from the read buffer
		// If the serializer reads into the extra buffer space, presumably it will not produce the correct result.
		Span<byte> nodeBytes = new byte[inputBytes.Length + EXTRA_BUFFER_SPACE];
		inputBytes.CopyTo(nodeBytes);

		var deserializationResult = CreateSerializer().Deserialize(nodeBytes, null!);

		await Assert.That(deserializationResult).IsEqualTo(expectedValue);
	}

#pragma warning disable xUnit1026 // ignore unused parameter warning; we want to use the same data for multiple tests

	/// <summary>
	/// Verifies that <see cref="IPandoSerializer{T}.Deserialize"/> will throw an
	/// <see cref="ArgumentOutOfRangeException"/> if the given buffer is too small to read a value from.
	/// </summary>
	/// <remarks>
	/// This is trivial for fixed size serializers, but for variable size serializers, whether or not the buffer is
	/// undersized can depend on the contents of the buffer. For these cases the serializer must read from the buffer,
	/// but must not chop the buffer in the case that the buffer is an incorrect size.
	/// </remarks>
	[Test]
	[MethodDataSource(nameof(SerializationTestData))]
	public virtual async Task Deserialize_should_throw_when_buffer_is_too_small(T _, byte[] inputBytes)
	{
		await Assert
			.That(() =>
			{
				ReadOnlySpan<byte> undersizedBuffer = inputBytes.AsSpan(..^1);
				CreateSerializer().Deserialize(undersizedBuffer, null!);
			})
			.ThrowsExactly<ArgumentOutOfRangeException>();
	}

#pragma warning restore xUnit1026 // unused parameter warning
}
