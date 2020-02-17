using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.BL.Parsers
{
    public interface IBreedsParser
    {
        Task<IEnumerable<string>> Run();
    }
}
