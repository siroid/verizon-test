using System.Collections.Generic;

namespace Pets.BL.DTO
{
    public class DogBreedsResponse
    {
        public IDictionary<string, IEnumerable<string>> message { get; set; }
        public string status { get; set; }
    }
}
