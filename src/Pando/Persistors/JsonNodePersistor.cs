using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
	internal record struct NodeEntry(NodeId NodeId, byte[] Bytes);

	private readonly Stream _nodeIndexStream;

	private JsonNodePersistor(Stream indexJsonStream)
	{
		_nodeIndexStream = indexJsonStream;
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
		var entry = new NodeEntry(nodeId, data.ToArray());
		JsonSerializer.Serialize(_nodeIndexStream, entry, JsonContext.Default.NodeEntry);
		_nodeIndexStream.WriteByte((byte)'\n');
	}

	public async Task<(IEnumerable<KeyValuePair<NodeId, Range>>, IEnumerable<byte>)> LoadNodeData()
	{
		_nodeIndexStream.Seek(0, SeekOrigin.Begin);
		using var reader = new StreamReader(_nodeIndexStream, Encoding.UTF8, true, -1, true);

		var indexDict = new Dictionary<NodeId, Range>();
		var dataList = new List<byte>();

		while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
		{
			var (nodeId, bytes) = JsonSerializer.Deserialize(line, JsonContext.Default.NodeEntry);
			var range = dataList.Count..(dataList.Count + bytes.Length);
			indexDict.Add(nodeId, range);
			dataList.AddRange(bytes);
		}

		return (indexDict, dataList);
	}

	public void Dispose()
	{
		_nodeIndexStream.Dispose();
	}
}
