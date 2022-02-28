using System.IO;
using System.Runtime.CompilerServices;
using DiffEngine;
using VerifyTests;

namespace SerializerGeneratorUnitTests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifierSettings.DerivePathInfo((sourceFile, _, type, method) => new PathInfo(
				directory: Path.Combine(Path.GetDirectoryName(sourceFile)!, "Verified"),
				typeName: type.Name,
				methodName: method.Name
			)
		);

		VerifySourceGenerators.Enable();

		DiffRunner.Disabled = true;
	}
}
