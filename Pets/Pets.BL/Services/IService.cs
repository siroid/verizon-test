using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.BL.Services
{
    public interface IService
    {
        Task<IEnumerable<string>> GetAll();
    }
}
