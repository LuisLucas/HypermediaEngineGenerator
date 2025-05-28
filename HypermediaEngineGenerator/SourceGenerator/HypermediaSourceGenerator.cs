using HypermediaEngineGenerator.Attributes;
using HypermediaEngineGenerator.AttributesGen.Attrubutes;
using HypermediaEngineGenerator.HypermediaGenerator;
using HypermediaEngineGenerator.InterfacesGen;
using HypermediaEngineGenerator.ModelsGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HypermediaEngineGenerator.Generator
{
    [Generator]
    internal class HypermediaSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context
                .AddAttributes()
                .AddInterfaces()
                .AddModels();

            HypermediaFactory.AddToSource(context);
            LinkGenerator.AddToSource(context);

            IncrementalValuesProvider<INamedTypeSymbol> classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (s, _) => IsCandidateClass(s),
                    transform: (ctx, _) => GetSemanticTarget(ctx))
                .Where(symbol => symbol != null);

            IncrementalValueProvider<(Compilation Left, ImmutableArray<INamedTypeSymbol> Right)> compilationAndClasses =
            context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
            {
                Compilation compilation = source.Left;
                IEnumerable<INamedTypeSymbol> classes = source.Right.Distinct();

                foreach (INamedTypeSymbol symbol in classes)
                {
                }
            });
        }

        private static bool IsCandidateClass(SyntaxNode node) =>
                            (node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0) || (node is RecordDeclarationSyntax r && r.AttributeLists.Count > 0);

        private static INamedTypeSymbol GetSemanticTarget(GeneratorSyntaxContext context)
        {
            if (context.Node is RecordDeclarationSyntax classSyntax &&
                (context.SemanticModel.GetDeclaredSymbol(classSyntax) is INamedTypeSymbol symbol))
            {
                bool hasHateoasAttr = symbol.GetAttributes()
                    .Any(attr => attr.AttributeClass?.Name.Contains(HypermediaAttribute.FileName) == true ||
                                    attr.AttributeClass?.Name.Contains(HypermediaListAttribute.FileName) == true);

                return hasHateoasAttr ? symbol : null;
            }
            return null;
        }
    }
}
