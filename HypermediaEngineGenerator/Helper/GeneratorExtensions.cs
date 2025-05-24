using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HypermediaEngineGenerator.Helper
{
    internal static class GeneratorExtensions
    {
        private static readonly string s_fileNameConvetion = ".g.cs";

        internal static IncrementalGeneratorInitializationContext AddFileToSource(this IncrementalGeneratorInitializationContext context, string classText, string fileName)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                                                        string.Concat(fileName, s_fileNameConvetion),
                                                        GetSourceText(classText)
                                                        ));
            return context;
        }

        private static SourceText GetSourceText(string classText) => SourceText.From(classText, Encoding.UTF8);
    }
}
