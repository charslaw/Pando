using Microsoft.CodeAnalysis;

namespace Pando.SerializerGenerator;

public static class DiagnosticDescriptors
{
	private static string DescriptorId(int number) => $"Pando_SG_{number:D4}";

	public static DiagnosticDescriptor TypeNotSealedDescriptor => new(
		id: DescriptorId(number: 1),
		title: "Type to serialize must be sealed.",
		messageFormat: "Type {0} must have the sealed type modifier.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public static DiagnosticDescriptor TypeDoesNotHaveDeconstructorDescriptor => new(
		id: DescriptorId(number: 2),
		title: "Type to serialize must define a Deconstruct method.",
		messageFormat: "Type {0} must define a Deconstruct method following the appropriate conventions.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: "https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types"
	);

	public static DiagnosticDescriptor TypeDoesNotHaveMatchingDeconstructConstructorPairDescriptor => new(
		id: DescriptorId(number: 3),
		title: "Type to serialize must define a Deconstruct method and a constructor with matching signatures.",
		messageFormat: "Type {0} must define a Deconstruct method whose parameters match those of a single constructor.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public static DiagnosticDescriptor TypeHasMoreThanOneMatchingDeconstructConstructorPairDescriptor => new(
		id: DescriptorId(number: 4),
		title: "Type to serialize must not have multiple valid Deconstruct/Constructor pairs.",
		messageFormat: "Type {0} must not define more than one Deconstruct method whose parameters match those of any constructor.",
		category: "Pando.SerializerGenerator.CandidateTypeCriteria",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
}
