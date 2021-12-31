using System;
using Pando.DataSources;

namespace Pando.Serialization;

public interface INodeWriter<in T>
{
	ulong Serialize(T obj, INodeDataSink dataSink);
}

public interface INodeReader<out T>
{
	T Deserialize(ReadOnlySpan<byte> bytes, INodeDataSource dataSource);
}

public interface INodeSerializer<T> : INodeWriter<T>, INodeReader<T>
{
	/// The size in bytes of the data produced by this serializer, if known.
	/// If the size can be dynamic, should return null.
	/// <remarks>This should be constant after initialization. If the size of the data can vary, return null</remarks>
	int? NodeSize { get; }
}
