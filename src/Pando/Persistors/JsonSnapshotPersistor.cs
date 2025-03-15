using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pando.Repositories;

namespace Pando.Persistors;

using IndexEntry = (SnapshotId sourceParentId, SnapshotId targetParentId, NodeId rootNodeId);

public sealed class JsonSnapshotPersistor : ISnapshotPersistor, IDisposable
{
	private readonly Stream _snapshotIndexStream;
	private Dictionary<SnapshotId, IndexEntry> _snapshotIndex;

	private JsonSnapshotPersistor(Stream snapshotIndexStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
		_snapshotIndex = new Dictionary<SnapshotId, IndexEntry>(Load(_snapshotIndexStream));
	}

	/// Creates a new <see cref="JsonSnapshotPersistor"/> that writes data to the file at the given path.
	public static JsonSnapshotPersistor CreateFromFile(string indexFilePath)
	{
		var stream = StreamExtensions.OpenOrCreate(indexFilePath);
		return new JsonSnapshotPersistor(stream);
	}

	/// Creates a new <see cref="JsonSnapshotPersistor"/> that writes data to the given stream.
	/// <remarks>
	/// The stream must be writable and seekable. If the stream is *not* readable,
	/// the contents will be overwritten when a node is added to the store.
	/// </remarks>
	public static JsonSnapshotPersistor CreateFromStream(Stream indexStream)
	{
		ArgumentNullException.ThrowIfNull(indexStream);
		StreamExtensions.ThrowIfNotWritable(indexStream);
		return new JsonSnapshotPersistor(indexStream);
	}

	public void PersistSnapshot(
		SnapshotId snapshotId,
		SnapshotId sourceParentId,
		SnapshotId targetParentId,
		NodeId rootNodeId
	)
	{
		_snapshotIndex[snapshotId] = (sourceParentId, targetParentId, rootNodeId);

		_snapshotIndexStream.SetLength(0);
		_snapshotIndexStream.Seek(0, SeekOrigin.Begin);
		JsonSerializer.Serialize(
			_snapshotIndexStream,
			_snapshotIndex,
			JsonContext.Default.DictionarySnapshotIdValueTupleSnapshotIdSnapshotIdNodeId
		);
	}

	public IEnumerable<KeyValuePair<SnapshotId, IndexEntry>> LoadSnapshotIndex()
	{
		_snapshotIndex = new Dictionary<SnapshotId, IndexEntry>(Load(_snapshotIndexStream));
		return _snapshotIndex;
	}

	private IEnumerable<KeyValuePair<SnapshotId, IndexEntry>> Load(Stream stream)
	{
		IEnumerable<KeyValuePair<SnapshotId, IndexEntry>>? result = null;
		if (stream is { Length: > 0, CanRead: true })
		{
			result = JsonSerializer.Deserialize<Dictionary<SnapshotId, IndexEntry>>(
				_snapshotIndexStream,
				JsonContext.Default.DictionarySnapshotIdValueTupleSnapshotIdSnapshotIdNodeId
			);
		}

		return result ?? [];
	}

	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
	}
}
