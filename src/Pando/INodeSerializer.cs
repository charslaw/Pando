using System;
using Pando.DataSources;

namespace Pando;

public interface INodeWriter<in T>
{
	ulong Serialize(T obj, INodeDataSink dataSink);
}

public interface INodeReader<out T>
{
	T Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource);
}

public interface INodeSerializer<T> : INodeWriter<T>, INodeReader<T> { }
