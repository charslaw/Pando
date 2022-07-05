using System.Diagnostics;

namespace Pando.SerializerGenerator.Utils;

public static class DebugUtils
{
	/// Simple utility to stop the debugger at the location of this call. If the debugger isn't already running, launch it.
	/// No-ops if the DEBUG flag is false
	public static void Break()
	{
#if DEBUG
		if (Debugger.IsAttached)
		{
			Debugger.Break();
		}
		else
		{
			Debugger.Launch();
		}
#endif
	}
}
