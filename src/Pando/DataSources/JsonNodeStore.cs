using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pando.DataSources.Utils;
using Pando.Exceptions;
using Pando.Repositories;

namespace Pando.DataSources;

public class JsonNodeStore : INodeDataStore, IDisposable
{
	private readonly Stream _nodeIndexStream;
	private readonly Dictionary<NodeId, byte[]> _nodeIndex;

	private JsonNodeStore(Stream indexFileStream)
	{
		_nodeIndexStream = indexFileStream;
		if (indexFileStream.Length > 0)
		{
			_nodeIndex =
				JsonSerializer.Deserialize<Dictionary<NodeId, byte[]>>(
					indexFileStream,
					JsonContext.Default.DictionaryNodeIdByteArray
				) ?? new Dictionary<NodeId, byte[]>();
		}
		else
		{
			_nodeIndex = new Dictionary<NodeId, byte[]>();
		}
	}

	public static JsonNodeStore CreateFromFile(string indexFilePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(indexFilePath);

		if (!File.Exists(indexFilePath))
		{
			var directoryName = Path.GetDirectoryName(indexFilePath);
			if (!string.IsNullOrWhiteSpace(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		var stream = File.Open(indexFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		return new JsonNodeStore(stream);
	}

	public static JsonNodeStore CreateFromStream(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);
		return new JsonNodeStore(stream);
	}

	public bool HasNode(ReadOnlySpan<byte> idBuffer) => HasNode(NodeId.FromBuffer(idBuffer));

	public bool HasNode(NodeId nodeId) => _nodeIndex.ContainsKey(nodeId);

	public int GetSizeOfNode(ReadOnlySpan<byte> idBuffer) => GetSizeOfNode(NodeId.FromBuffer(idBuffer));

	public int GetSizeOfNode(NodeId nodeId)
	{
		if (!_nodeIndex.TryGetValue(nodeId, out var arr))
		{
			throw new NodeIdNotFoundException(nodeId, nameof(nodeId));
		}

		return arr.Length;
	}

	public void CopyNodeBytesTo(ReadOnlySpan<byte> idBuffer, Span<byte> outputBytes) =>
		CopyNodeBytesTo(NodeId.FromBuffer(idBuffer), outputBytes);

	public void CopyNodeBytesTo(NodeId nodeId, Span<byte> outputBytes)
	{
		if (!_nodeIndex.TryGetValue(nodeId, out var arr))
		{
			throw new NodeIdNotFoundException(nodeId, nameof(nodeId));
		}

		arr.CopyTo(outputBytes);
	}

	public void AddNode(ReadOnlySpan<byte> bytes, Span<byte> idBuffer) => AddNode(bytes).CopyTo(idBuffer);

	public NodeId AddNode(ReadOnlySpan<byte> bytes)
	{
		var nodeId = HashUtils.ComputeNodeHash(bytes);
		if (_nodeIndex.ContainsKey(nodeId))
		{
			return nodeId;
		}

		_nodeIndex[nodeId] = bytes.ToArray();
		_nodeIndexStream.SetLength(0);
		_nodeIndexStream.Seek(0, SeekOrigin.Begin);
		JsonSerializer.Serialize(_nodeIndexStream, _nodeIndex, JsonContext.Default.DictionaryNodeIdByteArray);
		return nodeId;
	}

	public void Dispose()
	{
		_nodeIndexStream.Dispose();
	}
}
