using System;
using Pando.DataSources;
using Pando.DataSources.Utils;

namespace Pando.Serialization.Generic;

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;

	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer)
	{
		_t1Serializer = t1Serializer;
		_endOffset = t1Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1);
		_t1Serializer.Serialize(t1, childrenBuffer[.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._endOffset], dataSource);

		return TNode.Construct(t1);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;

	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer, IPandoSerializer<T2> t2Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_endOffset = _t2Offset + t2Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2);
	}
}


/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_endOffset = _t3Offset + t3Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._endOffset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._endOffset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_endOffset = _t4Offset + t4Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_endOffset = _t5Offset + t5Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_endOffset = _t6Offset + t6Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_endOffset = _t7Offset + t7Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer)
	{
		_t1Serializer = t1Serializer;
		_t2Serializer = t2Serializer;
		_t3Serializer = t3Serializer;
		_t4Serializer = t4Serializer;
		_t5Serializer = t5Serializer;
		_t6Serializer = t6Serializer;
		_t7Serializer = t7Serializer;
		_t8Serializer = t8Serializer;
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_endOffset = _t8Offset + t8Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _endOffset;
	private readonly IPandoSerializer<T1> _t1Serializer;
	private readonly IPandoSerializer<T2> _t2Serializer;
	private readonly IPandoSerializer<T3> _t3Serializer;
	private readonly IPandoSerializer<T4> _t4Serializer;
	private readonly IPandoSerializer<T5> _t5Serializer;
	private readonly IPandoSerializer<T6> _t6Serializer;
	private readonly IPandoSerializer<T7> _t7Serializer;
	private readonly IPandoSerializer<T8> _t8Serializer;
	private readonly IPandoSerializer<T9> _t9Serializer;

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_endOffset = _t9Offset + t9Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_endOffset = _t10Offset + t10Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
		IPandoSerializer<T2> t2Serializer,
		IPandoSerializer<T3> t3Serializer,
		IPandoSerializer<T4> t4Serializer,
		IPandoSerializer<T5> t5Serializer,
		IPandoSerializer<T6> t6Serializer,
		IPandoSerializer<T7> t7Serializer,
		IPandoSerializer<T8> t8Serializer,
		IPandoSerializer<T9> t9Serializer,
		IPandoSerializer<T10> t10Serializer,
		IPandoSerializer<T11> t11Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_endOffset = _t11Offset + t11Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _t12Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
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
		IPandoSerializer<T12> t12Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_t12Offset = _t11Offset + t11Serializer.SerializedSize;
		_endOffset = _t12Offset + t12Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);
		_t12Serializer.Serialize(t12, childrenBuffer[_t12Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t12Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _t12Offset;
	private readonly int _t13Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
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
		IPandoSerializer<T13> t13Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_t12Offset = _t11Offset + t11Serializer.SerializedSize;
		_t13Offset = _t12Offset + t12Serializer.SerializedSize;
		_endOffset = _t13Offset + t13Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);
		_t12Serializer.Serialize(t12, childrenBuffer[_t12Offset.._endOffset], dataSink);
		_t13Serializer.Serialize(t13, childrenBuffer[_t13Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t12Offset.._endOffset], dataSource);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t13Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _t12Offset;
	private readonly int _t13Offset;
	private readonly int _t14Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
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
		IPandoSerializer<T14> t14Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_t12Offset = _t11Offset + t11Serializer.SerializedSize;
		_t13Offset = _t12Offset + t12Serializer.SerializedSize;
		_t14Offset = _t13Offset + t13Serializer.SerializedSize;
		_endOffset = _t14Offset + t14Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);
		_t12Serializer.Serialize(t12, childrenBuffer[_t12Offset.._endOffset], dataSink);
		_t13Serializer.Serialize(t13, childrenBuffer[_t13Offset.._endOffset], dataSink);
		_t14Serializer.Serialize(t14, childrenBuffer[_t14Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t12Offset.._endOffset], dataSource);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t13Offset.._endOffset], dataSource);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t14Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
	}
}

