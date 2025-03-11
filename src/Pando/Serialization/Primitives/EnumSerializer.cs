using System;
using System.Runtime.CompilerServices;
using Pando.Vaults;

namespace Pando.Serialization.Primitives;

/// <summary>
/// Serializes a specified enum via a serializer capable of serializing the enum's underlying type.
/// </summary>
/// <remarks>
/// In order to create an EnumSerializer, use
/// <see cref="EnumSerializer"/>.<see cref="EnumSerializer.SerializerFor{TEnum}"/> or
/// <see cref="EnumSerializer"/>.<see cref="EnumSerializer.SerializerFor{TEnum, TUnderlying}"/>
/// </remarks>
public sealed class EnumSerializer<TEnum, TUnderlying> : IPandoSerializer<TEnum>
	where TEnum : unmanaged, Enum
	where TUnderlying : unmanaged
{
	public int SerializedSize { get; }

	private readonly IPandoSerializer<TUnderlying> _underlyingSerializer;

	internal EnumSerializer(IPandoSerializer<TUnderlying> underlyingSerializer)
	{
		// We somewhat dangerously assume that it is safe to convert TEnum to whatever TUnderlying is.
		// This is relatively safe because the only way to construct an EnumSerializer is via the provided factory methods,
		// which create serializers based on the underlying type of the given enum.
		_underlyingSerializer = underlyingSerializer;
		SerializedSize = underlyingSerializer.SerializedSize;
	}

	public void Serialize(TEnum value, Span<byte> buffer, INodeVault nodeVault)
	{
		var underlying = ToUnderlying(ref value);
		_underlyingSerializer.Serialize(underlying, buffer, nodeVault);
	}

	public TEnum Deserialize(ReadOnlySpan<byte> buffer, IReadOnlyNodeVault nodeVault)
	{
		var underlying = _underlyingSerializer.Deserialize(buffer, nodeVault);
		return ToEnum(ref underlying);
	}

	private static TUnderlying ToUnderlying(ref TEnum value) => Unsafe.As<TEnum, TUnderlying>(ref value);

	private static TEnum ToEnum(ref TUnderlying value) => Unsafe.As<TUnderlying, TEnum>(ref value);
}

/// <summary>Factory functions for creating <see cref="EnumSerializer{TEnum,TUnderlying}"/></summary>
public static class EnumSerializer
{
	/// <summary>Factory function to create an <see cref="EnumSerializer{TEnum,TUnderlying}"/>
	/// using a default serializer to serialize the underlying value.</summary>
	/// <exception cref="NotSupportedException">thrown if the specified enum type has an unsupported underlying type.
	/// Currently, <c>nint</c> and <c>nuint</c> are not supported.</exception>
	public static IPandoSerializer<TEnum> SerializerFor<TEnum>()
		where TEnum : unmanaged, Enum
	{
		var enumType = typeof(TEnum);
		var underlyingType = enumType.GetEnumUnderlyingType();

		if (underlyingType == typeof(sbyte))
			return new EnumSerializer<TEnum, sbyte>(SByteSerializer.Default);
		if (underlyingType == typeof(byte))
			return new EnumSerializer<TEnum, byte>(ByteSerializer.Default);
		if (underlyingType == typeof(short))
			return new EnumSerializer<TEnum, short>(Int16LittleEndianSerializer.Default);
		if (underlyingType == typeof(ushort))
			return new EnumSerializer<TEnum, ushort>(UInt16LittleEndianSerializer.Default);
		if (underlyingType == typeof(int))
			return new EnumSerializer<TEnum, int>(Int32LittleEndianSerializer.Default);
		if (underlyingType == typeof(uint))
			return new EnumSerializer<TEnum, uint>(UInt32LittleEndianSerializer.Default);
		if (underlyingType == typeof(long))
			return new EnumSerializer<TEnum, long>(Int64LittleEndianSerializer.Default);
		if (underlyingType == typeof(ulong))
			return new EnumSerializer<TEnum, ulong>(UInt64LittleEndianSerializer.Default);

		throw new NotSupportedException(
			$"Can't get a serializer for {enumType.FullName}: underlying type {underlyingType.FullName} is not supported."
		);
	}

	/// <summary>Factory function to create an <see cref="EnumSerializer{TEnum,TUnderlying}"/>
	/// using the provided <paramref name="underlyingSerializer"/> to serialize the underlying value.</summary>
	/// <exception cref="ArgumentException">thrown if the provided <paramref name="underlyingSerializer"/>
	/// is incapable of serializing the underlying type of <typeparamref name="TEnum"/>.</exception>
	public static IPandoSerializer<TEnum> SerializerFor<TEnum, TUnderlying>(
		IPandoSerializer<TUnderlying> underlyingSerializer
	)
		where TEnum : unmanaged, Enum
		where TUnderlying : unmanaged
	{
		ArgumentNullException.ThrowIfNull(underlyingSerializer);

		var enumType = typeof(TEnum);
		var actualUnderlyingType = enumType.GetEnumUnderlyingType();
		var givenUnderlyingType = typeof(TUnderlying);

		if (actualUnderlyingType != givenUnderlyingType)
		{
			throw new ArgumentException(
				"The given underlying serializer does not match the underlying type of the given enum type."
					+ $" Given enum has underlying type {actualUnderlyingType.FullName}"
					+ $" while the given serializer serializes type {givenUnderlyingType.FullName}.",
				nameof(underlyingSerializer)
			);
		}

		return new EnumSerializer<TEnum, TUnderlying>(underlyingSerializer);
	}
}
