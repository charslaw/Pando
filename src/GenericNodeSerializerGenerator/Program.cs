using System.CodeDom.Compiler;
using System.CommandLine;

var filenameArg = new Argument<string>("filename", "The filename to write the generated code to.");
var maxChildrenArg = new Option<int>("--max-children", () => 16, "The maximum number of children nodes to generate serializers for.");
var command = new RootCommand("Generates GenericNodeSerializers for varying numbers of children nodes.")
{
	filenameArg,
	maxChildrenArg,
};

command.SetHandler((filename, maxChildren) =>
{
	var absPath = Path.GetFullPath(filename);
	Console.WriteLine($"Writing to {absPath}...");

	var dir = Path.GetDirectoryName(absPath);
	if (dir is not null) Directory.CreateDirectory(dir);

	using var fileWriter = new StreamWriter(absPath, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create });
	var writer = new IndentedTextWriter(fileWriter, "\t");

	writer.WriteLine("using System;");
	writer.WriteLine("using System.Buffers.Binary;");
	writer.WriteLine("using Pando.DataSources;");
	writer.WriteLine("using Pando.Serialization.Utils;");
	writer.WriteLineNoTabs(null);
	writer.WriteLine("namespace Pando.Serialization.Generic;");
	writer.WriteLineNoTabs(null);

	for (var nChildren = 1; nChildren <= maxChildren; nChildren++)
	{
		var children = nChildren;
		var range = Enumerable.Range(1, children).ToArray();
		var typeParams = string.Join(", ", range.Select(i => $"T{i}"));

		var className = $"GenericNodeSerializer<TNode, {typeParams}>";
		Console.WriteLine($"  Generating {className}...");

		// comment
		writer.WriteLine(
			$$"""
			  /// <summary>
			  /// A generic node serializer for a node type that implements <see cref="IGenericSerializable{TSelf, {{typeParams}}}"/>.
			  /// </summary>
			  """
		);

		// class declaration
		writer.WriteLine($"public class {className} : IPandoSerializer<TNode>");
		writeIndented(writer, () => writer.WriteLine($"where TNode : IGenericSerializable<TNode, {typeParams}>"));

		writeBody(writer,
			() =>
			{
				// SerializedSize impl
				writer.WriteLine("public int SerializedSize => sizeof(ulong);");
				writer.WriteLineNoTabs(null);

				// child serializers
				for (int i = 1; i <= children; i++) writer.WriteLine($"private readonly IPandoSerializer<T{i}> {serializerField(i)};");
				writer.WriteLineNoTabs(null);

				// offsets
				writer.WriteLine("private const int _t0EndOffset = 0;");
				for (int i = 1; i <= children; i++) writer.WriteLine($"private readonly int {offsetVar(i)};");
				writer.WriteLineNoTabs(null);

				// constructor
				writer.WriteLine("public GenericNodeSerializer(");

				// constructor params
				writeIndented(writer, () =>
				{
					for (int i = 1; i < children; i++) writer.WriteLine($"IPandoSerializer<T{i}> {serializerVar(i)},");
					writer.WriteLine($"IPandoSerializer<T{children}> {serializerVar(children)}");
				});
				writer.WriteLine(")");

				// constructor params
				writeBody(writer, () =>
					{
						// assign child serializers
						for (int i = 1; i <= children; i++) writer.WriteLine($"{serializerField(i)} = {serializerVar(i)};");
						writer.WriteLineNoTabs(null);

						// assign offsets
						// writer.WriteLine($"{offsetVar(1)} = {serializerField(1)}.SerializedSize;");
						for (int i = 1; i <= children; i++) writer.WriteLine($"{offsetVar(i)} = {offsetVar(i - 1)} + {serializerField(i)}.SerializedSize;");
					}
				);

				// Serialize impl
				writeMethod(writer, "public void Serialize(TNode value, Span<byte> buffer, INodeDataSink dataSink)",
					() =>
					{
						writer.WriteLine($"Span<byte> childrenBuffer = stackalloc byte[{offsetVar(children)}];");
						writer.WriteLineNoTabs(null);
						writer.WriteLine($"value.Deconstruct({string.Join(", ", range.Select(i => "out var t" + i))});");

						// writer.WriteLine($"{serializerField(1)}.Serialize(t1, childrenBuffer[..{offsetVar(1)}], dataSink);");
						for (int i = 1; i <= children; i++) writer.WriteLine($"{serializerField(i)}.Serialize(t{i}, childrenBuffer[{offsetVar(i - 1)}..{offsetVar(i)}], dataSink);");
						writer.WriteLineNoTabs(null);

						writer.WriteLine("var nodeHash = dataSink.AddNode(childrenBuffer);");
						writer.WriteLine("BinaryPrimitives.WriteUInt64LittleEndian(buffer, nodeHash);");
					}
				);


				// Deserialize impl
				writeMethod(writer, "public TNode Deserialize(ReadOnlySpan<byte> buffer, INodeDataSource dataSource)",
					() =>
					{
						writer.WriteLine($"Span<byte> childrenBuffer = stackalloc byte[{offsetVar(children)}];");
						writer.WriteLine("dataSource.CopyNodeBytesTo(buffer, childrenBuffer);");
						writer.WriteLineNoTabs(null);

						// writer.WriteLine($"var t1 = {serializerField(1)}.Deserialize(childrenBuffer[..{offsetVar(1)}], dataSource);");
						for (int i = 1; i <= children; i++) writer.WriteLine($"var t{i} = {serializerField(i)}.Deserialize(childrenBuffer[{offsetVar(i - 1)}..{offsetVar(i)}], dataSource);");
						writer.WriteLineNoTabs(null);

						writer.WriteLine($"return TNode.Construct({string.Join(", ", range.Select(i => $"t{i}"))});");
					}
				);

				// Merge impl
				writeMethod(writer,
					"public void Merge(Span<byte> baseBuffer, ReadOnlySpan<byte> targetBuffer, ReadOnlySpan<byte> sourceBuffer, IDataSource dataSource)",
					() =>
					{
						writer.WriteLine("if (MergeUtils.MergeIfUnchanged(baseBuffer, targetBuffer, sourceBuffer)) return;");
						writer.WriteLine("if (MergeUtils.MergeIfUnchanged(baseBuffer, sourceBuffer, targetBuffer)) return;");
						writer.WriteLineNoTabs(null);

						writer.WriteLine("// allocate a buffer to contain the children data of base, target, and source");
						writer.WriteLine($"Span<byte> childrenBuffer = stackalloc byte[{offsetVar(children)} * 3];");
						writer.WriteLineNoTabs(null);
						writer.WriteLine($"var baseChildrenBuffer = childrenBuffer[..{offsetVar(children)}];");
						writer.WriteLine("dataSource.CopyNodeBytesTo(baseBuffer, baseChildrenBuffer);");
						writer.WriteLine($"var targetChildrenBuffer = childrenBuffer[{offsetVar(children)}..({offsetVar(children)} * 2)];");
						writer.WriteLine("dataSource.CopyNodeBytesTo(targetBuffer, targetChildrenBuffer);");
						writer.WriteLine($"var sourceChildrenBuffer = childrenBuffer[({offsetVar(children)} * 2)..({offsetVar(children)} * 3)];");
						writer.WriteLine("dataSource.CopyNodeBytesTo(sourceBuffer, sourceChildrenBuffer);");
						writer.WriteLineNoTabs(null);

						writer.WriteLine("// merge each child");
						for (int i = 1; i <= children; i++)
						{
							writer.WriteLine(
								$"{serializerField(i)}.Merge(" +
								$"baseChildrenBuffer[{offsetVar(i - 1)}..{offsetVar(i)}], " +
								$"targetChildrenBuffer[{offsetVar(i - 1)}..{offsetVar(i)}], " +
								$"sourceChildrenBuffer[{offsetVar(i - 1)}..{offsetVar(i)}], " +
								$"dataSource);");
						}
						writer.WriteLineNoTabs(null);

						writer.WriteLine("dataSource.AddNode(baseChildrenBuffer, baseBuffer);");
					}
				);
			}
		);
	}

	Console.WriteLine("Done!");

}, filenameArg, maxChildrenArg);

command.Invoke(args);
return;

string serializerVar(int i) => $"t{i}Serializer";
string serializerField(int i) => "_" + serializerVar(i);
string offsetVar(int i) => $"_t{i}EndOffset";

void writeMethod(IndentedTextWriter writer, string signature, Action body)
{
	writer.WriteLine(signature);
	writeBody(writer, body);
}

void writeBody(IndentedTextWriter writer, Action body)
{
	writer.WriteLine("{");
	writeIndented(writer, body);
	writer.WriteLine("}");
	writer.WriteLineNoTabs(null);
}

void writeIndented(IndentedTextWriter writer, Action body)
{
	writer.Indent++;
	body();
	writer.Indent--;
}
