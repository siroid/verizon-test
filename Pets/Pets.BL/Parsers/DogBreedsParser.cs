using Jil;
using Pets.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pets.BL.Parsers
{
    public class DogBreedsParser : IBreedsParser
    {
        private readonly string ApiUrl;
        private readonly HttpClient HttpClient;

        public DogBreedsParser(HttpClient httpClient, string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ArgumentException(ExceptionMessages.EmptyArgument, "apiUrl");
            }

            this.ApiUrl = apiUrl;
            this.HttpClient = httpClient;
        }

        public async Task<IEnumerable<string>> Run()
        {
            try
            {
                HttpClient.DefaultRequestHeaders.Clear();
                var content = await HttpClient.GetStringAsync(ApiUrl);
                var result = JSON.Deserialize<DogBreedsResponse>(content);
                return result.message.Select(x => x.Key);            }
            catch (Exception ex)
            {
                throw new ParserErrorException(ExceptionMessages.UnexpectedParsingError, ex);
            }
        }
    }
}
