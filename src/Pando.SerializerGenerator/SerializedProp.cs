using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator;

public class SerializedProp
{
	public INamedTypeSymbol Type { get; }
	public string Name { get; }
	public bool IsPrimitive { get; }
	
	public SerializedProp(INamedTypeSymbol type, string name, bool isPrimitive)
	{
		Type = type;
		Name = name;
		IsPrimitive = isPrimitive;
	}

}
