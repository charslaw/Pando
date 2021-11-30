using System;
using Pando.Repositories;

namespace Pando;

public interface IPandoNodeSerializer<T>
{
	public ulong Serialize(T obj, IWritablePandoNodeRepository repository);
	public T Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository);
}