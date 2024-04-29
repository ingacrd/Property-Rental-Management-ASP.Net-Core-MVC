using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    [Authorize] //Only if authorization can enter
    public class AppointmentsController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public AppointmentsController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // found the user
            ClaimsPrincipal claimUser = HttpContext.User;
            var nameIdentifierClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            string nameIdentifierValue = nameIdentifierClaim.Value;
            var usr = (from u in _context.Users
                       where u.Username == nameIdentifierValue
                       select u).FirstOrDefault();
            if (claimUser.IsInRole("Tenant"))
            {
                var appointment = from a in _context.Appointments
                                              where a.TenantId == usr.UserId
                                              select a;
                return View(await appointment.ToListAsync());
            }
                                           
            var propertyRentalDbContext = _context.Appointments.Include(a => a.Apartment).Include(a => a.Manager).Include(a => a.Status).Include(a => a.Tenant);
            return View(await propertyRentalDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment)
                .Include(a => a.Manager)
                .Include(a => a.Status)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,TenantId,ManagerId,ApartmentId,AppointmentDateTime,Description,StatusId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", appointment.ApartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", appointment.ManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", appointment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId", appointment.TenantId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", appointment.ApartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", appointment.ManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", appointment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId", appointment.TenantId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,TenantId,ManagerId,ApartmentId,AppointmentDateTime,Description,StatusId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", appointment.ApartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", appointment.ManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", appointment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "TenantId", "TenantId", appointment.TenantId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment)
                .Include(a => a.Manager)
                .Include(a => a.Status)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
