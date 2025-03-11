using System;
using Pando.Repositories;

namespace Pando.Vaults;

public static class NodeVaultExtensions
{
	/// Adds a node to the data store and populates the given <paramref name="idBuffer"/> with
	/// the bytes of the <see cref="NodeId"/> hash that can be used to retrieve the node.
	public static void AddNode(this IWriteOnlyNodeVault nodeVault, ReadOnlySpan<byte> bytes, Span<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);
		nodeVault.AddNode(bytes).CopyTo(idBuffer);
	}

	/// Returns whether a node identified by the given <see cref="NodeId"/> hash exists in the data source.
	public static bool HasNode(this IReadOnlyNodeVault nodeVault, ReadOnlySpan<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);
		return nodeVault.HasNode(NodeId.FromBuffer(idBuffer));
	}

	/// Gets the size in bytes of the node identified by the given <see cref="NodeId"/> hash.
	public static int GetSizeOfNode(this IReadOnlyNodeVault nodeVault, ReadOnlySpan<byte> idBuffer)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);
		return nodeVault.GetSizeOfNode(NodeId.FromBuffer(idBuffer));
	}

	/// Copies the binary representation of the node with the given <see cref="NodeId"/> hash into the given Span.
	public static void CopyNodeBytesTo(
		this IReadOnlyNodeVault nodeVault,
		ReadOnlySpan<byte> idBuffer,
		Span<byte> outputBytes
	)
	{
		ArgumentNullException.ThrowIfNull(nodeVault);
		nodeVault.CopyNodeBytesTo(NodeId.FromBuffer(idBuffer), outputBytes);
	}
}
