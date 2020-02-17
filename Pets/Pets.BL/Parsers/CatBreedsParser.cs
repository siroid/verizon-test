using Jil;
using Pets.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pets.BL.Parsers
{
    public class CatBreedsParser : IBreedsParser
    {
        private readonly string AuthKeyName;
        private readonly string AuthKeyValue;
        private readonly string ApiUrl;
        private readonly HttpClient HttpClient;

        public CatBreedsParser(HttpClient httpClient, string authKeyName, string authKeyValue, string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(authKeyName))
            {
                throw new ArgumentException(ExceptionMessages.EmptyArgument, "authKeyName");
            }
            if (string.IsNullOrWhiteSpace(authKeyValue))
            {
                throw new ArgumentException(ExceptionMessages.EmptyArgument, "authKeyValue");
            }
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ArgumentException(ExceptionMessages.EmptyArgument, "apiUrl");
            }

            this.AuthKeyName = authKeyName;
            this.AuthKeyValue = authKeyValue;
            this.ApiUrl = apiUrl;
            this.HttpClient = httpClient;
        }

        public async Task<IEnumerable<string>> Run()
        {
            try
            {
                HttpClient.DefaultRequestHeaders.Clear();
                HttpClient.DefaultRequestHeaders.Add(this.AuthKeyName, this.AuthKeyValue);
                var content = await HttpClient.GetStringAsync(this.ApiUrl);
                var deserializedArray = JSON.Deserialize<IEnumerable<CatBreedsResponse>>(content);
                return deserializedArray.Select(x => x.name);
            }
            catch(Exception ex)
            {
                throw new ParserErrorException(ExceptionMessages.UnexpectedParsingError, ex);
            }
        }
    }
}
