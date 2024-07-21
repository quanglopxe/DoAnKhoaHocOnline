using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Models;

namespace DoAnLTWEB.Controllers
{
    
    public class CourseController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;

        public CourseController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }
        //
        // GET: /Course/        
        public ActionResult Course()
        {
            List<KhoaHoc> listCourse = new List<KhoaHoc>();
            listCourse = db.KhoaHocs.Where(c => c.TrangThai == true).ToList();
            if (Session["SuccessMessage"] != null)
            {
                ViewBag.Message = Session["SuccessMessage"].ToString();
                Session["SuccessMessage"] = null;
            }
            return View(listCourse);
        }
        [HttpPost]
        public ActionResult Course(string searchStr)
        {
            List<KhoaHoc> listCourse = new List<KhoaHoc>();            
            if (searchStr != null)
            {                
                listCourse = db.KhoaHocs.Where(c => c.TrangThai == true && c.TenMonHoc.Contains(searchStr)).ToList();
            }            
            return View(listCourse);
        }
        public ActionResult RegisterdCourses()
        {
            var listCourseId= Session["CourseIds"] as List<string>;
            if (listCourseId == null)
            {
                return View();
            }
            else
            {
                var listCourse = db.KhoaHocs.Where(c => listCourseId.Contains(c.MaKH)).ToList();
                return View(listCourse);
            }
        }
        public ActionResult RegisterdCoursesPartial()
        {            
            var listCourseId = Session["CourseIds"] as List<string>;
            if (listCourseId == null)
            {
                ViewBag.SoLuong = 0;
                return View();
            }
            else
            {
                var listCourse = db.KhoaHocs.Where(c => listCourseId.Contains(c.MaKH)).ToList();
                ViewBag.SoLuong = listCourse.Count();
                return View();
            }
        }
        public ActionResult CourseWithSubjectTitle(string TenMH)
        {
            List<KhoaHoc> listCourse = new List<KhoaHoc>();
            ViewBag.TenMH = null;
            if(TenMH != null)
            {
                listCourse = db.KhoaHocs.Where(c => c.TrangThai == true && c.TenMonHoc == TenMH).ToList();
                ViewBag.TenMH = TenMH;
            }
            else
            {
                return RedirectToAction("Course");
            }
            return View(listCourse);
        }
        
        
        public ActionResult SideBar()
        {
            var listName = db.KhoaHocs.Where(c => c.TrangThai == true).ToList();
            return View(listName);
        }

        
    }
}