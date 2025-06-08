using HypermediaEngineGenerator.Demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace HypermediaEngineGenerator.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HypermediaEngieGeneratorController : Controller
    {

        [HttpGet]
        public IEnumerable<HypermediaEngineGeneratorModel> Get()
        {
            var list =  Enumerable.Range(1, 5).Select(index => new HypermediaEngineGeneratorModel
            {
                Id = index,
                Name = index.ToString(),
                Description = index.ToString() + "Description",
            })
            .ToArray();

            return list;
        }
    }
}
