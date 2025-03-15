using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pando.Repositories;
using Pando.Vaults;

namespace Pando.Persistors;

/// Stores an index of nodes in JSON format.
/// <remarks>
/// This <see cref="INodeVault"/> is intended for debugging usage only,
/// as it is not fully optimized in memory usage or speed.
/// </remarks>
public sealed class JsonNodePersistor : INodePersistor, IDisposable
{
	private readonly Stream _nodeIndexStream;
	private Dictionary<NodeId, byte[]> _nodeIndex;
	internal IReadOnlyDictionary<NodeId, byte[]> NodeIndex => _nodeIndex;

	private JsonNodePersistor(Stream indexJsonStream)
	{
		_nodeIndexStream = indexJsonStream;
		_nodeIndex = new Dictionary<NodeId, byte[]>(Load(indexJsonStream));
	}

	/// Creates a new <see cref="JsonNodePersistor"/> that writes data to the file at the given path.
	public static JsonNodePersistor CreateFromFile(string indexFilePath)
	{
		var stream = StreamExtensions.OpenOrCreate(indexFilePath);
		return new JsonNodePersistor(stream);
	}

	/// Creates a new <see cref="JsonNodePersistor"/> that writes data to the given stream.
	/// <remarks>
	/// The stream must be writable and seekable. If the stream is *not* readable,
	/// the contents will be overwritten when a node is added to the store.
	/// </remarks>
	public static JsonNodePersistor CreateFromStream(Stream indexStream)
	{
		ArgumentNullException.ThrowIfNull(indexStream);
		StreamExtensions.ThrowIfNotWritable(indexStream);
		return new JsonNodePersistor(indexStream);
	}

	public void PersistNode(NodeId nodeId, ReadOnlySpan<byte> data)
	{
		_nodeIndex[nodeId] = data.ToArray();

		_nodeIndexStream.SetLength(0);
		_nodeIndexStream.Seek(0, SeekOrigin.Begin);
		JsonSerializer.Serialize(_nodeIndexStream, _nodeIndex, JsonContext.Default.DictionaryNodeIdByteArray);
	}

	public (IEnumerable<KeyValuePair<NodeId, Range>>, IEnumerable<byte>) LoadNodeData()
	{
		_nodeIndex = new Dictionary<NodeId, byte[]>(Load(_nodeIndexStream));
		var resultIndex = new Dictionary<NodeId, Range>();
		var resultData = new List<byte>();

		var length = 0;
		foreach (var (nodeId, bytes) in _nodeIndex)
		{
			resultData.AddRange(bytes);
			var newLength = length + bytes.Length;
			resultIndex[nodeId] = length..newLength;
			length = newLength;
		}

		return (resultIndex, resultData);
	}

	public void Dispose()
	{
		_nodeIndexStream.Dispose();
	}

	private static IEnumerable<KeyValuePair<NodeId, byte[]>> Load(Stream stream)
	{
		IEnumerable<KeyValuePair<NodeId, byte[]>>? result = null;
		if (stream is { Length: > 0, CanRead: true })
		{
			stream.Seek(0, SeekOrigin.Begin);
			result = JsonSerializer.Deserialize<Dictionary<NodeId, byte[]>>(
				stream,
				JsonContext.Default.DictionaryNodeIdByteArray
			);
		}

		return result ?? [];
	}
}
