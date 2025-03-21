using System;

namespace Pando.Serializers.Utils;

/// <summary>
/// Utilities for performing common merge tasks
/// </summary>
public static class MergeUtils
{
	/// Overwrites <paramref name="baseBuffer"/> with contents of <paramref name="sourceBuffer"/> if it differs from <paramref name="baseBuffer"/>,
	/// otherwise overwrites <paramref name="baseBuffer"/> with contents of <paramref name="targetBuffer"/>.
	public static void MergeInline(
		Span<byte> baseBuffer,
		ReadOnlySpan<byte> targetBuffer,
		ReadOnlySpan<byte> sourceBuffer
	)
	{
		if (baseBuffer.SequenceEqual(sourceBuffer))
		{
			targetBuffer.CopyTo(baseBuffer);
		}
		else
		{
			sourceBuffer.CopyTo(baseBuffer);
		}
	}

	/// Overwrites contents of <paramref name="baseBuffer"/> with contents of <paramref name="mergeBuffer"/>
	/// if the existing contents match <paramref name="cmpBuffer"/>.
	public static bool MergeIfUnchanged(
		Span<byte> baseBuffer,
		ReadOnlySpan<byte> cmpBuffer,
		ReadOnlySpan<byte> mergeBuffer
	)
	{
		if (baseBuffer.SequenceEqual(cmpBuffer))
		{
			mergeBuffer.CopyTo(baseBuffer);
			return true;
		}

		return false;
	}
}
