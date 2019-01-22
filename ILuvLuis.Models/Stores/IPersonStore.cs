using ILuvLuis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ILuvLuis.Models.Stores
{
    public interface IPersonStore
    {
        string Token { set; }

        Task<IEnumerable<Person>> Search(string searchString);
    }
}
