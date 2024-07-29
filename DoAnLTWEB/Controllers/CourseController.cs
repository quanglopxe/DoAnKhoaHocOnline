using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DoAnLTWEB.Models;
using PagedList;

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
        public ActionResult Course(int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);            
            var listCourse = db.KhoaHocs.Where(c => c.TrangThai == true).ToPagedList(pageNumber,pageSize);
            if (Session["SuccessMessage"] != null)
            {
                ViewBag.Message = Session["SuccessMessage"].ToString();
                Session["SuccessMessage"] = null;
            }
            return View(listCourse);
        }
        [HttpPost]
        public ActionResult Course(string searchStr, int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            if (searchStr != null)
            {                
                var listCourse = db.KhoaHocs.Where(c => c.TrangThai == true && c.TenMonHoc.Contains(searchStr)).ToPagedList(pageNumber, pageSize);
                return View(listCourse);
            }
            return View();
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