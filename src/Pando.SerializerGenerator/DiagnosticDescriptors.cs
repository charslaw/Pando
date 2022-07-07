using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator;

public static class DiagnosticDescriptors
{
	private static string DescriptorId(int number) => $"PANDO_SG_{number:D4}";

	public static DiagnosticDescriptor TypeNotSealedDescriptor => new(
		id: DescriptorId(number: 1),
		title: "Type to serialize must be sealed",
		messageFormat: "Type {0} must have the sealed modifier.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
	
	public static DiagnosticDescriptor TypeHasNoParameterlessCtorDescriptor => new(
		id: DescriptorId(number: 2),
		title: "Type to serialize must have a parameterless constructor",
		messageFormat: "Type {0} must have a parameterless constructor in order to be constructed by the generated serializer.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
}
