using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using Pando;
using Pando.Repositories;

namespace PandoExampleProject.Serializers;

public class ImmutableArraySerializer<TNode> : IPandoNodeSerializerDeserializer<ImmutableArray<TNode>>
{
	private readonly IPandoNodeSerializerDeserializer<TNode> _elementSerializer;

	public ImmutableArraySerializer(IPandoNodeSerializerDeserializer<TNode> elementSerializer)
	{
		_elementSerializer = elementSerializer;
	}

	public ulong Serialize(ImmutableArray<TNode> obj, IWritablePandoNodeRepository repository)
	{
		var len = obj.Length;
		Span<byte> buffer = stackalloc byte[sizeof(ulong) * len];

		for (int i = 0; i < len; i++)
		{
			var hash = _elementSerializer.Serialize(obj[i], repository);
			BinaryPrimitives.WriteUInt64LittleEndian(buffer.Slice(i * sizeof(ulong), sizeof(ulong)), hash);
		}

		return repository.AddNode(buffer);
	}

	public ImmutableArray<TNode> Deserialize(ReadOnlySpan<byte> bytes, IReadablePandoNodeRepository repository)
	{
		var len = bytes.Length / sizeof(ulong);
		var arrayBuilder = ImmutableArray.CreateBuilder<TNode>(len);

		for (int i = 0; i < len; i++)
		{
			var hash = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(sizeof(ulong) * i, sizeof(ulong)));
			arrayBuilder.Add(repository.GetNode(hash, _elementSerializer));
		}

		return arrayBuilder.ToImmutable();
	}
}
