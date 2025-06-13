using HypermediaEngineGenerator.Demo.Data;
using HypermediaEngineGenerator.Demo.Model;
using Microsoft.AspNetCore.Mvc;
using Hypermedia.Interfaces;
using Hypermedia.Models;

namespace HypermediaEngineGenerator.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HypermediaEngineGeneratorController(HypermediaEngineGeneratorData data, IHypermediaGenerator<HypermediaEngineGeneratorModel> modelHyperMediaGenerator) : Controller
    {

        [HttpGet]
        public CollectionResource<HypermediaEngineGeneratorModel> Get()
        {
            var items = data.GetHypermediaGeneratorData();

            var collectionResources = modelHyperMediaGenerator.GenerateLinks(items, HttpContext);
            return collectionResources;
        }

        [HttpGet("page/{currentPage}")]
        public PaginatedResource<HypermediaEngineGeneratorModel> Get(int currentPage, int pageSize = 2)
        {
            var items = data.GetHypermediaGeneratorPaginatedData(currentPage, pageSize);

            var collectionResources = modelHyperMediaGenerator.GenerateLinks(items.Item2, HttpContext, currentPage, pageSize, items.Item1);
            return collectionResources;
        }

        [HttpGet("/{id}")]
        public Resource<HypermediaEngineGeneratorModel> Get(int id)
        {
            var item = data.GetHypermediaGeneratorData(id);

            var resource = modelHyperMediaGenerator.GenerateLinks(item, HttpContext);
            return resource;
        }

        [HttpPut]
        public Resource<HypermediaEngineGeneratorModel> Put(int id, string name, string description)
        {
            var item = data.UpdateHypermediaGeneratorData(id, name, description);

            var resource = modelHyperMediaGenerator.GenerateLinks(item, HttpContext);
            return resource;
        }

        [HttpPost]
        public Resource<HypermediaEngineGeneratorModel> Post(string name, string description)
        {
            var item = data.AddHypermediaGeneratorData(name, description);

            var resource = modelHyperMediaGenerator.GenerateLinks(item, HttpContext);
            return resource;
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            var result = data.DeleteHypermediaGeneratorData(id);
            return result;
        }
    }
}
