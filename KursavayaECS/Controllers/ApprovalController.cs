using KursavayaECS.AppServices;
using KursavayaECS.Data;
using Microsoft.AspNetCore.Mvc;
using KursavayaECS.Models;

namespace KursavayaECS.Controllers
{
    public class ApprovalController(
        AppDbContext ctx, IAuthenticationServices authS) : Controller
    {
        public IActionResult Index()
        {
            var User = ctx.Users.FirstOrDefault(u => u.ID == authS.GetUserIdFromToken(HttpContext));

            IndexApproveModel model = new IndexApproveModel
            {
                ID = User.ID,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Phone = User.Phone,
                BirthDate = User.BirthDate,
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveStudent()
        {
            return View();
        }    

        [HttpPost]
        public async Task<IActionResult> ApproveStudent(StudentApproveModel studentApprove)
        {
            var User = ctx.Users.FirstOrDefault(c => c.ID == authS.GetUserIdFromToken(HttpContext));

            if(ModelState.IsValid)
            {
                Student student = new Student
                {
                    ID = Guid.NewGuid(),
                    StudentUser = User,
                    PrefferedSpec = studentApprove.PrefSpec
                };

                User.Role = "Student";;

                await ctx.Students.AddAsync(student);

                await ctx.SaveChangesAsync();

                string token = authS.GenerateToken(User, "Student");

                HttpContext.Response.Cookies.Delete("tasty-kokis");

                HttpContext.Response.Cookies.Append("tasty-kokis", token);

                return RedirectToAction("Index", "Home"); 
            }
          
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ApproveTeacher()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTeacher(TeacherApproveModel teacherApprove)
        {
            var User = ctx.Users.FirstOrDefault(c => c.ID == authS.GetUserIdFromToken(HttpContext));

            if (ModelState.IsValid)
            {
                Teacher teacher = new Teacher
                {
                    ID = Guid.NewGuid(),
                    TeacherUser = User,
                    Specialization = teacherApprove.Specialization,
                    Category = teacherApprove.Category,
                    ExpirenceYears = teacherApprove.ExpirenceYears
                };

                User.Role = "Teacher";

                await ctx.Teachers.AddAsync(teacher);

                await ctx.SaveChangesAsync();

                string token = authS.GenerateToken(User, "Teacher");

                HttpContext.Response.Cookies.Delete("tasty-kokis");

                HttpContext.Response.Cookies.Append("tasty-kokis", token);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
