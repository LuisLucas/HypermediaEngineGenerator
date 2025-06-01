using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace HypermediaEngineGenerator.Helper
{
    internal static class INamedTypeSymbolExtensions
    {
        internal static string GetFullNamespace(this INamedTypeSymbol symbol)
        {
            var namespaces = new Stack<string>();
            INamespaceSymbol ns = symbol.ContainingNamespace;

            while (!ns.IsGlobalNamespace)
            {
                namespaces.Push(ns.Name);
                ns = ns.ContainingNamespace;
            }

            return string.Join(".", namespaces);
        }
    }
}
