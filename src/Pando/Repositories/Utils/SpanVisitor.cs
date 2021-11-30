using System;

namespace Pando.Repositories.Utils;

/// A SpanVisitor is an operation that can take in a span of some collection,
/// perform some operation, and return a result, without leaking the span.
/// <remarks>This needs to be a dedicated delegate because <see cref="System.ReadOnlySpan{T}" />
/// can't be used as a type parameter in a <see cref="System.Func{}" /></remarks>
public delegate TResult SpanVisitor<T, out TResult>(ReadOnlySpan<T> span);