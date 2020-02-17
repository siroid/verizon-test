using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pets.BL.Services;

namespace Pets.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BreedsController : ControllerBase
    {
        private readonly ILogger<BreedsController> logger;
        private readonly IService service;

        public BreedsController(ILogger<BreedsController> logger, IService service)
        {
            this.logger = logger;
            this.service = service;
        }

        public async Task<IEnumerable<string>> GetAsync()
        {
            return await service.GetAll();
        }
    }
}
