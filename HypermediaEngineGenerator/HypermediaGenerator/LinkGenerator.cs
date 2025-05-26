using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.HypermediaGenerator
{
    internal static class LinkGenerator
    {
        private const string FileName = "GenerateLinks";
        private const string Class = @"
using Hypermedia.Models;

namespace Hypermedia.Generator;

public static class GenerateLinks
{
    public static List<Link> BuildLinks(LinkGenerator linkGenerator, string controller, IEnumerable<ControllerAction> actions, string scheme, HostString host)
    {
        var links = new List<Link>();
        foreach (ControllerAction item in actions)
        {
            string controllerName = controller;
            if(!String.IsNullOrEmpty(item.controller))
            {
                controllerName = item.controller.Replace(""Controller"", """");
            }
            string? absoluteUri = linkGenerator.GetUriByAction(
                                                action: item.action,
                                                controller: controllerName,
                                                values: item.values,
                                                scheme,
                                                host);

            if (!string.IsNullOrEmpty(absoluteUri))
            {
                links.Add(new Link(absoluteUri, item.rel, item.method));
            }
        }
        return links;
    }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
