using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.AttributesGen.Attrubutes
{
    internal static class HypermediaEnableAttribute
    {
        internal const string FileName = "HypermediaAttribute";
        private const string Class = @"
namespace Hypermedia.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class HypermediaEnableAttribute : System.Attribute
    {
            public HypermediaEnableAttribute(Type dto)
            {
                Dto = dto;
            }

            public Type Dto { get; }
    }
}";

        internal static IncrementalGeneratorInitializationContext AddAttributeToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
