using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore;
using testcoreapi.Models;

namespace testcoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Person

     

        public async Task<long> Save(string json)
        {

            string Key = string.Empty;
            string Value = string.Empty;

            Dictionary<string, string> ValueList = new Dictionary<string, string>();

            string[] multiArray = json.Split(new Char[] { ' ', ',', '.', '-', '\n', '\t' });
            for (int i = 0; i < multiArray.Length; i = i + 3)
            {
                try
                {
                    Key = multiArray[i].ToString().Replace("{", string.Empty).Replace("\"", string.Empty).Replace(":", string.Empty).Trim();
                    Value = multiArray[i = i + 2].ToString().Replace("{", string.Empty).Replace("\"", string.Empty).Replace(":", string.Empty).Trim();
                    ValueList.Add(Key, Value);

                }
                catch (Exception ex)
                {
                }
            }
            Person person = new Person()
            {
                FirstName = ValueList["FirstName"],
                LastName = ValueList["LastName"],
                AddressId = (long?)Convert.ToInt64(ValueList["AddressId"])
            };

            _context.Persons.Add(person);
            return person.Id;

        }

        public class GetAllRequest : Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
        }

        public async Task<string> GetAll(GetAllRequest request)
        {
            Person person = new Person();
            string json = string.Empty;
            if (request is not null)
            {
                var same = _context.Persons.Find(request.Id);
                
                same.FirstName = request.FirstName;
                same.LastName = request.LastName;
                same.Address = request.Address;

                json = "{";
                json += "\"FirstName\":\"" + request.FirstName + "\",";

                json += "\"LastName\":\"" + request.LastName + "\",";

                json += "\"Address\":{";

                var addresses = _context.Addresses.Where(x=>x.Id==request.AddressId).ToList();

                for (int i = 0; i < addresses.Count(); i++)
                {
                    json += "\"City\":\"" + addresses[i].City + "\",";
                    json += "\"Addressline\":\"" + addresses[i].AddressLine + "\",";
                    
                }

                json += "}}";
            }
            else
            {
                return string.Empty;
            }
            return json;
        }

        




        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            return await _context.Persons.ToListAsync();
        }

        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // PUT: api/Person/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(long id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Person
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
          if (_context.Persons == null)
          {
              return Problem("Entity set 'AppDbContext.Persons'  is null.");
          }
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            if (_context.Persons == null)
            {
                return NotFound();
            }
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(long id)
        {
            return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
