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
        public ActionResult AdminLesson(int? page)
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
        public string generateMaBG()
        {
            int curYear = DateTime.Now.Year;
            string newMaKH = "BG" + curYear;

            if (db.BaiGiangs == null)
            {
                newMaKH += "000";
            }
            else
            {
                int count = db.BaiGiangs.Where(k => k.MaKH.StartsWith(newMaKH)).Count();
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
        public ActionResult CreateLesson(string maKH)
        {
            var BaiGiang = db.BaiGiangs.Where(l => l.MaKH == maKH).FirstOrDefault();
            return View(BaiGiang);
        }
        [HttpPost]
        public ActionResult CreateLesson(BaiGiang baiGiang, HttpPostedFileBase videoFile, string youtubeLink)
        {
            if (ModelState.IsValid)
            {
                baiGiang.MaBG = generateMaBG();

                // Lưu file video
                if (videoFile != null && videoFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(videoFile.FileName);
                    string filePath = Server.MapPath("~/uploads/videos/" + fileName);
                    videoFile.SaveAs(filePath);

                    baiGiang.Video = "file:" + filePath;
                    db.BaiGiangs.InsertOnSubmit(baiGiang);
                    db.SubmitChanges();
                    return RedirectToAction("AdminLesson");
                }
                else if (youtubeLink != null)
                {
                    // Lưu thông tin video vào bảng BaiGiang
                    baiGiang.Video = "youtube:" + youtubeLink;
                    db.BaiGiangs.InsertOnSubmit(baiGiang);
                    db.SubmitChanges();
                    return RedirectToAction("AdminLesson");
                }
                else
                {
                    return View(baiGiang);
                }
            }
            else
            {
                return View(baiGiang);
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
        public ActionResult EditLesson(string MaKH)
        {
            var BG = db.BaiGiangs.Where(bg => bg.MaKH == MaKH).ToList();
            return View(BG);
        }

        [HttpPost]
        public ActionResult EditLesson(BaiGiang bg)
        {
            if (ModelState.IsValid)
            {
                var BG = db.BaiGiangs.Where(k => k.MaKH == bg.MaKH).FirstOrDefault();
                if (BG != null)
                {
                    BG.TieuDeBG = bg.TieuDeBG;
                    BG.NoiDungBG = bg.NoiDungBG;
                    BG.ThuTuBaiHoc = bg.ThuTuBaiHoc;
                    BG.Video = bg.Video;
                    
                    db.SubmitChanges();
                    return RedirectToAction("AdminLesson");
                }
            }

            return View(bg);
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var listCourse = db.KhoaHocs.Where(kh => kh.TrangThai == false).ToPagedList(pageNumber,pageSize);
            return View(listCourse);            
        }
        
        [HttpPost]
        public ActionResult RecoverEachCourse(List<KhoaHoc> selectedCourses)
        {            
            var selectedCourseIds = selectedCourses.Where(c => c.IsSelected).Select(c => c.MaKH);            
            var coursesToRecover = db.KhoaHocs.Where(k => selectedCourseIds.Contains(k.MaKH));
            foreach (var course in coursesToRecover)
            {
                course.TrangThai = true;
            }
            db.SubmitChanges();

            return RedirectToAction("AdminCourse");
        }

        public ActionResult RecoverAllCourse()
        {
            var coursesToRecover = db.KhoaHocs.Where(kh => kh.TrangThai == false);
            
            foreach (var course in coursesToRecover)
            {
                course.TrangThai = true;
            }
            db.SubmitChanges();

            return RedirectToAction("AdminCourse");
        }
    }
}