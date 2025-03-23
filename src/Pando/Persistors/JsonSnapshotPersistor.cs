using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pando.Repositories;

namespace Pando.Persistors;

public sealed class JsonSnapshotPersistor : ISnapshotPersistor, IDisposable
{
	internal record struct SnapshotEntry(
		SnapshotId SnapshotId,
		SnapshotId? SourceParentId,
		SnapshotId? TargetParentId,
		NodeId RootNodeId
	);

	private readonly Stream _snapshotIndexStream;

	private JsonSnapshotPersistor(Stream snapshotIndexStream)
	{
		_snapshotIndexStream = snapshotIndexStream;
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
		var entry = new SnapshotEntry(snapshotId, nullableSourceParent, nullableTargetParent, rootNodeId);

		JsonSerializer.Serialize(_snapshotIndexStream, entry, JsonContext.Default.SnapshotEntry);
		_snapshotIndexStream.WriteByte((byte)'\n');
	}

	public async Task<IEnumerable<KeyValuePair<SnapshotId, (SnapshotId, SnapshotId, NodeId)>>> LoadSnapshotIndex()
	{
		_snapshotIndexStream.Seek(0, SeekOrigin.Begin);
		using var reader = new StreamReader(_snapshotIndexStream, Encoding.UTF8, true, -1, true);

		var indexDict = new Dictionary<SnapshotId, (SnapshotId, SnapshotId, NodeId)>();

		while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
		{
			var entry = JsonSerializer.Deserialize(line, JsonContext.Default.SnapshotEntry);
			var tuple = (
				entry.SourceParentId ?? SnapshotId.None,
				entry.TargetParentId ?? SnapshotId.None,
				entry.RootNodeId
			);
			indexDict.Add(entry.SnapshotId, tuple);
		}

		return indexDict;
	}

	public void Dispose()
	{
		_snapshotIndexStream.Dispose();
	}
}
