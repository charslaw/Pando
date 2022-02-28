using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator.Utils;

internal class ParameterArrayEqualityComparer : IEqualityComparer<ImmutableArray<IParameterSymbol>>
{
	public bool Equals(ImmutableArray<IParameterSymbol> x, ImmutableArray<IParameterSymbol> y)
	{
		if (x.Length != y.Length) return false;

		for (int i = 0; i < x.Length; i++)
		{
			if (!SymbolEqualityComparer.IncludeNullability.Equals(x[i].Type, y[i].Type)) return false;
		}

		return true;
	}

	public int GetHashCode(ImmutableArray<IParameterSymbol> obj)
	{
		unchecked
		{
			int hash = 17;
			foreach (var element in obj)
			{
				hash = hash * 31 + SymbolEqualityComparer.IncludeNullability.GetHashCode(element);
			}

			return hash;
		}
	}
}
