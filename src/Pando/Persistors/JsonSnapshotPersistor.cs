using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Pando.Repositories;

namespace Pando.Persistors;

public sealed class JsonSnapshotPersistor : ISnapshotPersistor, IDisposable
{
	internal record struct IndexEntry(SnapshotId? SourceParentId, SnapshotId? TargetParentId, NodeId RootNodeId)
	{
		public (SnapshotId, SnapshotId, NodeId) ToTuple() =>
			(SourceParentId ?? SnapshotId.None, TargetParentId ?? SnapshotId.None, RootNodeId);
	}

	private readonly Stream _snapshotIndexStream;
	private Dictionary<SnapshotId, IndexEntry> _snapshotIndex;

	private JsonSnapshotPersistor(Stream snapshotIndexStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
		_snapshotIndex = Load(_snapshotIndexStream);
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
		SnapshotId? nullableSourceParent = sourceParentId == SnapshotId.None ? null : sourceParentId;
		SnapshotId? nullableTargetParent = targetParentId == SnapshotId.None ? null : targetParentId;
		_snapshotIndex[snapshotId] = new IndexEntry(nullableSourceParent, nullableTargetParent, rootNodeId);

		_snapshotIndexStream.SetLength(0);
		_snapshotIndexStream.Seek(0, SeekOrigin.Begin);
		JsonSerializer.Serialize(
			_snapshotIndexStream,
			_snapshotIndex,
			JsonContext.Default.DictionarySnapshotIdIndexEntry
		);
	}

	public IEnumerable<KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>> LoadSnapshotIndex()
	{
		_snapshotIndex = Load(_snapshotIndexStream);
		return _snapshotIndex.Select(kvp => new KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>(
			kvp.Key,
			kvp.Value.ToTuple()
		));
	}

	private static Dictionary<SnapshotId, IndexEntry> Load(Stream stream)
	{
		Dictionary<SnapshotId, IndexEntry>? result = null;
		if (stream is { Length: > 0, CanRead: true })
		{
			stream.Seek(0, SeekOrigin.Begin);
			result = JsonSerializer.Deserialize<Dictionary<SnapshotId, IndexEntry>>(
				stream,
				JsonContext.Default.DictionarySnapshotIdIndexEntry
			);
		}

		return result ?? [];
	}

	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
	}
}
