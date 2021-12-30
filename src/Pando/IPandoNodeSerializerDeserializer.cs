using System;
using Pando.DataSources;

namespace Pando;

public interface IPandoNodeSerializer<in T>
{
	ulong Serialize(T obj, INodeDataSink dataSink);
}

public interface IPandoNodeDeserializer<out T>
{
	T Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource);
}

public interface IPandoNodeSerializerDeserializer<T> : IPandoNodeSerializer<T>, IPandoNodeDeserializer<T> { }
