using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PropertyRentals.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using NuGet.Versioning;
using Microsoft.EntityFrameworkCore;


namespace PropertyRentals.Controllers
{
    public class AccessController : Controller
    {

        public IActionResult Login()
        {

            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                ViewData["UserID"] = ClaimTypes.NameIdentifier;
                return RedirectToAction("Index", "Home");

            }
            
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Access/SignUp
        [HttpPost]
        public async Task<IActionResult> SignUp(User newUser)
        {
            if (newUser.PhotoFile != null && newUser.PhotoFile.Length > 0)
            {

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(newUser.PhotoFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newUser.PhotoFile.CopyToAsync(stream);
                }
                // newUser.Photo = "/images/users/" + fileName;
                newUser.Photo = fileName;
            }

            if (string.IsNullOrEmpty(newUser.Username) ||
                string.IsNullOrEmpty(newUser.Password) ||
                string.IsNullOrEmpty(newUser.Email) ||
                string.IsNullOrEmpty(newUser.Phone) ||
                string.IsNullOrEmpty(newUser.FirstName) ||
                string.IsNullOrEmpty(newUser.LastName) ||
                string.IsNullOrEmpty(newUser.Email) ||
                string.IsNullOrEmpty(newUser.Phone) )
            {
                ViewData["SignUpMessage"] = "All fields are required.";
                return View();
            }

            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            if (!Regex.IsMatch(newUser.Password, passwordPattern))
            {
                ViewData["SignUpMessage"] = "Password has to be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
                return View();
            }

            string emailPattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(newUser.Email, emailPattern))
            {
                ViewData["SignUpMessage"] = "The email is not valid";
                newUser.Email = "";
                return View();
            }

            string phonePattern = @"^\d{10}$";
            if (!Regex.IsMatch(newUser.Phone, phonePattern))
            {
                ViewData["SignUpMessage"] = "The phone number only has 10 numbers";
                newUser.Phone = "";
                return View();
            }

            newUser.UserType = "Tenant";
            using (PropertyRentalDbContext db = new PropertyRentalDbContext())
            {
                
                var userExist = (from u in db.Users
                                 where u.Username == newUser.Username
                                 select u).FirstOrDefault();
                if (userExist == null)
                {
                    db.Add(newUser);
                    await db.SaveChangesAsync();

                    var userCreated = (from u in db.Users
                                       where u.Username == newUser.Username
                                       select u).FirstOrDefault();

                    Tenant tnt = new Tenant()
                    {
                        UserId = Convert.ToInt32(userCreated.UserId),
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Phone = newUser.Phone,
                    };
                    db.Add(tnt);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Login", "Access");
                }
                else
                {
                    ViewData["SignUpMessage"] = "Username already exist";
                    
                }

               

            }
            


            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            using(PropertyRentalDbContext db =  new PropertyRentalDbContext())
            {
                var user = (from usr in db.Users
                           where usr.Username == modelLogin.UserName
                           select usr).FirstOrDefault();

                if (user != null)
                {
       
                    if (user.Password == modelLogin.Password)
                    {
                    
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, modelLogin.UserName),
                        
                            new Claim(ClaimTypes.Role, user.UserType)
                        };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                            );
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = modelLogin.KeepLogedIn
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);


                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ViewData["ValidateMessage"] = "wrong password";
                    }
                    

                } else
                {
                    ViewData["ValidateMessage"] = "user not found";
                }

            }
            
            
            return View();
        }


    }
}
