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
    public class ApartmentsController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public ApartmentsController(PropertyRentalDbContext context)
        {
            _context = context;
        }

        // GET: Apartments
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
                var apartments = from t in _context.Tenants
                                 join r in _context.Rentals on t.TenantId equals r.TenantId into rt
                                 from rental in rt.DefaultIfEmpty()
                                 join a in _context.Apartments on rental.ApartmentId equals a.ApartmentId into art
                                 from apartment in art.DefaultIfEmpty()
                                 where t.UserId == usr.UserId && rental != null
                                 select apartment;
                return View(await apartments.ToListAsync());

            }

                
            

            var propertyRentalDbContext = _context.Apartments
                .Include(a => a.PropertyCodeNavigation).
                Include(a => a.Status);

            return View(await propertyRentalDbContext.ToListAsync());
        }
        
        [HttpGet]
        public async Task<ActionResult> MakeAvailable(int id)
        {
            var app = await _context.Apartments.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            app.StatusId = (int) StatusType.ApartmentAvailable;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = app.ApartmentId });
 
        }
        
        [HttpGet]
        public async Task<ActionResult> MakeUnavailable(int id)
        {
            var app = await _context.Apartments.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            app.StatusId = (int)StatusType.ApartmentUnavailable;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = app.ApartmentId });

        }
        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.PropertyCodeNavigation)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            // Retrieve photos associated with the apartment
            var photos = await _context.Photos
                .Where(p => p.ApartmentId == id)
                .ToListAsync();

            //found property and manager

            var property = await _context.Properties
                    .Include(p => p.Manager)
                    .FirstOrDefaultAsync(p => p.PropertyCode == apartment.PropertyCode);

            // Access the ManagerUser from the property
            var managerUser = property?.Manager;
           // var managerUserJson = Newtonsoft.Json.JsonConvert.SerializeObject(managerUser);
            ViewData["ManagerUserId"] = managerUser.UserId;
            ViewData["ManagerId"] = managerUser.ManagerId;
            //ViewData["ManagerUser"] = managerUser;

            // find the user
            ClaimsPrincipal claimUser = HttpContext.User;
            var nameIdentifierClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            string nameIdentifierValue = nameIdentifierClaim.Value;

            var usr = (from u in _context.Users
                       where u.Username == nameIdentifierValue
                       select u).FirstOrDefault();

            ViewData["UserId"] = usr.UserId;
            //find all Messages on The Apartment for the user

            var messageDetails = await (from m in _context.Messages
                                        where (m.SenderUserId == usr.UserId || m.ReceiverUserId == usr.UserId)&& m.ApartmentId == apartment.ApartmentId
                                        join u in _context.Users on m.SenderUserId equals u.UserId
                                        select new
                                        {
                                            Message = m,
                                            SenderDetails = u
                                        }).ToListAsync();

            //update the message not readed to readed

            var messagesNotRead = from m in _context.Messages
                                  where (m.ReceiverUserId == usr.UserId && m.StatusId == 11) && m.ApartmentId == apartment.ApartmentId
                                  select m;
            foreach (var message in messagesNotRead)
            {
                message.StatusId = 10; // mark as read
            }
            _context.SaveChanges();


            if (claimUser.IsInRole("Tenant"))
            {
                
                var appointments = await (from a in _context.Appointments
                                          join t in _context.Tenants on a.TenantId equals t.TenantId
                                          where t.UserId == usr.UserId && a.ApartmentId == apartment.ApartmentId
                                          select a).ToListAsync();
                ViewBag.Appointments = appointments;
                var tenant = (from t in _context.Tenants
                              where t.UserId == usr.UserId
                              select t).FirstOrDefault();
                ViewData["TenantId"] = tenant.TenantId;

            } 
            else if (claimUser.IsInRole("Manager") || claimUser.IsInRole("Owner"))
            {

                var alltenants = await (from u in _context.Users
                                        where u.UserType == "Tenant"
                                        select u).ToListAsync();
                ViewBag.AllTenants = alltenants;

               //StatusType sType = new StatusType();
                var eventsDetails = await (from e in _context.Events
                                    where e.ApartmentId == apartment.ApartmentId
                                    select new
                                    {
                                        Events = e,
                                        statusType = (StatusType)e.StatusId
                                    }).ToListAsync();
                
                ViewBag.EventDetails = eventsDetails;

                var rentalDetails = await (from r in _context.Rentals
                                           where r.ApartmentId == apartment.ApartmentId
                                           select r).ToListAsync();
                
                ViewBag.Rentals = rentalDetails;

                var apartmentRented = (from a in _context.Rentals
                                       where a.ApartmentId == apartment.ApartmentId && a.EndContractDate <= DateOnly.FromDateTime(DateTime.Now)
                                       select a).FirstOrDefault();
                if (apartmentRented != null)
                {
                    ViewData["RentalApartment"] = "false";
                }
                else ViewData["RentalApartment"] = "true";

                if (claimUser.IsInRole("Manager"))
                {
                    var appointments = await (from a in _context.Appointments
                                              join m in _context.Managers on a.ManagerId equals m.ManagerId
                                              where m.UserId == usr.UserId && a.ApartmentId == apartment.ApartmentId
                                              select a).ToListAsync();
                    ViewBag.Appointments = appointments;

                }
                else
                {
                    var appointments = await (from appointment in _context.Appointments
                                              join a in _context.Apartments on appointment.ApartmentId equals a.ApartmentId into aa

                                              from app in aa.DefaultIfEmpty()
                                              join p in _context.Properties on apartment.PropertyCode equals p.PropertyCode into aap

                                              from prop in aap.DefaultIfEmpty()
                                              where prop.ManagerId == managerUser.ManagerId && appointment.ApartmentId == apartment.ApartmentId
                                              select appointment).ToListAsync();

                    ViewBag.Appointments = appointments;

                }
            }

            ViewBag.Messages = messageDetails;
            ViewData["Property"] = property;
            ViewData["ManagerUser"] = managerUser;
            ViewBag.Photos = photos; 
            return View(apartment);



        }

        // GET: Apartments/Create
        public IActionResult Create()
        {
            ViewData["PropertyCode"] = new SelectList(_context.Properties, "PropertyCode", "PropertyCode");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,ApartmentCode,PropertyCode,Title,Type,Description,Rent,StatusId,Bedrooms,Bathrooms,FloorArea,ParkingSpots")] Apartment apartment)
        {
            
           _context.Add(apartment);
           await _context.SaveChangesAsync();

           return RedirectToAction(nameof(Index));
            
        }

        // GET: Apartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["PropertyCode"] = new SelectList(_context.Properties, "PropertyCode", "PropertyCode", apartment.PropertyCode);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", apartment.StatusId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,ApartmentCode,PropertyCode,Title,Type,Description,Rent,StatusId,Bedrooms,Bathrooms,FloorArea,ParkingSpots")] Apartment apartment)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.ApartmentId))
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
            //ViewData["PropertyCode"] = new SelectList(_context.Properties, "PropertyCode", "PropertyCode", apartment.PropertyCode);
            //ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", apartment.StatusId);
            //return View(apartment);
        }

        // GET: Apartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.PropertyCodeNavigation)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentId == id);
        }
    }
}
