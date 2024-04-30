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
    [Authorize(Roles = "Owner")]
    public class TenantsController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public TenantsController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Tenants
        public async Task<IActionResult> Index()
        {
            var propertyRentalDbContext = _context.Tenants.Include(t => t.User);

            var tenants = await (from t in _context.Tenants
                                      join u in _context.Users on t.UserId equals u.UserId
                                      select new
                                      {
                                          Tenant = t,
                                          User = u
                                      }).ToListAsync();

            ViewBag.Tenants = tenants;
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Tenants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TenantId == id);
            if (tenant == null)
            {
                return NotFound();
            }

            var user = (from u in _context.Users
                        where u.UserId == tenant.UserId
                        select u).FirstOrDefault();

            tenant.FirstName = user.FirstName;
            tenant.LastName = user.LastName;
            tenant.Email = user.Email;
            tenant.Phone = user.Phone;

            return View(tenant);
        }

        // GET: Tenants/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Tenants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,UserId,FirstName,LastName,Email,Phone")] Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tenant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", tenant.UserId);
            return View(tenant);
        }

        // GET: Tenants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }


            var user = (from u in _context.Users
                        where u.UserId == tenant.UserId
                        select u).FirstOrDefault();

            tenant.FirstName = user.FirstName;
            tenant.LastName = user.LastName;
            tenant.Email = user.Email;
            tenant.Phone = user.Phone;


            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", tenant.UserId);
            return View(tenant);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TenantId,UserId,FirstName,LastName,Email,Phone")] Tenant tenant)
        {
            if (id != tenant.TenantId)
            {
                return NotFound();
            }

            var userToUpdate = (from u in _context.Users
                               where u.UserId == tenant.UserId
                               select u).FirstOrDefault();
            userToUpdate.FirstName = tenant.FirstName;
            userToUpdate.LastName = tenant.LastName;
            userToUpdate.Email = tenant.Email;
            userToUpdate.Phone = tenant.Phone;

                try
                {
                    _context.Update(tenant);
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TenantExists(tenant.TenantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", tenant.UserId);
            //return View(tenant);
        }

        // GET: Tenants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenant = await _context.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TenantId == id);
            if (tenant == null)
            {
                return NotFound();
            }


            var user = (from u in _context.Users
                        where u.UserId == tenant.UserId
                        select u).FirstOrDefault();

            tenant.FirstName = user.FirstName;
            tenant.LastName = user.LastName;
            tenant.Email = user.Email;
            tenant.Phone = user.Phone;

            return View(tenant);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            
            if (tenant != null)
            {
                var user = (from u in _context.Users
                            where u.UserId == tenant.UserId
                            select u).FirstOrDefault();
                try {
                    _context.Tenants.Remove(tenant);
                    _context.Users.Remove(user);
                }
                catch(Exception e)
                {
                    ViewData["errorMesagge"] = "You cant delete this tenant, error: " + e;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TenantExists(int id)
        {
            return _context.Tenants.Any(e => e.TenantId == id);
        }
    }
}
