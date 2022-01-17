using System;

namespace Pando.Serialization.PrimitiveSerializers;

/// <summary>
/// Serializes a specified enum via a serializer capable of serializing the enum's underlying type.
/// </summary>
/// <remarks>
/// In order to create an EnumSerializer, use
/// <see cref="EnumSerializer"/>.<see cref="EnumSerializer.SerializerFor{TEnum}"/> or
/// <see cref="EnumSerializer"/>.<see cref="EnumSerializer.SerializerFor{TEnum, TUnderlying}"/>
/// </remarks>
public sealed class EnumSerializer<TEnum, TUnderlying> : IPrimitiveSerializer<TEnum>
	where TEnum : unmanaged, Enum
	where TUnderlying : unmanaged
{
	private readonly IPrimitiveSerializer<TUnderlying> _underlyingSerializer;

	internal EnumSerializer(IPrimitiveSerializer<TUnderlying> underlyingSerializer)
	{
		// We somewhat dangerously assume that it is safe to convert TEnum to whatever TUnderlying is.
		// This is relatively safe because the only way to construct an EnumSerializer is via the provided factory methods,
		// which create serializers based on the underlying type of the given enum.
		_underlyingSerializer = underlyingSerializer;
		ByteCount = _underlyingSerializer.ByteCount;
	}

	public int? ByteCount { get; }
	public unsafe int ByteCountForValue(TEnum value) => ByteCount ?? _underlyingSerializer.ByteCountForValue(*(TUnderlying*)(&value));

	public unsafe void Serialize(TEnum value, ref Span<byte> buffer)
	{
		// pointer magic: get address of value, cast to pointer to underlying type, dereference pointer to underlying type.
		var underlying = *(TUnderlying*)(&value);
		_underlyingSerializer.Serialize(underlying, ref buffer);
	}

	public unsafe TEnum Deserialize(ref ReadOnlySpan<byte> buffer)
	{
		var underlying = _underlyingSerializer.Deserialize(ref buffer);
		// pointer magic: get address of underlying value, cast to pointer of enum type, dereference pointer to enum type.
		return *(TEnum*)(&underlying);
	}
}

/// <summary>Factory functions for creating <see cref="EnumSerializer{TEnum,TUnderlying}"/></summary>
public static class EnumSerializer
{
	/// <summary>Factory function to create an <see cref="EnumSerializer{TEnum,TUnderlying}"/>
	/// using a default serializer to serialize the underlying value.</summary>
	/// <exception cref="NotSupportedException">thrown if the specified enum type has an unsupported underlying type.
	/// Currently, <c>nint</c> and <c>nuint</c> are not supported.</exception>
	public static IPrimitiveSerializer<TEnum> SerializerFor<TEnum>()
		where TEnum : unmanaged, Enum
	{
		var enumType = typeof(TEnum);
		var underlyingType = enumType.GetEnumUnderlyingType();

		if (underlyingType == typeof(sbyte)) return new EnumSerializer<TEnum, sbyte>(SByteSerializer.Default);
		if (underlyingType == typeof(byte)) return new EnumSerializer<TEnum, byte>(ByteSerializer.Default);
		if (underlyingType == typeof(short)) return new EnumSerializer<TEnum, short>(Int16LittleEndianSerializer.Default);
		if (underlyingType == typeof(ushort)) return new EnumSerializer<TEnum, ushort>(UInt16LittleEndianSerializer.Default);
		if (underlyingType == typeof(int)) return new EnumSerializer<TEnum, int>(Int32LittleEndianSerializer.Default);
		if (underlyingType == typeof(uint)) return new EnumSerializer<TEnum, uint>(UInt32LittleEndianSerializer.Default);
		if (underlyingType == typeof(long)) return new EnumSerializer<TEnum, long>(Int64LittleEndianSerializer.Default);
		if (underlyingType == typeof(ulong)) return new EnumSerializer<TEnum, ulong>(UInt64LittleEndianSerializer.Default);

		throw new NotSupportedException(
			$"Can't get a serializer for {enumType.FullName}: underlying type {underlyingType.FullName} is not supported."
		);
	}

	/// <summary>Factory function to create an <see cref="EnumSerializer{TEnum,TUnderlying}"/>
	/// using the provided <paramref name="underlyingSerializer"/> to serialize the underlying value.</summary>
	/// <exception cref="ArgumentException">thrown if the provided <paramref name="underlyingSerializer"/>
	/// is incapable of serializing the underlying type of <typeparamref name="TEnum"/>.</exception>
	public static IPrimitiveSerializer<TEnum> SerializerFor<TEnum, TUnderlying>(IPrimitiveSerializer<TUnderlying> underlyingSerializer)
		where TEnum : unmanaged, Enum
		where TUnderlying : unmanaged
	{
		var enumType = typeof(TEnum);
		var actualUnderlyingType = enumType.GetEnumUnderlyingType();
		var givenUnderlyingType = typeof(TUnderlying);

		if (actualUnderlyingType != givenUnderlyingType)
		{
			throw new ArgumentException(
				"The given underlying serializer does not match the underlying type of the given enum type." +
				$" Given enum has underlying type {actualUnderlyingType.FullName}" +
				$" while the given serializer serializes type {givenUnderlyingType.FullName}.",
				nameof(underlyingSerializer)
			);
		}

		return new EnumSerializer<TEnum, TUnderlying>(underlyingSerializer);
	}
}
