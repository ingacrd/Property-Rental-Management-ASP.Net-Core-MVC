using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    [Authorize(Roles = "Owner, Manager")]
    public class RentalsController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public RentalsController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Rentals.Include(r => r.Apartment).Include(r => r.Tenant);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Apartment)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId");
            return View();
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,TenantId,ApartmentId,RentalDate,EndContractDate")] Rental rental)
        {
            rental.RentalDate = DateOnly.FromDateTime(DateTime.Now);
            _context.Add(rental);

            var apartment = (from a in _context.Apartments
                             where a.ApartmentId == rental.ApartmentId
                             select a).FirstOrDefault();

            apartment.StatusId = (int)StatusType.ApartmentRented;
            _context.Update(apartment);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = rental.ApartmentId });
            
            
            
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", rental.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId", rental.TenantId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,TenantId,ApartmentId,RentalDate,EndContractDate")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }
            if(rental.EndContractDate > DateOnly.FromDateTime(DateTime.Now))
            {
                var apartment = (from a in _context.Apartments
                                 where a.ApartmentId == rental.ApartmentId
                                 select a).FirstOrDefault();
                apartment.StatusId = (int)StatusType.ApartmentRented;
                _context.Update(apartment);
            }

                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            return RedirectToAction("Details", "Apartments", new { id = rental.ApartmentId });
           // return RedirectToAction(nameof(Index));
            
        }

        // GET: Rentals/EndContract/5
        public async Task<IActionResult> EndContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Apartment)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("EndContract")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndContractConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            
            if (rental != null)
            {
                var apartment = (from a in _context.Apartments
                                 where a.ApartmentId == rental.ApartmentId
                                 select a).FirstOrDefault();
                apartment.StatusId = (int)StatusType.ApartmentAvailable;
                rental.EndContractDate = DateOnly.FromDateTime(DateTime.Now);
                try
                {
                    _context.Update(rental);
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction("Details", "Apartments", new { id = rental.ApartmentId });
            
        }
        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Apartment)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }
    }
}
