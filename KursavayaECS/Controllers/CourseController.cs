using Microsoft.AspNetCore.Mvc;
using KursavayaECS.Models;
using KursavayaECS.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using KursavayaECS.AppServices;
using System;

namespace KursavayaECS.Controllers
{
    public class CourseController (AppDbContext ctx, IAuthenticationServices authS) : Controller
    {
        [Authorize(Policy = "StudentPolicy")]
        public IActionResult Index()
        {
            var Courses = ctx.Courses.Include(c => c.Teacher).Include(c => c.Teacher.TeacherUser).ToArray();
            var CourseModel = new List<CourseViewModel>();

            foreach (var course in Courses)
            {
                var currentCourse = new CourseViewModel
                {
                    ID = course.ID,
                    CourseName = course.CourseName,
                    CourseDescription = course.CourseDescription,
                    CoursePrice = course.CoursePrice,
                    Teacher = course.Teacher
                };

                CourseModel.Add(currentCourse);
            };

            return View(CourseModel);
        }

        public IActionResult SubscribeToCourse(Guid ID)
        {
            CourseClaim claim = new CourseClaim
            {
                ID = Guid.NewGuid(),
                Student = ctx.Students.FirstOrDefault(c => c.StudentUser.ID == authS.GetUserIdFromToken(HttpContext)),
                Course = ctx.Courses.FirstOrDefault(c => c.ID == ID)
            };

            ctx.CoursesClaims.Add(claim);
            ctx.SaveChanges();

            return RedirectToAction("Index", "Course");
        }

        public IActionResult UnSubscribeFromCourse(Guid ID)
        {
            var Claims = ctx.CoursesClaims.Include(c => c.Student).Include(c => c.Student.StudentUser).Include(c => c.Course).ToList();

            foreach(var claim in Claims)
            {
                if (claim.Course.ID == ID && claim.Student.StudentUser.ID == authS.GetUserIdFromToken(HttpContext))
                {
                    ctx.CoursesClaims.Remove(claim);
                    ctx.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Course");
        }

        [Authorize(Policy = "AdminPolicy")]
        public IActionResult IndexAdmin()
        {
            var Courses = ctx.Courses.Include(c => c.Teacher).ToList();
            var CourseModel = new List<CourseViewModel>();

            foreach (var course in Courses)
            {
                var currentCourse = new CourseViewModel
                {
                    ID = course.ID,
                    CourseName = course.CourseName,
                    CourseDescription = course.CourseDescription,
                    CoursePrice = course.CoursePrice
                };

                CourseModel.Add(currentCourse);
            };

            return View(CourseModel);
        }

        [Authorize(Policy = "TeacherPolicy")]
        public IActionResult IndexTeacher()
        {
            var Courses = ctx.Courses.Include(c => c.Teacher.TeacherUser).ToArray();
            var CourseModel = new List<CourseViewModel>();

            foreach (var course in Courses)
            {
                var currentCourse = new CourseViewModel
                {
                    ID = course.ID,
                    CourseName = course.CourseName,
                    CourseDescription = course.CourseDescription,
                    CoursePrice = course.CoursePrice,
                    Teacher = course.Teacher
                };

                CourseModel.Add(currentCourse);
            };

            return View(CourseModel);
        }

        [Authorize(Policy = "TeacherPolicy")]
        public IActionResult LeadCourse(Guid ID)
        {
            Course course = ctx.Courses.FirstOrDefault(c => c.ID == ID);

            Teacher teacher = ctx.Teachers.FirstOrDefault(c => c.TeacherUser.ID == authS.GetUserIdFromToken(HttpContext));

            course.Teacher = teacher;
            
            ctx.SaveChanges();

            return RedirectToAction("IndexTeacher", "Course");
        }

        [Authorize(Policy = "TeacherPolicy")]
        public IActionResult UnLeadCourse(Guid ID)
        {
            var Courses = ctx.Courses.Include(c => c.Teacher).Include(c => c.Teacher.TeacherUser).ToList();

            foreach (var course in Courses)
            {
                if (course.ID == ID && course.Teacher.TeacherUser.ID == authS.GetUserIdFromToken(HttpContext))
                {
                    course.Teacher = null;
                    ctx.SaveChanges();
                }
            }

            return RedirectToAction("IndexTeacher", "Course");
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> Insert()
        {
            return View();
        }

        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Insert(CourseViewModel courseModel)
        {
            if(ModelState.IsValid)
            {
                var course = new Course
                {
                    ID = Guid.NewGuid(),
                    CourseName = courseModel.CourseName,
                    CourseDescription = courseModel.CourseDescription,
                    CoursePrice = courseModel.CoursePrice
                };

                await ctx.Courses.AddAsync(course);
                await ctx.SaveChangesAsync();

                return RedirectToAction("IndexAdmin", "Course");
            }

            return View();
        }
        
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete(Guid ID)
        {
            var courseToDelete = ctx.Courses.FirstOrDefault(c => c.ID == ID);

            if (courseToDelete == null) return RedirectToAction("IndexAdmin", "Course");

            ctx.Courses.Remove(courseToDelete);
            ctx.SaveChanges();
            
            return RedirectToAction("IndexAdmin", "Course");
        }
        
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> Alter(Guid ID)
        {
            var course = ctx.Courses.FirstOrDefault(c => c.ID == ID);

            TempData["UpdateID"] = ID;

            var model = new CourseViewModel
            {
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                CoursePrice = course.CoursePrice,
                Teacher = course.Teacher
            };
            
            return View(model);
        }

        
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> Alter(CourseViewModel courseModel)
        {
            var course = new Course
            {
                ID = (Guid)TempData["UpdateID"],
                CourseName = courseModel.CourseName,
                CourseDescription = courseModel.CourseDescription,
                CoursePrice = courseModel.CoursePrice,
                Teacher = courseModel.Teacher
            };
            
            if(ModelState.IsValid)
            {
                ctx.Courses.Update(course);
                await ctx.SaveChangesAsync();

                return RedirectToAction("IndexAdmin", "Course");
            }

            return View();
        }
    }
}
