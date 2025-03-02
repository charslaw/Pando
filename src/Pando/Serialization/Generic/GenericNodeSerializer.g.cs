using System;
using Pando.DataSources;
using Pando.Repositories;
using Pando.Serialization.Utils;

namespace Pando.Serialization.Generic;

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer
	)
	{
		_t1Serializer = t1Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t1EndOffset];

		value.Deconstruct(out var t1);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t1EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);

		return TNode.Construct(t1);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t1EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t1EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t1EndOffset..(_t1EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t1EndOffset * 2)..(_t1EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t2EndOffset];

		value.Deconstruct(out var t1, out var t2);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t2EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);

		return TNode.Construct(t1, t2);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t2EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t2EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t2EndOffset..(_t2EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t2EndOffset * 2)..(_t2EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t3EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t3EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t3EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t3EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t3EndOffset..(_t3EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t3EndOffset * 2)..(_t3EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t4EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t4EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t4EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t4EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t4EndOffset..(_t4EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t4EndOffset * 2)..(_t4EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t5EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t5EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t5EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t5EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t5EndOffset..(_t5EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t5EndOffset * 2)..(_t5EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t6EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t6EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t6EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t6EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t6EndOffset..(_t6EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t6EndOffset * 2)..(_t6EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t7EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t7EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t7EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t7EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t7EndOffset..(_t7EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t7EndOffset * 2)..(_t7EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t8EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t8EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t8EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t8EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t8EndOffset..(_t8EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t8EndOffset * 2)..(_t8EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t9EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t9EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t9EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t9EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t9EndOffset..(_t9EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t9EndOffset * 2)..(_t9EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t10EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t10EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t10EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t10EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t10EndOffset..(_t10EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t10EndOffset * 2)..(_t10EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t11EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t11EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t11EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t11EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t11EndOffset..(_t11EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t11EndOffset * 2)..(_t11EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;
	private readonly IPandoSerializer<T12> _t12Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;
	private readonly int _t12EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer,
		IPandoSerializer<T12> t12Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;
		_t12Serializer = t12Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
		_t12EndOffset = _t11EndOffset + _t12Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t12EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Serialize(t12, childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t12EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t12EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t12EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t12EndOffset..(_t12EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t12EndOffset * 2)..(_t12EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Merge(baseChildrenBuffer[_t11EndOffset.._t12EndOffset], targetChildrenBuffer[_t11EndOffset.._t12EndOffset], sourceChildrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;
	private readonly IPandoSerializer<T12> _t12Serializer;
	private readonly IPandoSerializer<T13> _t13Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;
	private readonly int _t12EndOffset;
	private readonly int _t13EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer,
		IPandoSerializer<T12> t12Serializer,
		IPandoSerializer<T13> t13Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;
		_t12Serializer = t12Serializer;
		_t13Serializer = t13Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
		_t12EndOffset = _t11EndOffset + _t12Serializer.SerializedSize;
		_t13EndOffset = _t12EndOffset + _t13Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t13EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Serialize(t12, childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Serialize(t13, childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t13EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t13EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t13EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t13EndOffset..(_t13EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t13EndOffset * 2)..(_t13EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Merge(baseChildrenBuffer[_t11EndOffset.._t12EndOffset], targetChildrenBuffer[_t11EndOffset.._t12EndOffset], sourceChildrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Merge(baseChildrenBuffer[_t12EndOffset.._t13EndOffset], targetChildrenBuffer[_t12EndOffset.._t13EndOffset], sourceChildrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;
	private readonly IPandoSerializer<T12> _t12Serializer;
	private readonly IPandoSerializer<T13> _t13Serializer;
	private readonly IPandoSerializer<T14> _t14Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;
	private readonly int _t12EndOffset;
	private readonly int _t13EndOffset;
	private readonly int _t14EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer,
		IPandoSerializer<T12> t12Serializer,
		IPandoSerializer<T13> t13Serializer,
		IPandoSerializer<T14> t14Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;
		_t12Serializer = t12Serializer;
		_t13Serializer = t13Serializer;
		_t14Serializer = t14Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
		_t12EndOffset = _t11EndOffset + _t12Serializer.SerializedSize;
		_t13EndOffset = _t12EndOffset + _t13Serializer.SerializedSize;
		_t14EndOffset = _t13EndOffset + _t14Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t14EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Serialize(t12, childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Serialize(t13, childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Serialize(t14, childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t14EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t14EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t14EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t14EndOffset..(_t14EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t14EndOffset * 2)..(_t14EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Merge(baseChildrenBuffer[_t11EndOffset.._t12EndOffset], targetChildrenBuffer[_t11EndOffset.._t12EndOffset], sourceChildrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Merge(baseChildrenBuffer[_t12EndOffset.._t13EndOffset], targetChildrenBuffer[_t12EndOffset.._t13EndOffset], sourceChildrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Merge(baseChildrenBuffer[_t13EndOffset.._t14EndOffset], targetChildrenBuffer[_t13EndOffset.._t14EndOffset], sourceChildrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;
	private readonly IPandoSerializer<T12> _t12Serializer;
	private readonly IPandoSerializer<T13> _t13Serializer;
	private readonly IPandoSerializer<T14> _t14Serializer;
	private readonly IPandoSerializer<T15> _t15Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;
	private readonly int _t12EndOffset;
	private readonly int _t13EndOffset;
	private readonly int _t14EndOffset;
	private readonly int _t15EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer,
		IPandoSerializer<T12> t12Serializer,
		IPandoSerializer<T13> t13Serializer,
		IPandoSerializer<T14> t14Serializer,
		IPandoSerializer<T15> t15Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;
		_t12Serializer = t12Serializer;
		_t13Serializer = t13Serializer;
		_t14Serializer = t14Serializer;
		_t15Serializer = t15Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
		_t12EndOffset = _t11EndOffset + _t12Serializer.SerializedSize;
		_t13EndOffset = _t12EndOffset + _t13Serializer.SerializedSize;
		_t14EndOffset = _t13EndOffset + _t14Serializer.SerializedSize;
		_t15EndOffset = _t14EndOffset + _t15Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t15EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14, out var t15);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Serialize(t12, childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Serialize(t13, childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Serialize(t14, childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		_t15Serializer.Serialize(t15, childrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t15EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		var t15 = _t15Serializer.Deserialize(childrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t15EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t15EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t15EndOffset..(_t15EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t15EndOffset * 2)..(_t15EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Merge(baseChildrenBuffer[_t11EndOffset.._t12EndOffset], targetChildrenBuffer[_t11EndOffset.._t12EndOffset], sourceChildrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Merge(baseChildrenBuffer[_t12EndOffset.._t13EndOffset], targetChildrenBuffer[_t12EndOffset.._t13EndOffset], sourceChildrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Merge(baseChildrenBuffer[_t13EndOffset.._t14EndOffset], targetChildrenBuffer[_t13EndOffset.._t14EndOffset], sourceChildrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		_t15Serializer.Merge(baseChildrenBuffer[_t14EndOffset.._t15EndOffset], targetChildrenBuffer[_t14EndOffset.._t15EndOffset], sourceChildrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
{
	public int SerializedSize => NodeId.SIZE;

	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;
	private readonly IPandoSerializer<T10> _t10Serializer;
	private readonly IPandoSerializer<T11> _t11Serializer;
	private readonly IPandoSerializer<T12> _t12Serializer;
	private readonly IPandoSerializer<T13> _t13Serializer;
	private readonly IPandoSerializer<T14> _t14Serializer;
	private readonly IPandoSerializer<T15> _t15Serializer;
	private readonly IPandoSerializer<T16> _t16Serializer;

	private const int _t0EndOffset = 0;
	private readonly int _t1EndOffset;
	private readonly int _t2EndOffset;
	private readonly int _t3EndOffset;
	private readonly int _t4EndOffset;
	private readonly int _t5EndOffset;
	private readonly int _t6EndOffset;
	private readonly int _t7EndOffset;
	private readonly int _t8EndOffset;
	private readonly int _t9EndOffset;
	private readonly int _t10EndOffset;
	private readonly int _t11EndOffset;
	private readonly int _t12EndOffset;
	private readonly int _t13EndOffset;
	private readonly int _t14EndOffset;
	private readonly int _t15EndOffset;
	private readonly int _t16EndOffset;

	public GenericNodeSerializer(
		IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer,
		IPandoSerializer<T12> t12Serializer,
		IPandoSerializer<T13> t13Serializer,
		IPandoSerializer<T14> t14Serializer,
		IPandoSerializer<T15> t15Serializer,
		IPandoSerializer<T16> t16Serializer
	)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t9Serializer = t9Serializer;
		_t10Serializer = t10Serializer;
		_t11Serializer = t11Serializer;
		_t12Serializer = t12Serializer;
		_t13Serializer = t13Serializer;
		_t14Serializer = t14Serializer;
		_t15Serializer = t15Serializer;
		_t16Serializer = t16Serializer;

		_t1EndOffset = _t0EndOffset + _t1Serializer.SerializedSize;
		_t2EndOffset = _t1EndOffset + _t2Serializer.SerializedSize;
		_t3EndOffset = _t2EndOffset + _t3Serializer.SerializedSize;
		_t4EndOffset = _t3EndOffset + _t4Serializer.SerializedSize;
		_t5EndOffset = _t4EndOffset + _t5Serializer.SerializedSize;
		_t6EndOffset = _t5EndOffset + _t6Serializer.SerializedSize;
		_t7EndOffset = _t6EndOffset + _t7Serializer.SerializedSize;
		_t8EndOffset = _t7EndOffset + _t8Serializer.SerializedSize;
		_t9EndOffset = _t8EndOffset + _t9Serializer.SerializedSize;
		_t10EndOffset = _t9EndOffset + _t10Serializer.SerializedSize;
		_t11EndOffset = _t10EndOffset + _t11Serializer.SerializedSize;
		_t12EndOffset = _t11EndOffset + _t12Serializer.SerializedSize;
		_t13EndOffset = _t12EndOffset + _t13Serializer.SerializedSize;
		_t14EndOffset = _t13EndOffset + _t14Serializer.SerializedSize;
		_t15EndOffset = _t14EndOffset + _t15Serializer.SerializedSize;
		_t16EndOffset = _t15EndOffset + _t16Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t16EndOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14, out var t15, out var t16);
		_t1Serializer.Serialize(t1, childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Serialize(t2, childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Serialize(t3, childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Serialize(t4, childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Serialize(t5, childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Serialize(t6, childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Serialize(t7, childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Serialize(t8, childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Serialize(t9, childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Serialize(t10, childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Serialize(t11, childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Serialize(t12, childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Serialize(t13, childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Serialize(t14, childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		_t15Serializer.Serialize(t15, childrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);
		_t16Serializer.Serialize(t16, childrenBuffer[_t15EndOffset.._t16EndOffset], dataStore);

		dataStore.AddNode(childrenBuffer, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeDataStore dataStore)
	{
		Span<byte> childrenBuffer = stackalloc byte[_t16EndOffset];
		dataStore.CopyNodeBytesTo(buffer, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		var t15 = _t15Serializer.Deserialize(childrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);
		var t16 = _t16Serializer.Deserialize(childrenBuffer[_t15EndOffset.._t16EndOffset], dataStore);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
	}

	public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, INodeDataStore dataStore)
	{
		if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;
		if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;

		// allocate a buffer to contain the children data of base, target, and source
		Span<byte> childrenBuffer = stackalloc byte[_t16EndOffset * 3];

		var baseChildrenBuffer = childrenBuffer[.._t16EndOffset];
		dataStore.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[_t16EndOffset..(_t16EndOffset * 2)];
		dataStore.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(_t16EndOffset * 2)..(_t16EndOffset * 3)];
		dataStore.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		// merge each child
		_t1Serializer.Merge(baseChildrenBuffer[_t0EndOffset.._t1EndOffset], targetChildrenBuffer[_t0EndOffset.._t1EndOffset], sourceChildrenBuffer[_t0EndOffset.._t1EndOffset], dataStore);
		_t2Serializer.Merge(baseChildrenBuffer[_t1EndOffset.._t2EndOffset], targetChildrenBuffer[_t1EndOffset.._t2EndOffset], sourceChildrenBuffer[_t1EndOffset.._t2EndOffset], dataStore);
		_t3Serializer.Merge(baseChildrenBuffer[_t2EndOffset.._t3EndOffset], targetChildrenBuffer[_t2EndOffset.._t3EndOffset], sourceChildrenBuffer[_t2EndOffset.._t3EndOffset], dataStore);
		_t4Serializer.Merge(baseChildrenBuffer[_t3EndOffset.._t4EndOffset], targetChildrenBuffer[_t3EndOffset.._t4EndOffset], sourceChildrenBuffer[_t3EndOffset.._t4EndOffset], dataStore);
		_t5Serializer.Merge(baseChildrenBuffer[_t4EndOffset.._t5EndOffset], targetChildrenBuffer[_t4EndOffset.._t5EndOffset], sourceChildrenBuffer[_t4EndOffset.._t5EndOffset], dataStore);
		_t6Serializer.Merge(baseChildrenBuffer[_t5EndOffset.._t6EndOffset], targetChildrenBuffer[_t5EndOffset.._t6EndOffset], sourceChildrenBuffer[_t5EndOffset.._t6EndOffset], dataStore);
		_t7Serializer.Merge(baseChildrenBuffer[_t6EndOffset.._t7EndOffset], targetChildrenBuffer[_t6EndOffset.._t7EndOffset], sourceChildrenBuffer[_t6EndOffset.._t7EndOffset], dataStore);
		_t8Serializer.Merge(baseChildrenBuffer[_t7EndOffset.._t8EndOffset], targetChildrenBuffer[_t7EndOffset.._t8EndOffset], sourceChildrenBuffer[_t7EndOffset.._t8EndOffset], dataStore);
		_t9Serializer.Merge(baseChildrenBuffer[_t8EndOffset.._t9EndOffset], targetChildrenBuffer[_t8EndOffset.._t9EndOffset], sourceChildrenBuffer[_t8EndOffset.._t9EndOffset], dataStore);
		_t10Serializer.Merge(baseChildrenBuffer[_t9EndOffset.._t10EndOffset], targetChildrenBuffer[_t9EndOffset.._t10EndOffset], sourceChildrenBuffer[_t9EndOffset.._t10EndOffset], dataStore);
		_t11Serializer.Merge(baseChildrenBuffer[_t10EndOffset.._t11EndOffset], targetChildrenBuffer[_t10EndOffset.._t11EndOffset], sourceChildrenBuffer[_t10EndOffset.._t11EndOffset], dataStore);
		_t12Serializer.Merge(baseChildrenBuffer[_t11EndOffset.._t12EndOffset], targetChildrenBuffer[_t11EndOffset.._t12EndOffset], sourceChildrenBuffer[_t11EndOffset.._t12EndOffset], dataStore);
		_t13Serializer.Merge(baseChildrenBuffer[_t12EndOffset.._t13EndOffset], targetChildrenBuffer[_t12EndOffset.._t13EndOffset], sourceChildrenBuffer[_t12EndOffset.._t13EndOffset], dataStore);
		_t14Serializer.Merge(baseChildrenBuffer[_t13EndOffset.._t14EndOffset], targetChildrenBuffer[_t13EndOffset.._t14EndOffset], sourceChildrenBuffer[_t13EndOffset.._t14EndOffset], dataStore);
		_t15Serializer.Merge(baseChildrenBuffer[_t14EndOffset.._t15EndOffset], targetChildrenBuffer[_t14EndOffset.._t15EndOffset], sourceChildrenBuffer[_t14EndOffset.._t15EndOffset], dataStore);
		_t16Serializer.Merge(baseChildrenBuffer[_t15EndOffset.._t16EndOffset], targetChildrenBuffer[_t15EndOffset.._t16EndOffset], sourceChildrenBuffer[_t15EndOffset.._t16EndOffset], dataStore);

		dataStore.AddNode(baseChildrenBuffer, baseBuffer);
	}

}

