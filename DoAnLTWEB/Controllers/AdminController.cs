using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Models;
using PagedList;
using DoAnLTWEB.Filter;
using System.Web.UI.WebControls;

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
        public string generateMaBG()
        {
            int curYear = DateTime.Now.Year;
            string newMaBG = "BG" + curYear;

            if (db.BaiGiangs == null)
            {
                newMaBG += "000";
            }
            else
            {
                int count = db.BaiGiangs.Where(k => k.MaBG.StartsWith(newMaBG)).Count();
                newMaBG += (count + 1).ToString("D3");
            }
            return newMaBG;
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
            var baiGiang = new BaiGiang();
            int soBaiGiang = db.BaiGiangs.Where(l => l.MaKH == maKH).Count();
            baiGiang.ThuTuBaiHoc = soBaiGiang + 1;
            baiGiang.MaKH = maKH;
            return View(baiGiang);
        }
        [HttpPost]
        public ActionResult CreateLesson(BaiGiang baiGiang, string youtubeLink)
        {
            var existedTitle = db.BaiGiangs.Any(t => t.TieuDeBG == baiGiang.TieuDeBG);
            if (existedTitle == true)
            {
                ModelState.AddModelError("TieuDeBG", "Tiêu đề bài giảng đã tồn tại!");
            }
            if (ModelState.IsValid)
            {
                baiGiang.MaBG = generateMaBG();
                baiGiang.TrangThai = true;                
                                
                if (baiGiang.videoFile != null && baiGiang.videoFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(baiGiang.videoFile.FileName);
                    string filePath = Server.MapPath("~/Content/Video/" + fileName);
                    baiGiang.videoFile.SaveAs(filePath);

                    baiGiang.Video = "~/Content/Video/" + fileName;
                    db.BaiGiangs.InsertOnSubmit(baiGiang);
                    db.SubmitChanges();
                    return RedirectToAction("AdminCourse");
                }
                else if (!String.IsNullOrEmpty(youtubeLink))
                {                    
                    baiGiang.Video = youtubeLink;
                    db.BaiGiangs.InsertOnSubmit(baiGiang);
                    db.SubmitChanges();
                    return RedirectToAction("AdminCourse");
                }
                else
                {
                    ViewBag.ErrorMessage = "Vui lòng tải file bài giảng lên hoặc truyền đường dẫn bài giảng trên youtube!";
                    return View(baiGiang);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Vui lòng tải file bài giảng lên hoặc truyền đường dẫn bài giảng trên youtube!";
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
                var existedTitle = db.KhoaHocs.Any(t => t.TenKH == kh.TenKH && t.MaKH != kh.MaKH);
                if (existedTitle == true)
                {
                    ModelState.AddModelError("TenKH", "Tên khóa học đã tồn tại!");
                }
                
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
        public ActionResult EditLesson(string MaBG)
        {
            var BG = db.BaiGiangs.Where(bg => bg.MaBG == MaBG).FirstOrDefault();            
            return View(BG);
        }

        [HttpPost]
        public ActionResult EditLesson(BaiGiang bg, string youtubeLink)
        {
            var existedTitle = db.BaiGiangs.Any(t => t.TieuDeBG == bg.TieuDeBG && t.MaBG != bg.MaBG);
            if (existedTitle == true)
            {
                ModelState.AddModelError("TieuDeBG", "Tiêu đề bài giảng đã tồn tại!");
            }
            if (ModelState.IsValid)
            {
                var BG = db.BaiGiangs.Where(k => k.MaBG == bg.MaBG).FirstOrDefault();
                if (BG != null)
                {
                    BG.TieuDeBG = bg.TieuDeBG;
                    BG.NoiDungBG = bg.NoiDungBG;
                    BG.ThuTuBaiHoc = bg.ThuTuBaiHoc;

                    if (bg.videoFile != null && bg.videoFile.ContentLength > 0)
                    {
                        string fileName = System.IO.Path.GetFileName(bg.videoFile.FileName);
                        string filePath = Server.MapPath("~/Content/Video/" + fileName);
                        bg.videoFile.SaveAs(filePath);

                        bg.Video = "~/Content/Video/" + fileName;                        
                    }
                    else if (!String.IsNullOrEmpty(youtubeLink))
                    {
                        bg.Video = youtubeLink;                        
                    }
                    BG.Video = bg.Video;

                    db.SubmitChanges();
                    return RedirectToAction("Lesson", "Lesson", new { maKH = bg.MaKH });
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
        [HttpPost]
        public ActionResult DeleteLesson(string MaBG)
        {
            var BG = db.BaiGiangs.Where(bg => bg.MaBG == MaBG).FirstOrDefault();
            if (BG != null)
            {
                BG.TrangThai = false;
                db.SubmitChanges();
            }

            return RedirectToAction("Lesson", "Lesson", new { maKH = BG.MaKH });
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

        public ActionResult RecoverLesson(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var listLesson = db.BaiGiangs.Where(kh => kh.TrangThai == false).ToPagedList(pageNumber, pageSize);            
            return View(listLesson);
        }

        [HttpPost]
        public ActionResult RecoverEachLesson(List<BaiGiang> selectedLessons)
        {
            var baiGiang= selectedLessons.First();
            var BG = db.BaiGiangs.Where(l => l.MaBG == baiGiang.MaBG).FirstOrDefault();
            var selectedLessonIds = selectedLessons.Where(c => c.IsSelected).Select(c => c.MaBG);
            var lessonsToRecover = db.BaiGiangs.Where(k => selectedLessonIds.Contains(k.MaBG));
            foreach (var lesson in lessonsToRecover)
            {
                lesson.TrangThai = true;
            }
            db.SubmitChanges();

            return RedirectToAction("Lesson", "Lesson", new { maKH = BG.MaKH });
        }

        public ActionResult RecoverAllLesson()
        {
            var lessonsToRecover = db.BaiGiangs.Where(kh => kh.TrangThai == false);
            var baiGiang = lessonsToRecover.First();
            foreach (var lesson in lessonsToRecover)
            {
                lesson.TrangThai = true;
            }
            db.SubmitChanges();

            return RedirectToAction("Lesson", "Lesson", new { maKH = baiGiang.MaKH });
        }
    }
}