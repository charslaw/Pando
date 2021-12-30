using System;
using Pando.DataSources;

namespace Pando;

public interface IPandoNodeSerializer<in T>
{
	ulong Serialize(T obj, IWritablePandoNodeRepository repository);
}

public interface IPandoNodeDeserializer<out T>
{
	T Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository);
}

public interface IPandoNodeSerializerDeserializer<T> : IPandoNodeSerializer<T>, IPandoNodeDeserializer<T> { }
