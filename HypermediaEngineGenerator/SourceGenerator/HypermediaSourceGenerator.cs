using HypermediaEngineGenerator.Attributes;
using HypermediaEngineGenerator.AttributesGen.Attrubutes;
using HypermediaEngineGenerator.Helper;
using HypermediaEngineGenerator.HypermediaGenerator;
using HypermediaEngineGenerator.InterfacesGen;
using HypermediaEngineGenerator.ModelsGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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
                IEnumerable<INamedTypeSymbol> classes = source.Right.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>();

                foreach (INamedTypeSymbol symbol in classes)
                {
                    Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes = ExtractAttributesFromType(symbol);
                    string typeNamespace = symbol.GetFullNamespace();
                    string typeName = symbol.Name;
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

        public static Dictionary<string, List<ImmutableArray<TypedConstant>>> ExtractAttributesFromType(INamedTypeSymbol typeSymbol)
        {
            var actions = new Dictionary<string, List<ImmutableArray<TypedConstant>>>()
            {
                { HypermediaAttribute.FileName, new List<ImmutableArray<TypedConstant>>() },
                { HypermediaListAttribute.FileName, new List<ImmutableArray<TypedConstant>>() }
            };

            foreach (AttributeData attributeData in typeSymbol.GetAttributes())
            {
                string attrClassName = attributeData.AttributeClass?.Name;
                if (attrClassName is null || !attrClassName.StartsWith("Hypermedia"))
                {
                    continue;
                }

                if (actions.ContainsKey(attrClassName))
                {
                    actions[attrClassName].Add(attributeData.ConstructorArguments);
                }
            }
            return actions;
        }

        private void AddModelHateoasClassToSource(SourceProductionContext spc, Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes, string typeName, string typeNamespace)
        {
            List<string> actions = new List<string>();
            foreach (ImmutableArray<TypedConstant> action in actionsAttributes[HypermediaAttribute.FileName])
            {
                string method = action[0].Value as string;
                string rel = action[1].Value as string;
                string property = action[2].Value as string;
                string controller = action[3].Value as string;
                actions.Add($"new ControllerAction(\"{method}\", new {{ {property.ToLowerInvariant()} = item.{property} }}, \"{rel}\", \"{method}\", \"{controller}\")");
            }

            List<string> listActions = new List<string>();
            foreach (ImmutableArray<TypedConstant> action in actionsAttributes[HypermediaListAttribute.FileName])
            {
                string method = action[0].Value as string;
                string rel = action[1].Value as string;
                listActions.Add($"new ControllerAction(\"{method}\", new {{ }}, \"{rel}\", \"{method}\"));");
            }

            var sb = new StringBuilder();
            sb.AppendLine("using Hypermedia.Interfaces;");
            sb.AppendLine($"using {typeNamespace};");
            sb.AppendLine("namespace HyperMediaGenerator");
            sb.AppendLine("{");
            sb.AppendLine($"  public class {typeName}HyperMedia(IHypermediaGenerator hypermediaGen) : IHateoas<{typeName}>");
            sb.AppendLine("   {");
            sb.AppendLine("");
            sb.AppendLine("   }");
            sb.AppendLine("}");

            spc.AddSource($"{typeName}_Hateoas.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }.
    }
}
