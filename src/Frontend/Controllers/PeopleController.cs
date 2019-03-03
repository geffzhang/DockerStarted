using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Frontend.Data;
using Entities;

namespace Frontend.Controllers
{
    public class PeopleController : Controller
    {
        private IPeopleService _people;

        public PeopleController(IPeopleService people)
        {
            _people = people;
        }

        /// GET: People
        public async Task<IActionResult> Index(string filter)
        {
            var list = await _people.GetListAsync();
            
            return View(list);
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _people.GetByIdAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                await _people.CreateAsync(person);

                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _people.GetByIdAsync(id);

            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email")] Person person)
        {
            var original = await _people.GetByIdAsync(id);

            if (original == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid && await this.TryUpdateModelAsync<Person>(original))
            {
                await _people.UpdateAsync(original);

                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _people.GetByIdAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _people.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}