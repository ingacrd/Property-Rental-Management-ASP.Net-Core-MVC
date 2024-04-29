using Microsoft.AspNetCore.Mvc;
using PropertyRentals.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;




namespace PropertyRentals.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly PropertyRentalDbContext _context;


        public HomeController(ILogger<HomeController> logger, PropertyRentalDbContext context)
        {
            _logger = logger;
            _context = context;
        }



        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // find the user
                ClaimsPrincipal claimUser = HttpContext.User;
                var nameIdentifierClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                string nameIdentifierValue = nameIdentifierClaim.Value;
                var user = (from u in _context.Users
                            where u.Username == nameIdentifierValue
                            select u).FirstOrDefault();
                // ======================   Find if the user have messages

                var messages = await (from message in _context.Messages
                                      where message.ReceiverUser == user && message.StatusId == 11
                                      select message).ToListAsync();
                if (messages.Count == 0)
                {
                    ViewBag.Messages = null;
                }
                else
                {
                    ViewBag.Messages = messages;
                }

                if (claimUser.IsInRole("Tenant"))
                {
                    // ======================== Find if there are Appointments
                    var now = DateTime.Now;

                    var appointments = await (from appointment in _context.Appointments
                                              join t in _context.Tenants on appointment.TenantId equals t.TenantId into at
                                              from tenant in at.DefaultIfEmpty()
                                              where tenant.UserId == user.UserId && appointment.AppointmentDateTime > now
                                              select appointment).ToListAsync();

                    if (appointments.Count == 0)
                    {
                        ViewBag.Appointments = null; // Set ViewBag to null if there are no apartments rented
                    }
                    else
                    {
                        ViewBag.Appointments = appointments; // Assign the apartmentsRented to ViewBag
                    }

                    //ViewBag.Appointments = await appointments.ToListAsync();

                    // ====================   Find apartments rented
                    var apartmentsRented = (from a in _context.Apartments
                                            join r in _context.Rentals on a.ApartmentId equals r.ApartmentId into ar

                                            from apartment in ar.DefaultIfEmpty()
                                            join t in _context.Tenants on apartment.TenantId equals t.TenantId into art

                                            from rented in art.DefaultIfEmpty()
                                            join p in _context.Photos on a.ApartmentId equals p.ApartmentId into artp

                                            from photo in artp.DefaultIfEmpty()
                                            join b in _context.Properties on a.PropertyCode equals b.PropertyCode into artpp

                                            from property in artpp.DefaultIfEmpty()
                                            where rented.UserId == user.UserId && apartment.EndContractDate > DateOnly.FromDateTime(DateTime.Now)
                                            group new { a, photo, property } by a into g
                                            select new ApartmentPhotoLinkViewModel
                                            {
                                                Apartment = g.Key,
                                                PhotoLink = g.Select(p => p.photo.PhotoLink).FirstOrDefault(),
                                                Address = g.Select(p => p.property.Address).FirstOrDefault()
                                            }).ToList();

                    if (apartmentsRented.Count == 0)
                    {
                        ViewBag.ApartmentsRented = null;
                    }
                    else
                    {
                        ViewBag.ApartmentsRented = apartmentsRented;
                    }



                }
                else if (claimUser.IsInRole("Manager") || (claimUser.IsInRole("Owner")))
                {
                    if (claimUser.IsInRole("Manager"))
                    {
                        //Find Appointments to Mananger 
                        var now = DateTime.Now;

                        var appointments = await (from appointment in _context.Appointments
                                                  join m in _context.Managers on appointment.ManagerId equals m.ManagerId into at
                                                  from manager in at.DefaultIfEmpty()
                                                  where manager.UserId == user.UserId && appointment.AppointmentDateTime > now
                                                  select appointment).ToListAsync();

                        if (appointments.Count == 0)
                        {
                            ViewBag.Appointments = null; // Set ViewBag to null if there are no apartments rented
                        }
                        else
                        {
                            ViewBag.Appointments = appointments; // Assign the apartmentsRented to ViewBag
                        }
                    }

                    var pendingEvents = await (from e in _context.Events
                                               where e.StatusId == (int)StatusType.EventPending
                                               select e).ToListAsync();

                    ViewBag.PendingEvents = pendingEvents;

                }



            }


            //===============================  List of apartments
            //where a.StatusId == 1
            var propertyRentalDbContext = from a in _context.Apartments
                                          join p in _context.Photos
                                          on a.ApartmentId equals p.ApartmentId into apartmentPhotos
                                          from photo in apartmentPhotos.DefaultIfEmpty()

                                          join b in _context.Properties
                                          on a.PropertyCode equals b.PropertyCode into apartmentProperty
                                          from property in apartmentProperty.DefaultIfEmpty()
                                          group new { a, photo, property } by a into g
                                          select new ApartmentPhotoLinkViewModel 
                                           {
                                              Apartment = g.Key,
                                              PhotoLink = g.Select(p => p.photo.PhotoLink).FirstOrDefault(),
                                              Address = g.Select(p => p.property.Address).FirstOrDefault()
                                          };

            return View(await propertyRentalDbContext.ToListAsync());

        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        

        public IActionResult Privacy()
        {
            return View();
        }
       
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
            
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