/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _t12Offset;
	private readonly int _t13Offset;
	private readonly int _t14Offset;
	private readonly int _t15Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
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
		IPandoSerializer<T15> t15Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_t12Offset = _t11Offset + t11Serializer.SerializedSize;
		_t13Offset = _t12Offset + t12Serializer.SerializedSize;
		_t14Offset = _t13Offset + t13Serializer.SerializedSize;
		_t15Offset = _t14Offset + t14Serializer.SerializedSize;
		_endOffset = _t15Offset + t15Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14, out var t15);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);
		_t12Serializer.Serialize(t12, childrenBuffer[_t12Offset.._endOffset], dataSink);
		_t13Serializer.Serialize(t13, childrenBuffer[_t13Offset.._endOffset], dataSink);
		_t14Serializer.Serialize(t14, childrenBuffer[_t14Offset.._endOffset], dataSink);
		_t15Serializer.Serialize(t15, childrenBuffer[_t15Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t12Offset.._endOffset], dataSource);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t13Offset.._endOffset], dataSource);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t14Offset.._endOffset], dataSource);
		var t15 = _t15Serializer.Deserialize(childrenBuffer[_t15Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
	}
}


/// <summary>
/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15}"/>.
/// </summary>
public class GenericNodeSerializer<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : IPandoSerializer<TNode>
	where TNode : IGenericSerializable<TNode, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
{
	public int SerializedSize => sizeof(ulong);

	private readonly int _t2Offset;
	private readonly int _t3Offset;
	private readonly int _t4Offset;
	private readonly int _t5Offset;
	private readonly int _t6Offset;
	private readonly int _t7Offset;
	private readonly int _t8Offset;
	private readonly int _t9Offset;
	private readonly int _t10Offset;
	private readonly int _t11Offset;
	private readonly int _t12Offset;
	private readonly int _t13Offset;
	private readonly int _t14Offset;
	private readonly int _t15Offset;
	private readonly int _t16Offset;
	private readonly int _endOffset;
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

	/// <summary>
	/// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15}"/>.
	/// </summary>
	public GenericNodeSerializer(IPandoSerializer<T1> t1Serializer,
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
		IPandoSerializer<T16> t16Serializer)
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
		_t2Offset = t1Serializer.SerializedSize;
		_t3Offset = _t2Offset + t2Serializer.SerializedSize;
		_t4Offset = _t3Offset + t3Serializer.SerializedSize;
		_t5Offset = _t4Offset + t4Serializer.SerializedSize;
		_t6Offset = _t5Offset + t5Serializer.SerializedSize;
		_t7Offset = _t6Offset + t6Serializer.SerializedSize;
		_t8Offset = _t7Offset + t7Serializer.SerializedSize;
		_t9Offset = _t8Offset + t8Serializer.SerializedSize;
		_t10Offset = _t9Offset + t9Serializer.SerializedSize;
		_t11Offset = _t10Offset + t10Serializer.SerializedSize;
		_t12Offset = _t11Offset + t11Serializer.SerializedSize;
		_t13Offset = _t12Offset + t12Serializer.SerializedSize;
		_t14Offset = _t13Offset + t13Serializer.SerializedSize;
		_t15Offset = _t14Offset + t14Serializer.SerializedSize;
		_t16Offset = _t15Offset + t15Serializer.SerializedSize;
		_endOffset = _t16Offset + t16Serializer.SerializedSize;
	}

	public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)
	{
		Span<byte> childrenBuffer = stackalloc byte[_endOffset];

		value.Deconstruct(out var t1, out var t2, out var t3, out var t4, out var t5, out var t6, out var t7, out var t8, out var t9, out var t10, out var t11, out var t12, out var t13, out var t14, out var t15, out var t16);
		_t1Serializer.Serialize(t1, childrenBuffer[.._t2Offset], dataSink);
		_t2Serializer.Serialize(t2, childrenBuffer[_t2Offset.._t3Offset], dataSink);
		_t3Serializer.Serialize(t3, childrenBuffer[_t3Offset.._t4Offset], dataSink);
		_t4Serializer.Serialize(t4, childrenBuffer[_t4Offset.._t5Offset], dataSink);
		_t5Serializer.Serialize(t5, childrenBuffer[_t5Offset.._t6Offset], dataSink);
		_t6Serializer.Serialize(t6, childrenBuffer[_t6Offset.._t7Offset], dataSink);
		_t7Serializer.Serialize(t7, childrenBuffer[_t7Offset.._t8Offset], dataSink);
		_t8Serializer.Serialize(t8, childrenBuffer[_t8Offset.._t9Offset], dataSink);
		_t9Serializer.Serialize(t9, childrenBuffer[_t9Offset.._t10Offset], dataSink);
		_t10Serializer.Serialize(t10, childrenBuffer[_t10Offset.._endOffset], dataSink);
		_t11Serializer.Serialize(t11, childrenBuffer[_t11Offset.._endOffset], dataSink);
		_t12Serializer.Serialize(t12, childrenBuffer[_t12Offset.._endOffset], dataSink);
		_t13Serializer.Serialize(t13, childrenBuffer[_t13Offset.._endOffset], dataSink);
		_t14Serializer.Serialize(t14, childrenBuffer[_t14Offset.._endOffset], dataSink);
		_t15Serializer.Serialize(t15, childrenBuffer[_t15Offset.._endOffset], dataSink);
		_t16Serializer.Serialize(t16, childrenBuffer[_t16Offset.._endOffset], dataSink);

		var hash = dataSink.AddNode(childrenBuffer);
		ByteEncoder.CopyBytes(hash, buffer);
	}

	public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)
	{
		var hash = ByteEncoder.GetUInt64(buffer);

		var nodeDataSize = dataSource.GetSizeOfNode(hash);
		Span<byte> childrenBuffer = stackalloc byte[nodeDataSize];
		dataSource.CopyNodeBytesTo(hash, childrenBuffer);

		var t1 = _t1Serializer.Deserialize(childrenBuffer[.._t2Offset], dataSource);
		var t2 = _t2Serializer.Deserialize(childrenBuffer[_t2Offset.._t3Offset], dataSource);
		var t3 = _t3Serializer.Deserialize(childrenBuffer[_t3Offset.._t4Offset], dataSource);
		var t4 = _t4Serializer.Deserialize(childrenBuffer[_t4Offset.._t5Offset], dataSource);
		var t5 = _t5Serializer.Deserialize(childrenBuffer[_t5Offset.._t6Offset], dataSource);
		var t6 = _t6Serializer.Deserialize(childrenBuffer[_t6Offset.._t7Offset], dataSource);
		var t7 = _t7Serializer.Deserialize(childrenBuffer[_t7Offset.._t8Offset], dataSource);
		var t8 = _t8Serializer.Deserialize(childrenBuffer[_t8Offset.._t9Offset], dataSource);
		var t9 = _t9Serializer.Deserialize(childrenBuffer[_t9Offset.._t10Offset], dataSource);
		var t10 = _t10Serializer.Deserialize(childrenBuffer[_t10Offset.._endOffset], dataSource);
		var t11 = _t11Serializer.Deserialize(childrenBuffer[_t11Offset.._endOffset], dataSource);
		var t12 = _t12Serializer.Deserialize(childrenBuffer[_t12Offset.._endOffset], dataSource);
		var t13 = _t13Serializer.Deserialize(childrenBuffer[_t13Offset.._endOffset], dataSource);
		var t14 = _t14Serializer.Deserialize(childrenBuffer[_t14Offset.._endOffset], dataSource);
		var t15 = _t15Serializer.Deserialize(childrenBuffer[_t15Offset.._endOffset], dataSource);
		var t16 = _t16Serializer.Deserialize(childrenBuffer[_t16Offset.._endOffset], dataSource);

		return TNode.Construct(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
	}
}
