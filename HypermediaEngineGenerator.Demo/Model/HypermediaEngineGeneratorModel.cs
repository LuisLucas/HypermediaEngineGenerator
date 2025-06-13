using Hypermedia.Attributes;

namespace HypermediaEngineGenerator.Demo.Model
{
    [Hypermedia("GET", "self", "Id")]
    [Hypermedia("PUT", "edit", "Id")]
    [Hypermedia("DELETE", "delete", "Id")]
    //[Hypermedia("GET", "related", "Id", "stocks")]
    [HypermediaList("GET", "self")] // TODO: identifie the parameter name for pagination if we are using the option with pagination on the generated class.
    [HypermediaList("POST", "create")]
    public class HypermediaEngineGeneratorModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
