using Microsoft.AspNetCore.Mvc;
using KursavayaECS.Models;
using KursavayaECS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KursavayaECS.AppServices;

namespace KursavayaECS.Controllers
{
    public class AuthController(
        AppDbContext ctx, IAuthenticationServices authS) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel regModel)
        {
            if(regModel.BirthDate >= DateOnly.FromDateTime(DateTime.Now))
                ModelState.AddModelError(nameof(regModel.BirthDate), "Невозможно указать дату рождения больше чем текущаяя");

            var users = ctx.Users.ToList();

            foreach (var user in users)
            {
                if(user.Email == regModel.Email)
                    ModelState.AddModelError(nameof(regModel.Email), "Пользователь с таким Email уже существует");
            }
            
            if(ModelState.IsValid)
            {
                LoginViewModel LVM = new LoginViewModel
                {
                    Email = regModel.Email,
                    Password = regModel.Password
                };

                var hashedPassword = authS.GenerateHash(regModel.Password);

                var user = new AppUser
                {
                    ID = Guid.NewGuid(),
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,                  
                    Email = regModel.Email,
                    Phone = regModel.Phone,
                    PasswordHash = hashedPassword,
                    Role = "Default",
                    BirthDate = regModel.BirthDate
                };

                await ctx.Users.AddAsync(user);
                await ctx.SaveChangesAsync();

                return await Login(LVM);
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach(var error in errors)
                {
                    Console.WriteLine(error.Exception);
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == loginModel.Email);

                if (user == null)
                {
                    ModelState.AddModelError(nameof(loginModel.Email), "User with that email not found");
                    return View();
                }

                var result = authS.Verify(loginModel.Password, user.PasswordHash);

                if (result == false)
                {
                    ModelState.AddModelError(nameof(loginModel.Password), "Password is invalid");
                    return View();
                }

                string policy = "";

                switch(user.Role)
                {
                    case "Default": policy = "Default"; 
                        HttpContext.Response.Cookies.Append("SoTastyKokis", loginModel.Email);
                        HttpContext.Response.Cookies.Append("VeryKokis", loginModel.Password);
                        ; break;
                    case "Admin": policy = "Admin"; break;
                    case "Student": policy = "Student"; break;
                    case "Teacher": policy = "Teacher"; break;
                }

                var token = authS.GenerateToken(user, policy);

                if( user.Role != "Default")
                {
                    HttpContext.Response.Cookies.Delete("SoTastyKokis");
                    HttpContext.Response.Cookies.Delete("VeryKokis");
                }

                HttpContext.Response.Cookies.Append("tasty-kokis", token);
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("tasty-kokis");

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult ShowRegistred()
        {
            var Users = ctx.Users.ToList();
            var RegistredUserModel = new List<RegisterViewModel>();

            foreach(var user in Users)
            {
                var currentUser = new RegisterViewModel
                {
                    ID = user.ID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,                   
                    Email = user.Email,
                    Phone = user.Phone,
                    Password = user.PasswordHash,
                };

                RegistredUserModel.Add(currentUser);
            }

            return View(RegistredUserModel);
        }
        
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult DeleteUser(Guid ID)
        {
            var userToDelete = ctx.Users.FirstOrDefault(c => c.ID == ID);

            var courses = ctx.Courses.Include(c => c.Teacher.TeacherUser).ToList();

            foreach (var course in courses)
            {
                if (course.Teacher != null)
                {
                    if (course.Teacher.TeacherUser.ID == ID)
                        course.Teacher = null;
                }
                
                
            }
            
            if (userToDelete == null) return RedirectToAction("ShowRegistred");
            
            ctx.Users.Remove(userToDelete);
            ctx.SaveChanges();

            return RedirectToAction("ShowRegistred");

        }
    }
}

