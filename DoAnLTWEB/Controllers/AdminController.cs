using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Models;
using PagedList;
using DoAnLTWEB.Filter;

namespace DoAnLTWEB.Controllers
{
    [CheckRole]
    public class AdminController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;

        public AdminController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }        
        // GET: Admin
        public ActionResult AdminCourse(int? page)
        {            
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var listCourse = db.KhoaHocs.Where(kh => kh.TrangThai == true).ToPagedList(pageNumber, pageSize);
            return View(listCourse);            
        }
        public string generateMaKH()
        {
            int curYear = DateTime.Now.Year;
            string newMaKH = "KH" + curYear;

            if (db.KhoaHocs == null)
            {
                newMaKH += "000";
            }
            else
            {
                int count = db.KhoaHocs.Where(k => k.MaKH.StartsWith(newMaKH)).Count();
                newMaKH += (count + 1).ToString("D3");
            }
            return newMaKH;
        }
        public ActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(KhoaHoc khoaHoc)
        {
            if (ModelState.IsValid)
            {                
                khoaHoc.MaKH = generateMaKH();
                khoaHoc.NgayBD = DateTime.Today;
                khoaHoc.NgayKT = DateTime.Today.AddMonths(6);
                khoaHoc.TrangThai = true;
                db.KhoaHocs.InsertOnSubmit(khoaHoc);
                db.SubmitChanges();
                return RedirectToAction("AdminCourse");
            }  
            else
            {

                return View(khoaHoc);
            }            
        }
        public ActionResult EditCourse(string MaKH)
        {
            var KH = db.KhoaHocs.Where(kh => kh.MaKH == MaKH).FirstOrDefault();
            return View(KH);
        }

        [HttpPost]
        public ActionResult EditCourse(KhoaHoc kh)
        {
            if (ModelState.IsValid)
            {
                var KH = db.KhoaHocs.Where(k => k.MaKH == kh.MaKH).FirstOrDefault();
                if (KH != null)
                {
                    KH.TenKH = kh.TenKH;
                    KH.MoTaKH = kh.MoTaKH;
                    KH.GiangVien = kh.GiangVien;                    
                    KH.GiaKhoaHoc = kh.GiaKhoaHoc;
                    KH.TenMonHoc = kh.TenMonHoc;
                    KH.Picture = kh.Picture;
                    db.SubmitChanges();
                    return RedirectToAction("AdminCourse");
                }
            }

            return View(kh);
        }


        [HttpPost]
        public ActionResult DeleteCourse(string MaKH)
        {
            var KH = db.KhoaHocs.Where(kh => kh.MaKH == MaKH).FirstOrDefault();
            if (KH != null)
            {
                KH.TrangThai = false;
                db.SubmitChanges();
            }

            return RedirectToAction("AdminCourse");
        }
        public ActionResult RecoverCourse(int ?page)
        {
            int pageSize = 1;
            int pageNumber = (page ?? 1);

            var listCourse = db.KhoaHocs.Where(kh => kh.TrangThai == false).ToPagedList(pageNumber,pageSize);
            return View(listCourse);            
        }
        
        [HttpPost]
        public ActionResult RecoverEachCourse(List<KhoaHoc> selectedCourses)
        {
            foreach (var course in selectedCourses.Where(c => c.IsSelected))
            {
                var kh = db.KhoaHocs.Where(k => k.MaKH == course.MaKH).FirstOrDefault();
                if (kh != null)
                {
                    kh.TrangThai = true;
                    db.SubmitChanges();
                }
            }

            return RedirectToAction("AdminCourse");
        }
        
        public ActionResult RecoverAllCourse()
        {
            List<KhoaHoc> listCourse = new List<KhoaHoc>();
            listCourse = db.KhoaHocs.Where(kh => kh.TrangThai == false).ToList();            
            if (listCourse != null)
            {
                for(int i = 0; i < listCourse.Count; i++)
                {
                    listCourse[i].TrangThai = true;
                    db.SubmitChanges();
                }                
            }

            return RedirectToAction("AdminCourse");
        }
    }
}