using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Data
{
    public interface IPeopleService
    {
        Task<IEnumerable<Person>> GetListAsync();

        Task<Person> GetByIdAsync(string id);

        Task<Person> CreateAsync(Person person);

        Task UpdateAsync(Person person);

        Task DeleteAsync(string id);
    }
}
