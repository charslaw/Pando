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

	/// Merges two nodes if a fast-forward merge can be performed.
	/// If <paramref name="targetBuffer"/> is unchanged from <paramref name="baseBuffer"/>, take <paramref name="sourceBuffer"/>.
	/// If <paramref name="sourceBuffer"/> is unchanged from <paramref name="baseBuffer"/>, take <paramref name="targetBuffer"/>.
	/// If <paramref name="targetBuffer"/> and <paramref name="sourceBuffer"/> are the same, take their value.
	/// <returns>Whether a fast-forward merge was performed.</returns>
	public static bool TryMergeFastForward(
		Span<byte> baseBuffer,
		ReadOnlySpan<byte> targetBuffer,
		ReadOnlySpan<byte> sourceBuffer
	)
	{
		// if target is unchanged from base, merged value is source
		if (targetBuffer.SequenceEqual(baseBuffer))
		{
			sourceBuffer.CopyTo(baseBuffer);
			return true;
		}

		// if source is unchanged from base, merged value is target
		if (sourceBuffer.SequenceEqual(baseBuffer))
		{
			targetBuffer.CopyTo(baseBuffer);
			return true;
		}

		// if source and target are the same, merged value is both
		if (sourceBuffer.SequenceEqual(targetBuffer))
		{
			sourceBuffer.CopyTo(baseBuffer);
			return true;
		}

		return false;
	}
}
