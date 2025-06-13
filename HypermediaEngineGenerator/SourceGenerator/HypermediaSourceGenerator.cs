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

namespace HypermediaEngineGenerator.SourceGenerator
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

                List<string> registration = new List<string>();
                List<string> registrationUsings = new List<string>();
                foreach (INamedTypeSymbol symbol in classes)
                {
                    Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes = ExtractAttributesFromType(symbol);
                    AddModelHateoasClassToSource(spc, actionsAttributes, symbol);
                    AddHypermediaEngineGeneratorRegistration(registration, registrationUsings, symbol);
                }

                AddHypermediaEngineGeneratorRegistrationExtension(spc, registration, registrationUsings);
            });
        }

        private static bool IsCandidateClass(SyntaxNode node) =>
                            node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0 || node is RecordDeclarationSyntax r && r.AttributeLists.Count > 0;

        private static INamedTypeSymbol GetSemanticTarget(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax)
            {
                var classSyntax = (ClassDeclarationSyntax)context.Node;
                if (context.SemanticModel.GetDeclaredSymbol(classSyntax) is INamedTypeSymbol symbol)
                {
                    bool hasHateoasAttr = symbol.GetAttributes()
                                                    .Any(attr => attr.AttributeClass?.Name.Contains(HypermediaAttribute.FileName) == true
                                                              || attr.AttributeClass?.Name.Contains(HypermediaListAttribute.FileName) == true);

                    return hasHateoasAttr ? symbol : null;
                }

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

        private void AddModelHateoasClassToSource(SourceProductionContext spc, Dictionary<string, List<ImmutableArray<TypedConstant>>> actionsAttributes, INamedTypeSymbol symbol)
        {
            string typeNamespace = symbol.GetFullNamespace();
            string typeName = symbol.Name;

            List<string> actions = new List<string>();
            List<string> actionsWithFunc = new List<string>();
            foreach (ImmutableArray<TypedConstant> action in actionsAttributes[HypermediaAttribute.FileName])
            {
                string method = action[0].Value as string;
                string rel = action[1].Value as string;
                string property = action[2].Value as string;
                string controller = action[3].Value as string;
                actions.Add($"new ControllerAction(\"{method}\", new {{ {property.ToLowerInvariant()} = item.{property} }}, \"{rel}\", \"{method}\", \"{controller}\")");

                actionsWithFunc.Add($"new ControllerAction<{typeName}, object>(\"{method}\", new Tuple<string, Func<{typeName}, object>>(\"{property.ToLowerInvariant()}\", new Func<{typeName}, object>((item) => item.{property})), \"{rel}\", \"{method}\", \"{controller}\")");
            }

            List<string> listActions = new List<string>();
            List<string> paginatedListActions = new List<string>();
            var listSelfLink = "";
            foreach (ImmutableArray<TypedConstant> action in actionsAttributes[HypermediaListAttribute.FileName])
            {
                string method = action[0].Value as string;
                string rel = action[1].Value as string;
                listActions.Add($"new ControllerAction(\"{method}\", new {{ }}, \"{rel}\", \"{method}\")");
                
                if (rel == "self")
                {
                    listSelfLink = "new ControllerAction(\"{method}\", new { }, \"collection\", \"{method}\")";

                    paginatedListActions.Add($"new ControllerAction(\"{method}\", new {{ page }}, \"{rel}\", \"{method}\")");
                    paginatedListActions.Add($"new ControllerAction(\"{method}\", new {{ page = 1 }}, \"first\", \"first\")");
                    paginatedListActions.Add($"new ControllerAction(\"{method}\", new {{ page = page - 1 }}, \"previous\", \"previous\")");
                    paginatedListActions.Add($"new ControllerAction(\"{method}\", new {{ page = page + 1 }}, \"next\", \"next\")");
                    paginatedListActions.Add($"new ControllerAction(\"{method}\", new {{ page = (int)Math.Ceiling((double)totalNumberOfRecords / pageSize)}}, \"last\", \"last\")");

                }
            }

            var sourceText = $@"
using Hypermedia.Interfaces;
using Hypermedia.Models;
using Hypermedia.Generator;
using {typeNamespace};
    
namespace HypermediaGenerator
{{
    public class {typeName}HypermediaGenerator(IHypermediaFactory hypermediaGen) : IHypermediaGenerator<{typeName}>
    {{
        public Resource<{typeName}> GenerateLinks({typeName} item, HttpContext httpContext)
        {{
            var routeData = httpContext.GetRouteData();
            var controllerName = routeData.Values[""controller""]?.ToString();
            var itemActions = new List<ControllerAction>()
                                                {{
                                                    {string.Join(", ", actions)},{listSelfLink}
                                                }};
            return hypermediaGen.CreateResponse<{typeName}>(controllerName, item, itemActions);
        }}
            
        public Resource<{typeName}> GenerateLinks({typeName} item, Type controller)
        {{
            var itemActions = new List<ControllerAction>()
                                                {{
                                                    {string.Join(", ", actions)},{listSelfLink}
                                                }};
            return hypermediaGen.CreateResponse<{typeName}>(controller.Name.Replace(""Controller"", """"), item, itemActions);
        }}

        public CollectionResource<{typeName}> GenerateLinks(IEnumerable<{typeName}> items, HttpContext httpContext)
        {{
            var routeData = httpContext.GetRouteData();
            var controllerName = routeData.Values[""controller""]?.ToString();
            
            var itemActions = new List<ControllerAction<{typeName}, object>>()
                                                {{
                                                    {string.Join(", ", actionsWithFunc)}
                                                }};
            var listActions = new List<ControllerAction>()
                                                {{
                                                    {string.Join(", ", listActions)}
                                                }};

            return hypermediaGen.CreateCollectionResponse<{typeName}>(
                                                     controllerName,
                                                     items,
                                                     listActions,
                                                     itemActions);

        }}

        public CollectionResource<{typeName}> GenerateLinks(IEnumerable<{typeName}> items, Type controller)
        {{
            var itemActions = new List<ControllerAction<{typeName}, object>>()
                                                {{
                                                    {string.Join(", ", actionsWithFunc)}
                                                }};

            var listActions = new List<ControllerAction>()
                                                {{
                                                    {string.Join(", ", listActions)}
                                                }};

            return hypermediaGen.CreateCollectionResponse<{typeName}>(
                                                     controller.Name.Replace(""Controller"", """"),
                                                     items,
                                                     listActions,
                                                     itemActions);
        }}

        public PaginatedResource<{typeName}> GenerateLinks(IEnumerable<{typeName}> items, HttpContext httpContext, int page, int pageSize, int totalNumberOfRecords)
        {{
            var routeData = httpContext.GetRouteData();
            var controllerName = routeData.Values[""controller""]?.ToString();

            var itemActions = new List<ControllerAction<{typeName}, object>>()
                                                {{
                                                    {string.Join(", ", actionsWithFunc)}
                                                }};

            var listActions = new List<ControllerAction>()
                                            {{
                                                {string.Join(", ", listActions)}, {string.Join(", ", paginatedListActions)}
                                            }};

            return hypermediaGen.CreatePaginatedResponse<{typeName}>(
                                                controllerName, 
                                                items, 
                                                listActions, 
                                                itemActions);
        }}

        public PaginatedResource<{typeName}> GenerateLinks(IEnumerable<{typeName}> items, Type controller, int page, int pageSize, int totalNumberOfRecords)
        {{                            
            var itemActions = new List<ControllerAction<{typeName}, object>>()
                                                {{
                                                    {string.Join(", ", actionsWithFunc)}
                                                }};

            var listActions = new List<ControllerAction>()
                                            {{
                                                {string.Join(", ", listActions)}
                                            }};

            return hypermediaGen.CreatePaginatedResponse<{typeName}>(
                                                controller.Name.Replace(""Controller"", """"), 
                                                items, 
                                                listActions, 
                                                itemActions);
        }}
    }}
}}
";

            spc.AddSource($"{typeName}Hypermedia.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }

        private void AddHypermediaEngineGeneratorRegistrationExtension(SourceProductionContext spc, List<string> registration, List<string> registrationUsings)
        {
            var usings = new StringBuilder();
            foreach (var nameSpace in registrationUsings) 
            {
                usings.AppendLine("using " + nameSpace + ";");
            }
            

            var sourceText = $@"
using Microsoft.Extensions.DependencyInjection;
using Hypermedia.Interfaces;
using Hypermedia.Generator;
{usings}

namespace HypermediaGenerator
{{
    public static class HypermediaGeneratorRegistration
    {{
        public static IServiceCollection AddHypermediaGenerator(this IServiceCollection services)
        {{
            services.AddHttpContextAccessor();
            services.AddTransient<IHypermediaFactory, HypermediaFactory>();
            {string.Join("\n", registration)}
            return services;
        }}
    }}
}}
";
            spc.AddSource($"HypermediaEngineGeneratorRegistration.g.cs.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }

        internal static void AddHypermediaEngineGeneratorRegistration(List<string> registration, List<string> registrationUsings, INamedTypeSymbol symbol)
        {
            string typeNamespace = symbol.GetFullNamespace();
            string typeName = symbol.Name;
            registrationUsings.Add(typeNamespace);
            registration.Add($"services.AddScoped<IHypermediaGenerator<{typeName}>, {typeName}HypermediaGenerator>();");
        }
    }
}
