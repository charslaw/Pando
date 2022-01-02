using System;

namespace Pando.Serialization.PrimitiveSerializers;

public class EnumSerializer<TEnum> : IPrimitiveSerializer<TEnum>
	where TEnum : unmanaged, Enum
{
	private readonly Type _underlyingType;

	public EnumSerializer()
	{
		var enumType = typeof(TEnum);
		_underlyingType = enumType.GetEnumUnderlyingType();

		if (_underlyingType == typeof(nint) || _underlyingType == typeof(nuint))
		{
			throw new ArgumentException($"{_underlyingType.FullName} is not a valid underlying type for EnumSerializer");
		}
	}

	public void Serialize(TEnum value, ref Span<byte> buffer)
	{
		unsafe
		{
			if (_underlyingType == typeof(sbyte))
			{
				SByteSerializer.Default.Serialize(*(sbyte*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(byte))
			{
				ByteSerializer.Default.Serialize(*(byte*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(short))
			{
				Int16LittleEndianSerializer.Default.Serialize(*(short*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(ushort))
			{
				UInt16LittleEndianSerializer.Default.Serialize(*(ushort*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(int))
			{
				Int32LittleEndianSerializer.Default.Serialize(*(int*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(uint))
			{
				UInt32LittleEndianSerializer.Default.Serialize(*(uint*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(long))
			{
				Int64LittleEndianSerializer.Default.Serialize(*(long*)(&value), ref buffer);
			}
			else if (_underlyingType == typeof(ulong))
			{
				UInt64LittleEndianSerializer.Default.Serialize(*(ulong*)(&value), ref buffer);
			}
		}

		// This shouldn't happen because we account for all possible underlying types (except nint and nuint, which are caught in the constructor)
		throw new Exception($"Can't serialize enum with underlying type {_underlyingType.FullName}");
	}

	public TEnum Deserialize(ReadOnlySpan<byte> buffer)
	{
		unsafe
		{
			if (_underlyingType == typeof(sbyte))
			{
				var underlyingValue = SByteSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(byte))
			{
				var underlyingValue = ByteSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(short))
			{
				var underlyingValue = Int16LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(ushort))
			{
				var underlyingValue = UInt16LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(int))
			{
				var underlyingValue = Int32LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(uint))
			{
				var underlyingValue = UInt32LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(long))
			{
				var underlyingValue = Int64LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}

			if (_underlyingType == typeof(ulong))
			{
				var underlyingValue = UInt64LittleEndianSerializer.Default.Deserialize(buffer);
				return *(TEnum*)(&underlyingValue);
			}
		}

		// This shouldn't happen because we account for all possible underlying types (except nint and nuint, which are caught in the constructor)
		throw new Exception($"Can't deserialize enum with underlying type {_underlyingType.FullName}");
	}
}
