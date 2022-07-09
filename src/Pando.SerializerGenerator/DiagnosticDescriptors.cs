using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator;

public static class DiagnosticDescriptors
{
	private static string DescriptorId(int number) => $"PANDO_{number:D4}";

	public static DiagnosticDescriptor TypeNotSealedDescriptor => new(
		id: DescriptorId(number: 1),
		title: "Type to serialize must be sealed",
		messageFormat: "Type {0} must have the sealed modifier.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
	
	public static DiagnosticDescriptor TypeHasNoAppropriateConstructor => new(
		id: DescriptorId(number: 2),
		title: "Type to serialize must have a constructor that takes all serialized properties",
		messageFormat: "Type {0} must have a constructor with a signature matching the order and type of the serialized properties: {1}({2})",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
}
