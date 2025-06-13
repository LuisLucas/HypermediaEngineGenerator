using HypermediaEngineGenerator.Demo.Model;

namespace HypermediaEngineGenerator.Demo.Data
{
    public class HypermediaEngineGeneratorData
    {
        private int currentIdx = 10;
        private Dictionary<int, HypermediaEngineGeneratorModel> data = new Dictionary<int, HypermediaEngineGeneratorModel>()
        {
            {1, new HypermediaEngineGeneratorModel() { Id = 1, Name = "Name1", Description = "Description1" }},
            {2, new HypermediaEngineGeneratorModel() { Id = 2, Name = "Name2", Description = "Description2" }},
            {3, new HypermediaEngineGeneratorModel() { Id = 3, Name = "Name3", Description = "Description3" }},
            {4, new HypermediaEngineGeneratorModel() { Id = 4, Name = "Name4", Description = "Description4" }},
            {5, new HypermediaEngineGeneratorModel() { Id = 5, Name = "Name5", Description = "Description5" }},
            {6, new HypermediaEngineGeneratorModel() { Id = 6, Name = "Name6", Description = "Description6" }},
            {7, new HypermediaEngineGeneratorModel() { Id = 7, Name = "Name7", Description = "Description7" }},
            {8, new HypermediaEngineGeneratorModel() { Id = 8, Name = "Name8", Description = "Description8" }},
            {9, new HypermediaEngineGeneratorModel() { Id = 9, Name = "Name9", Description = "Description9" }},
        };

        public List<HypermediaEngineGeneratorModel> GetHypermediaGeneratorData()
        {
            return data.Values.ToList();
        }

        internal (int, IEnumerable<HypermediaEngineGeneratorModel>) GetHypermediaGeneratorPaginatedData(int currentPage, int pageSize)
        {
            return (data.Count, data.Values.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
        }

        public HypermediaEngineGeneratorModel GetHypermediaGeneratorData(int idx)
        {
            return data[idx];
        }

        public HypermediaEngineGeneratorModel UpdateHypermediaGeneratorData(int idx, string name, string description)
        {
            data[idx].Name = name;
            data[idx].Description = description;
            return data[idx];
        }

        public HypermediaEngineGeneratorModel AddHypermediaGeneratorData(string name, string description)
        {
            var hypermediaModel = new HypermediaEngineGeneratorModel()
            {
                Id = currentIdx,
                Name = name,
                Description = description
            };
            data.Add(currentIdx, hypermediaModel);
            currentIdx++;
            return hypermediaModel;
        }

        public bool DeleteHypermediaGeneratorData(int idx)
        {
            data.Remove(idx);
            return true;
        }
    }
}
