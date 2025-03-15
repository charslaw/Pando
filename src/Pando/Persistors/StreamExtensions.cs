using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Pando.Persistors;

///
internal static class StreamExtensions
{
	internal static void ThrowIfNotWritable(
		Stream stream,
		[CallerArgumentExpression(nameof(stream))] string? paramName = null
	)
	{
		if (!stream.CanWrite)
			throw new ArgumentException("Given stream must be writable.", paramName);
		if (!stream.CanSeek)
			throw new ArgumentException("Given stream must be seekable.", paramName);
	}

	internal static Stream OpenOrCreate(string path)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		if (!File.Exists(path))
		{
			var directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrWhiteSpace(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		return File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
	}
}
