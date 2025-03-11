using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Pando.Repositories;

namespace Pando.DataSources;

/// Stores an index of nodes in JSON format.
/// <remarks>
/// This <see cref="INodeDataStore"/> is intended for debugging usage only,
/// as it is not fully optimized in memory usage or speed.
/// </remarks>
public sealed class JsonNodePersistor : INodePersistor, IDisposable
{
	private readonly Stream _nodeIndexStream;
	private readonly Dictionary<NodeId, byte[]> _nodeIndex;
	internal IReadOnlyDictionary<NodeId, byte[]> NodeIndex => _nodeIndex;

	private JsonNodePersistor(Stream indexFileStream)
	{
		_nodeIndexStream = indexFileStream;
		if (indexFileStream is { Length: > 0, CanRead: true })
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

	/// <summary>
	/// Creates a new <see cref="JsonNodePersistor"/> that writes data to the file at the given path.
	/// </summary>
	public static JsonNodePersistor CreateFromFile(string indexFilePath)
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
		return new JsonNodePersistor(stream);
	}

	/// <summary>
	/// Creates a new <see cref="JsonNodePersistor"/> that writes data to the given stream.
	/// </summary>
	/// <remarks>
	/// The stream must be writable and seekable. If the stream is *not* readable,
	/// the contents will be overwritten when a node is added to the store.
	/// </remarks>
	public static JsonNodePersistor CreateFromStream(Stream indexStream)
	{
		ArgumentNullException.ThrowIfNull(indexStream);
		ThrowIfNotWritable(indexStream);
		return new JsonNodePersistor(indexStream);
	}

	public void PersistNode(NodeId nodeId, ReadOnlySpan<byte> data)
	{
		_nodeIndex[nodeId] = data.ToArray();

		_nodeIndexStream.SetLength(0);
		_nodeIndexStream.Seek(0, SeekOrigin.Begin);
		JsonSerializer.Serialize(_nodeIndexStream, _nodeIndex, JsonContext.Default.DictionaryNodeIdByteArray);
	}

	public (Dictionary<NodeId, Range>, byte[]) LoadNodeData()
	{
		var resultIndex = new Dictionary<NodeId, Range>();
		var resultData = new List<byte>();

		foreach (var (nodeId, bytes) in _nodeIndex)
		{
			var current = resultData.Count;
			resultData.AddRange(bytes);
			resultIndex[nodeId] = current..resultData.Count;
		}

		return (resultIndex, resultData.ToArray());
	}

	public void Dispose()
	{
		_nodeIndexStream.Dispose();
	}

	private static void ThrowIfNotWritable(
		Stream stream,
		[CallerArgumentExpression(nameof(stream))] string? paramName = null
	)
	{
		if (!stream.CanWrite)
			throw new ArgumentException("Given stream must be writable.", paramName);
		if (!stream.CanSeek)
			throw new ArgumentException("Given stream must be seekable.", paramName);
	}
}
