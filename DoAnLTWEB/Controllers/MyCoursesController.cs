using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Filter;
using DoAnLTWEB.Models;

namespace DoAnLTWEB.Controllers
{
    [CheckLogin]
    public class MyCoursesController : Controller
    {        
        private readonly QuanLyKhoaHocDataContext db;
        public MyCoursesController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }

        public List<CourseCart> GetMyCourses()
        {
            var nd = (NguoiDung)Session["User"];
            var listMyCourses = db.CourseCarts.Where(l => l.MaND == nd.MaND).ToList();
            if (listMyCourses == null)
            {
                listMyCourses = new List<CourseCart>();                
            }
            Session["MyCourses"] = listMyCourses;
            return listMyCourses;
        }

        private int TongSoLuong()
        {
            int tsl = 0;
            var nd = (NguoiDung)Session["User"];
            var listMyCourses = GetMyCourses();
            if (listMyCourses != null)
            {
                tsl = listMyCourses.Count();
            }
            return tsl;
        }

        private decimal TongThanhTien()
        {
            decimal ttt = 0;
            var nd = (NguoiDung)Session["User"];
            var listMyCourses = db.CourseCarts.Where(l => l.MaND == nd.MaND).ToList();
            if (listMyCourses != null)
            {
                ttt += (decimal)listMyCourses.Sum(c => c.GiaKhoaHoc);
            }
            return ttt;
        }

        //
        // GET: /MyCourses/
        public ActionResult MyCourses()
        {            
            List<CourseCart> listMyCourses = GetMyCourses();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listMyCourses);
        }
        public ActionResult ViewSoLuong()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            return View();
        }

        public ActionResult MyCoursesPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            return View();
        }
        [HttpPost]
        public ActionResult ThemMyCourses(string cID/*, string strURL*/)
        {                        
            var nd = (NguoiDung)Session["User"];
            var maND = nd.MaND;
            List<CourseCart> listMyCourses = GetMyCourses();
            CourseCart khoahoc = listMyCourses.Find(c => c.MaKH == cID && c.MaND == maND);
            if (khoahoc == null)
            {
                khoahoc = new CourseCart(cID, maND);
                listMyCourses.Add(khoahoc);
                db.CourseCarts.InsertOnSubmit(khoahoc);
                db.SubmitChanges();
                //return Redirect(strURL);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        public ActionResult XoaMyCourses(string MaKH)
        {
            var nd = (NguoiDung)Session["User"];
            var maND = nd.MaND;
            List<CourseCart> listMyCourses = GetMyCourses();
            CourseCart kh = listMyCourses.Single(s => s.MaKH == MaKH && s.MaND == maND);
            if (kh != null)
            {
                listMyCourses.RemoveAll(s => s.MaKH == MaKH && s.MaND == maND);
                db.CourseCarts.DeleteOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("MyCourses", "MyCourses");
            }            
            return RedirectToAction("MyCourses", "MyCourses");
        }

        public ActionResult XoaMyCoursesAll()
        {
            var nd = (NguoiDung)Session["User"];
            List<CourseCart> listMyCourses = GetMyCourses();            
            foreach(var courseCart in listMyCourses)
            {
                db.CourseCarts.DeleteOnSubmit(courseCart);
            }
            db.SubmitChanges();
            listMyCourses.Clear();            
            return RedirectToAction("Course", "Course");
        }

        //public ActionResult CapNhatMyCourses(string MaKH, FormCollection f)
        //{
        //    List<CourseCart> listMyCourses = GetMyCourses();
        //    CourseCart kh = listMyCourses.Single(s => s.MaKH == MaKH);
        //    return RedirectToAction("MyCourses", "MyCourses");
        //}

        
	}
}