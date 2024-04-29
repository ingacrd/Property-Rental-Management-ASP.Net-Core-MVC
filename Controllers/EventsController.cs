using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    public class EventsController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public EventsController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Events.Include(e => e.Apartment).Include(e => e.Status);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Apartment)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,ApartmentId,Description")] Event @event)
        {
            var newEvent = new Event
            {
                ApartmentId = @event.ApartmentId,
                Description = @event.Description,
                EventDate = DateOnly.FromDateTime(DateTime.Now),
                StatusId = (int)StatusType.EventPending
            };

                _context.Add(newEvent);
                await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = @event.ApartmentId });
            
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", @event.ApartmentId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", @event.StatusId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,ApartmentId,Description,EventDate,StatusId")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", @event.ApartmentId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", @event.StatusId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();

            }
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = @event.ApartmentId });


        }

        // POST: Events/SolvedUpdate/5
        [HttpGet]
        public async Task<IActionResult> SolvedUpdate(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
                
            }
            @event.StatusId = (int)StatusType.EventSolved;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = @event.ApartmentId });
            //return RedirectToAction(nameof(Index));
        }

        // POST: Events/SolvedUpdate/5
        [HttpGet]
        public async Task<IActionResult> PendingUpdate(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            @event.StatusId = (int)StatusType.EventPending;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = @event.ApartmentId });
            //return RedirectToAction(nameof(Index));
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
