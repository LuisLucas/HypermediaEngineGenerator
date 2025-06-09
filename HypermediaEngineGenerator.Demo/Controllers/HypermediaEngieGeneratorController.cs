using HypermediaEngineGenerator.Demo.Data;
using HypermediaEngineGenerator.Demo.Model;
using Microsoft.AspNetCore.Mvc;

namespace HypermediaEngineGenerator.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HypermediaEngieGeneratorController(HypermediaEngineGeneratorData data) : Controller
    {

        [HttpGet]
        public IEnumerable<HypermediaEngineGeneratorModel> Get()
        {
            var result = data.GetHypermediaGeneratorData();
            return result;
        }

        [HttpGet]
        public HypermediaEngineGeneratorModel Get(int id)
        {
            var result = data.GetHypermediaGeneratorData(id);
            return result;
        }

        [HttpPut]
        public HypermediaEngineGeneratorModel Put(int id, string name, string description)
        {
            var result = data.UpdateHypermediaGeneratorData(id, name, description);
            return result;
        }

        [HttpPost]
        public HypermediaEngineGeneratorModel Post(string name, string description)
        {
            var result = data.AddHypermediaGeneratorData(name, description);
            return result;
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            var result = data.DeleteHypermediaGeneratorData(id);
            return result;
        }
    }
}
