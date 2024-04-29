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
    public class MessagesController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public MessagesController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Messages.Include(m => m.Apartment).Include(m => m.ReceiverUser).Include(m => m.SenderUser).Include(m => m.Status);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Apartment)
                .Include(m => m.ReceiverUser)
                .Include(m => m.SenderUser)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["ReceiverUserId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["SenderUserId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,SenderUserId,ReceiverUserId,ApartmentId,Subject,Body,MessageDateTime,StatusId")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", message.ApartmentId);
            ViewData["ReceiverUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.ReceiverUserId);
            ViewData["SenderUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderUserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", message.StatusId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", message.ApartmentId);
            ViewData["ReceiverUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.ReceiverUserId);
            ViewData["SenderUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderUserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", message.StatusId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,SenderUserId,ReceiverUserId,ApartmentId,Subject,Body,MessageDateTime,StatusId")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", message.ApartmentId);
            ViewData["ReceiverUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.ReceiverUserId);
            ViewData["SenderUserId"] = new SelectList(_context.Users, "UserId", "UserId", message.SenderUserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", message.StatusId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Apartment)
                .Include(m => m.ReceiverUser)
                .Include(m => m.SenderUser)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
