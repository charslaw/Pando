using System;
using System.Runtime.CompilerServices;

namespace Pando.Serialization.Utils;

public static class SpanHelpers
{
	/// Removes the given number of elements from the start of the span and returns them
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> PopStart<T>(ref Span<T> input, int length)
	{
		var start = input[..length];
		input = input[length..];
		return start;
	}

	/// Removes the given number of elements from the start of the span and returns them
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> PopStart<T>(ref ReadOnlySpan<T> input, int length)
	{
		var start = input[..length];
		input = input[length..];
		return start;
	}
}
