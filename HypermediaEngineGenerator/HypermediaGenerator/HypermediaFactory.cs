using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;

namespace HypermediaEngineGenerator.HypermediaGenerator
{
    internal static class HypermediaFactory
    {
        private const string FileName = "HypermediaFactory";
        private const string Class = @"
using Hypermedia.Models;

namespace Hypermedia.Generator;

public interface IHypermediaFactory
{
    Resource<T> CreateResponse<T>(
                            string controller,
                            T item,
                            List<ControllerAction> itemActions);

    PaginatedResource<T> CreatePaginatedResponse<T>(
                                                    string controller,
                                                    IEnumerable<T> items,
                                                    List<ControllerAction> listActions,
                                                    List<ControllerAction<T, object>> itemActions);
}

public class HypermediaFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : IHateoasFactory
{
    public Resource<T> CreateResponse<T>(
                                string controller,
                                T item,
                                List<ControllerAction> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        (string? scheme, HostString? host) = ExtractSchemeAndHost();
        var resource = new Resource<T>
        {
            Item = item,
            Links = BuildLinks(linkGenerator, controller, scheme, host.Value, itemActions)
        };
        return resource;
    }

    public PaginatedResource<T> CreatePaginatedResponse<T>(string controller,
        IEnumerable<T> items,
        List<ControllerAction> listActions,
        List<ControllerAction<T, object>> itemActions)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        (string? scheme, HostString? host) = ExtractSchemeAndHost();

        var paginatedResponse = new PaginatedResource<T>
        {
            Items = AddLinkstoItems(linkGenerator, controller, scheme, host.Value, items, itemActions),
            Links = BuildLinks(linkGenerator, controller, scheme, host.Value, listActions)
        };
        return paginatedResponse;
    }

    private static List<Resource<T>> AddLinkstoItems<T, R>(
        LinkGenerator linkGenerator,
        string controller,
        string scheme,
        HostString host,
        IEnumerable<T> items,
        List<ControllerAction<T, R>> itemActions)
    {
        var resourceItems = new List<Resource<T>>();
        foreach (T? item in items)
        {
            var resource = AddLinksToItem(linkGenerator, controller, scheme, host, item, itemActions);
            resourceItems.Add(resource);
        }
        return resourceItems;
    }

    private static Resource<T> AddLinksToItem<T, R>(LinkGenerator linkGenerator,
        string controller,
        string scheme,
        HostString host,
        T item,
        List<ControllerAction<T, R>> itemActions)
    {
        var itemControllerActions = new List<ControllerAction>();
        foreach (ControllerAction<T, R> c in itemActions)
        {
            var routeValueDic = new RouteValueDictionary
            {
                { c.values.Item1, c.values.Item2.Invoke(item) }
            };
            itemControllerActions.Add(new ControllerAction(c.action, routeValueDic, c.rel, c.method, c.controller));
        }

        var resource = new Resource<T>
        {
            Item = item,
            Links = BuildLinks(linkGenerator, controller, scheme, host, itemControllerActions)
        };
        return resource;
    }

    private static List<Link> BuildLinks(LinkGenerator linkGenerator, string controller, string scheme, HostString host, List<ControllerAction> listActions)
    {
        return GenerateLinks.BuildLinks(
                                    linkGenerator,
                                    controller,
                                    listActions,
                                    scheme,
                                    host);
    }

    private (string?, HostString?) ExtractSchemeAndHost()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        return (request?.Scheme, request?.Host);
    }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
