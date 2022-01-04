using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace PandoTests.Tests.Serialization.PrimitiveSerializers;

/// This isn't actively used anywhere in the codebase,
/// it is just the code used to generate the enums in TestEnums.cs, included for illustrative purposes.
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class GenerateTestEnums
{
	private static readonly Dictionary<Type, (BigInteger Min, BigInteger Max)> types = new()
	{
		{ typeof(sbyte), (sbyte.MinValue, sbyte.MaxValue) },
		{ typeof(byte), (byte.MinValue, byte.MaxValue) },
		{ typeof(short), (short.MinValue, short.MaxValue) },
		{ typeof(ushort), (ushort.MinValue, ushort.MaxValue) },
		{ typeof(int), (int.MinValue, int.MaxValue) },
		{ typeof(uint), (uint.MinValue, uint.MaxValue) },
		{ typeof(long), (long.MinValue, long.MaxValue) },
		{ typeof(ulong), (ulong.MinValue, ulong.MaxValue) },
	};

	public static void Generate()
	{
		var numberFormat = new NumberFormatInfo
		{
			NumberGroupSeparator = "_",
			NumberGroupSizes = new[] { 3 },
			NumberDecimalDigits = 0
		};

		foreach (var (type, (min, max)) in types)
		{
			var mid1Frac = Math.PI - 3;
			var mid2Frac = Math.E - 2;

			if (min < 0)
			{
				(mid1Frac, mid2Frac) = (1 - mid2Frac, 1 - mid1Frac);
			}

			var mid1 = Math.Ceiling((double)(max - min) * mid1Frac + (double)min);
			var mid2 = Math.Ceiling((double)(max - min) * mid2Frac + (double)min);

			Console.WriteLine($"public enum {type.Name}Enum : {type.FullName}");
			Console.WriteLine("{");
			Console.WriteLine($"\tMin = {min.ToString("n", numberFormat)},");
			Console.WriteLine($"\tMid1 = {mid1.ToString("n", numberFormat)},     // ((Max - Min) * {Math.Round(mid1Frac, 5)}) + Min");
			Console.WriteLine($"\tMid2 = {mid2.ToString("n", numberFormat)},     // ((Max - Min) * {Math.Round(mid2Frac, 5)}) + Min");
			Console.WriteLine($"\tMax = {max.ToString("n", numberFormat)},");
			Console.WriteLine("}\n");
		}
	}
}
