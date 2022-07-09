using System;
using System.CodeDom.Compiler;

namespace Pando.SerializerGenerator.Utils;

public static class IndentedTextWriterExtensions
{
	public static void DoIndent(this IndentedTextWriter writer, Action body)
	{
		try
		{
			writer.Indent += 1;
			body();
		}
		finally
		{
			writer.Indent -= 1;
		}
	}

	public static void BodyIndent(this IndentedTextWriter writer, Action body)
	{
		writer.WriteLine("{");
		writer.DoIndent(body);
		writer.WriteLine("}");
	}

	public static void BlankLine(this IndentedTextWriter writer) => writer.InnerWriter.WriteLine();
}
