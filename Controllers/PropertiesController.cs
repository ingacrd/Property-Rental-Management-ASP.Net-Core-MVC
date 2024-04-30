using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    //[Authorize] //Only if authorization can enter
    [Authorize(Roles = "Manager, Owner")]
    public class PropertiesController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public PropertiesController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Properties.Include(p => p.Manager).Include(p => p.Owner);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(m => m.Manager)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.PropertyCode == id);
            if (@property == null)
            {
                return NotFound();
            }

            var apartments = from a in _context.Apartments
                             join p in _context.Photos
                             on a.ApartmentId equals p.ApartmentId into apartmentPhotos
                             from photo in apartmentPhotos.DefaultIfEmpty()
                             join b in _context.Properties
                             on a.PropertyCode equals b.PropertyCode into apartmentProperty
                             where a.PropertyCode == id
                             from building in apartmentProperty.DefaultIfEmpty()
                             group new { a, photo, building } by a into g
                             select new ApartmentPhotoLinkViewModel
                             {
                                 Apartment = g.Key,
                                 PhotoLink = g.Select(p => p.photo.PhotoLink).FirstOrDefault(),
                                 Address = g.Select(p => p.building.Address).FirstOrDefault()
                             };
            ViewBag.Apartments = await apartments.ToListAsync();
            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId");
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyCode,OwnerId,ManagerId,Name,Description,Address,City,State,ZipCode")] Property @property)
        {
          
            //if (ModelState.IsValid)
           //{
                _context.Add(@property);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", @property.ManagerId);
            //ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", @property.OwnerId);
            //return View(@property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", @property.ManagerId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", @property.OwnerId);
            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PropertyCode,OwnerId,ManagerId,Name,Description,Address,City,State,ZipCode")] Property @property)
        {
            if (id != @property.PropertyCode)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", @property.ManagerId);
            //ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", @property.OwnerId);
            //return View(@property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(m => m.Manager)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.PropertyCode == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(string id)
        {
            return _context.Properties.Any(e => e.PropertyCode == id);
        }
    }
}
