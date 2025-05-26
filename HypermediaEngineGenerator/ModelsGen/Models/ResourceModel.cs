using HypermediaEngineGenerator.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HypermediaEngineGenerator.ModelsGen.Models
{
    internal static class ResourceModel
    {
        private const string FileName = "Resource";
        public const string Class = @"
namespace Hypermedia.Models;

public class Resource<T>
{
    public T Item { get; set; }

    public List<Link> Links { get; set; }
}";

        internal static IncrementalGeneratorInitializationContext AddToSource(this IncrementalGeneratorInitializationContext context)
        {
            return context.AddFileToSource(Class, FileName);
        }
    }
}
