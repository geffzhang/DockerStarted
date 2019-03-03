using Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class PeopleController : Controller
    {
        private IMongoDatabase _db;
        private IMongoCollection<Person> _people;

        public PeopleController(IMongoDatabase database)
        {
            _db = database;
            _people = database.GetCollection<Person>("People");
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            return await _people.AsQueryable().ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _people.AsQueryable()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (result == null)
                return NotFound();

            return new ObjectResult(result);
        }

        // POST api/values
        [HttpPost]
        public async Task<Person> Post([FromBody]Person person)
        {
            person.Id = ObjectId.GenerateNewId().ToString();
            await _people.InsertOneAsync(person);

            return person;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<Person> Put(string id, [FromBody]Person person)
        {
            await _people.ReplaceOneAsync(x => x.Id == id, person);

            return person;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _people.DeleteOneAsync(x => x.Id == id);
        }
    }
}
