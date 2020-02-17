using Pets.BL.Parsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.BL.Services
{
    public class BreedsService : IService
    {
        private readonly IEnumerable<IBreedsParser> Parsers;

        public BreedsService(IEnumerable<IBreedsParser> parsers)
        {
            this.Parsers = parsers;
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            var result = new List<string>();
            foreach(var x in Parsers)
            {
                result.AddRange(await x.Run());
            }

            return result;
        }
    }
}
