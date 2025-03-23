using System;
using Pando.Repositories;
using Pando.Serializers.Utils;
using Pando.Vaults;

namespace Pando.Serializers;

public class NullableNodeSerializer<T>(IPandoSerializer<T> innerSerializer) : IPandoSerializer<T?>
	where T : class
{
	public int SerializedSize => NodeId.SIZE;

	public void Serialize(T? value, Span<byte> buffer, INodeVault nodeVault)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);

		if (value is null)
		{
			buffer[..NodeId.SIZE].Clear();
			return;
		}

		Span<byte> innerBuffer = stackalloc byte[innerSerializer.SerializedSize];
		innerSerializer.Serialize(value, innerBuffer, nodeVault);
		nodeVault.AddNode(innerBuffer, buffer);
	}

	public T? Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);

		var nodeId = NodeId.FromBuffer(buffer);

		if (nodeId == NodeId.None)
		{
			return null;
		}

		Span<byte> innerBuffer = stackalloc byte[innerSerializer.SerializedSize];
		nodeVault.CopyNodeBytesTo(nodeId, innerBuffer);
		return innerSerializer.Deserialize(innerBuffer, nodeVault);
	}

	public void Merge(
		Span<byte> baseBuffer,
		ReadOnlySpan<byte> targetBuffer,
		ReadOnlySpan<byte> sourceBuffer,
		INodeVault nodeVault
	)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);

		if (MergeUtils.TryMergeFastForward(baseBuffer, targetBuffer, sourceBuffer))
			return;

		// If the source value is null, the merged result is null
		if (NodeId.BufferIsNone(sourceBuffer))
		{
			baseBuffer.Clear();
			return;
		}

		// If the target value is null, the merged result is the source value
		if (NodeId.BufferIsNone(targetBuffer))
		{
			targetBuffer.CopyTo(baseBuffer);
			return;
		}

		// Ok, now we know that we're merging two actual, different values
		InnerMerge(baseBuffer, targetBuffer, sourceBuffer, nodeVault);
	}

	// Inner function so that it has a separate stack frame for stack allocation
	private void InnerMerge(
		Span<byte> baseBuffer,
		ReadOnlySpan<byte> targetBuffer,
		ReadOnlySpan<byte> sourceBuffer,
		INodeVault nodeVault
	)
	{
		Span<byte> childrenBuffer = stackalloc byte[NodeId.SIZE * 3];

		var baseChildrenBuffer = childrenBuffer[..NodeId.SIZE];
		nodeVault.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);
		var targetChildrenBuffer = childrenBuffer[NodeId.SIZE..(NodeId.SIZE * 2)];
		nodeVault.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);
		var sourceChildrenBuffer = childrenBuffer[(NodeId.SIZE * 2)..(NodeId.SIZE * 3)];
		nodeVault.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);

		innerSerializer.Merge(baseChildrenBuffer, targetChildrenBuffer, sourceChildrenBuffer, nodeVault);

		nodeVault.AddNode(baseChildrenBuffer, baseBuffer);
	}
}
