using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentals.Models;
using System.Security.Claims;

namespace PropertyRentals.Controllers
{
    [Authorize] //Only if authorization can enter
    public class MensajesController : Controller
    {
        private readonly PropertyRentalDbContext _context;

        public MensajesController(PropertyRentalDbContext context)
        {
            _context = context;
        }
        // GET: Mensajes/Create
        public IActionResult Create()
        {
            int appId;
            if (Request.Query.TryGetValue("apartmentId", out var apartmentId))
            {
                appId = Convert.ToInt32(apartmentId);
                ViewData["ApartmentId"] = appId;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Message msj)
        {
            
            var message = new Message
            {
                SenderUserId = msj.SenderUserId,
                ReceiverUserId = msj.ReceiverUserId,
                Subject = msj.Subject,
                Body = msj.Body,
                MessageDateTime = DateTime.Now,
                StatusId = 11,
                ApartmentId = msj.ApartmentId
            };

            //// found the sender
            //ClaimsPrincipal claimUser = HttpContext.User;
            //var nameIdentifierClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            //string nameIdentifierValue = nameIdentifierClaim.Value;
            //var usr = (from u in _context.Users
            //           where u.Username == nameIdentifierValue
            //           select u).FirstOrDefault();
            //int senderUserId = usr.UserId;

            //// found receiver
            //var apt = (from r in _context.Apartments
            //           where r.ApartmentId == msj.ApartmentId
            //           select r).FirstOrDefault();
            //var property = (from p in _context.Properties
            //                where p.PropertyCode == apt.PropertyCode
            //                select p).FirstOrDefault();
            //var receiverUsr = (from u in _context.Users
            //                     where u.UserId == property.ManagerId
            //                     select u).FirstOrDefault();
           
            //int receiverUserId = receiverUsr.UserId;

            //// Setting the IDs
            //message.SenderUserId = senderUserId;
            //message.ReceiverUserId = receiverUserId;
  

            // Add the message to the context and save changes
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Redirect to a different action or return a view
            return RedirectToAction("Details", "Apartments", new { id = message.ApartmentId });
            //return RedirectToAction("Index", "Home"); // Redirect to Index action for example
            //return View();
        }

   
    }
}
