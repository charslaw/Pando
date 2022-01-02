namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

// For unsigned
// Mid1 = ((Max - Min) * (Math.PI - 3)) + Min
// Mid2 = ((Max - Min) * (Math.E - 2)) + Min

// For signed
// Mid1 = ((Max - Min) * (1 - (Math.E - 2))) + Min
// Mid2 = ((Max - Min) * (1 - (Math.PI - 3))) + Min

public enum SByteEnum : sbyte
{
	Min = -128,
	Mid1 = -56, // ((Max - Min) * 0.28172) + Min
	Mid2 = 91,  // ((Max - Min) * 0.85841) + Min
	Max = 127,
}

public enum ByteEnum : byte
{
	Min = 0,
	Mid1 = 37,  // ((Max - Min) * 0.14159) + Min
	Mid2 = 184, // ((Max - Min) * 0.71828) + Min
	Max = 255,
}

public enum Int16Enum : short
{
	Min = -32_768,
	Mid1 = -14_305, // ((Max - Min) * 0.28172) + Min
	Mid2 = 23_488,  // ((Max - Min) * 0.85841) + Min
	Max = 32_767,
}

public enum UInt16Enum : ushort
{
	Min = 0,
	Mid1 = 9_280,  // ((Max - Min) * 0.14159) + Min
	Mid2 = 47_073, // ((Max - Min) * 0.71828) + Min
	Max = 65_535,
}

public enum Int32Enum : int
{
	Min = -2_147_483_648,
	Mid1 = -937_513_314,  // ((Max - Min) * 0.28172) + Min
	Mid2 = 1_539_347_831, // ((Max - Min) * 0.85841) + Min
	Max = 2_147_483_647,
}

public enum UInt32Enum : uint
{
	Min = 0,
	Mid1 = 608_135_817,   // ((Max - Min) * 0.14159) + Min
	Mid2 = 3_084_996_962, // ((Max - Min) * 0.71828) + Min
	Max = 4_294_967_295,
}

public enum Int64Enum : long
{
	Min = -9_223_372_036_854_775_808,
	Mid1 = -4_026_589_025_525_376_000, // ((Max - Min) * 0.28172) + Min
	Mid2 = 6_611_448_593_366_448_128,  // ((Max - Min) * 0.85841) + Min
	Max = 9_223_372_036_854_775_807,
}

public enum UInt64Enum : ulong
{
	Min = 0,
	Mid1 = 2_611_923_443_488_325_120,  // ((Max - Min) * 0.14159) + Min
	Mid2 = 13_249_961_062_380_148_736, // ((Max - Min) * 0.71828) + Min
	Max = 18_446_744_073_709_551_615,
}
