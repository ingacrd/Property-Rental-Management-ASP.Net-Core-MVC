using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    [Authorize(Roles = "Manager,Owner")] //Only if authorization can enter
    public class PhotosController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public PhotosController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Photos
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Photos.Include(p => p.Apartment);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.Apartment)
                .FirstOrDefaultAsync(m => m.PhotoId == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // GET: Photos/Create
        
        public IActionResult Create(int apartmentId)
        {
            ViewData["test"] = apartmentId;

            var selectedApartment = _context.Apartments.FirstOrDefault(a => a.ApartmentId == apartmentId);

            if (selectedApartment != null)
            {
                ViewData["ApartmentId"] = new SelectList(new List<Apartment> { selectedApartment }, "ApartmentId", "ApartmentId");
            }
            else
            {
                return NotFound();
            }

            return View();
            //var selectedApartment = _context.Apartments.FirstOrDefault(a => a.ApartmentId == apartmentId);
            //ViewData["ApartmentId"] = new SelectList(new List<Apartment> { selectedApartment }, "ApartmentId", "ApartmentId");

            //return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PhotoId,PhotoLink,ApartmentId")] Photo photo)
        public async Task<IActionResult> Create(Photo photo)
        {
            if (photo.PhotoFile != null && photo.PhotoFile.Length > 0)
            {
                // Generate a unique file name to prevent overwriting existing files
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.PhotoFile.FileName);

                // Combine the file name with the path where you want to save the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "apartments", fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.PhotoFile.CopyToAsync(stream);
                }

                // Set the PhotoLink property to the path of the saved file
                photo.PhotoLink = "/images/apartments/" + fileName;
            }
            _context.Add(photo);
                await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = photo.ApartmentId });
            //return RedirectToAction(nameof(Index));
            //}
            //ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", photo.ApartmentId);
            //return View(photo);
        }

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", photo.ApartmentId);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhotoId,PhotoLink,ApartmentId")] Photo photo)
        {
            if (id != photo.PhotoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.PhotoId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", photo.ApartmentId);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .Include(p => p.Apartment)
                .FirstOrDefaultAsync(m => m.PhotoId == id);
            if (photo == null)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = photo.ApartmentId });
            //return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.PhotoId == id);
        }
    }
}
