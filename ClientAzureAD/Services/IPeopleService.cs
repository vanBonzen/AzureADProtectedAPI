using ClientAzureAD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientAzureAD.Services
{
    public interface IPeopleService
    {
        Task<IEnumerable<Person>> GetAsync();

        Task<Person> GetAsync(string guid);

        Task DeleteAsync(string guid);

        Task<Person> AddAsync(Person person);

        Task<Person> EditAsync(Person person);
    }
}
