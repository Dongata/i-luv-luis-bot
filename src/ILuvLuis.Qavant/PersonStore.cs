using ILuvLuis.Models;
using ILuvLuis.Models.Stores;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ILuvLuis.Qavant
{
    public class PersonStore : IPersonStore
    {
        #region Fields

        private readonly HttpClient _httpClient;

        private string token;

        #endregion

        #region Constructor

        public PersonStore(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        #region Properties

        public string Token { set => token = value; }

        #endregion

        #region Public methods

        public async Task<IEnumerable<Person>> Search(string searchString)
        {
            _httpClient.DefaultRequestHeaders.Add("x-token", token);

            var res = await _httpClient.GetAsync($"users/search/{searchString}/page/1");

            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<Person>>(
                await res.Content.ReadAsStringAsync());
        }

        #endregion
    }
}
