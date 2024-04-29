using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;

namespace PropertyRentals.Controllers
{
    [Authorize] //Only if authorization can enter

    public class Appointments1Controller : Controller
    {
        private readonly PropertyRentalDbContext _context;
        public Appointments1Controller(PropertyRentalDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Appointment app)
        {

            var appointment = new Appointment
            {
                //TenantId = 99992,
                TenantId = app.TenantId,
                ManagerId = app.ManagerId, //ON THE DATABASE IS [ManagerID]
                //ManagerId = 99990, //ON THE DATABASE IS [ManagerID]
                ApartmentId = app.ApartmentId,
                AppointmentDateTime = app.AppointmentDateTime,
                Description = app.Description,
                StatusId = app.StatusId
            };


            // Add the message to the context and save changes
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Redirect to a different action or return a view
            return RedirectToAction("Details", "Apartments", new { id = appointment.ApartmentId });
            //return RedirectToAction("Index", "Home"); // Redirect to Index action for example

        }

        [HttpGet] 
        public async Task<ActionResult> ConfirmUpdate(int id) 
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            app.StatusId = 5;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = app.ApartmentId });
            //return RedirectToAction("Index", "Home");
        }

        [HttpGet] 
        public async Task<ActionResult> CancelUpdate(int id) 
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }

            app.StatusId = 6;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = app.ApartmentId });
            //return RedirectToAction("Index", "Home");
        }

        // GET: Apartments/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            
                       
            _context.Appointments.Remove(app);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Apartments", new { id = app.ApartmentId });
            //return RedirectToAction("Index", "Home");

        }

            


    }
}
