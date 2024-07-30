using DoAnLTWEB.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using DoAnLTWEB.Filter;
using PagedList;
using System.Web;
using DoAnLTWEB.ViewModels;

namespace DoAnLTWEB.Controllers
{    
    public class LessonController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;
        public LessonController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }
        public ActionResult Lesson(string maKH, int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                var listBaiGiang = db.BaiGiangs.Where(c => c.MaKH == maKH && c.TrangThai == true).ToPagedList(pageNumber,pageSize);
                Session["MaKH"] = maKH;
                ViewData["IsRegisterCourse"] = false;
                if (listBaiGiang.Count > 0)
                {                                        
                    NguoiDung nguoiDung = (NguoiDung)Session["User"];                    
                    var listMaKH = Session["CourseIds"] as List<string>;                                        

                    if (nguoiDung != null)
                    {
                        HoaDonDky dKyKhoaHoc = db.HoaDonDkies.FirstOrDefault(t => t.MaND == nguoiDung.MaND);                        
                        if(dKyKhoaHoc != null && listMaKH != null && listMaKH.Contains(maKH))
                        {
                            ViewData["IsRegisterCourse"] = true;
                        }                        
                        else
                        {
                            return View(listBaiGiang);
                        }
                    }                    
                }
                return View(listBaiGiang);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }
        [CheckRegisteredCourse]
        public ActionResult LessonDetail(string maBG)
        {
            try
            {
                var lesson = db.BaiGiangs.FirstOrDefault(m => m.MaBG == maBG);
                if (lesson == null)
                {
                    return RedirectToAction("Error", "User", new { error = "Không tìm thấy bài giảng " + maBG });
                }                
                var listBaiGiang = db.BaiGiangs.Where(bg => bg.MaKH == lesson.MaKH).ToList();
                var viewModel = new LessonDetailViewModel()
                {
                    baiGiang = lesson,
                    listBG = listBaiGiang
                };

                var videoType = lesson.Video.Substring(0, 5);
                ViewBag.VideoType = videoType;                
                if(videoType == "https")
                {
                    ViewBag.Video = GetEmbedYouTubeLink(lesson.Video);
                }                
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }
        private string GetEmbedYouTubeLink(string link)
        {
            Regex regex = new Regex(@"(?<=v=)[a-zA-Z0-9_-]*");

            Match match = regex.Match(link);
            string newLink = "https://www.youtube.com/";
            if (match.Success)
            {
                newLink += "embed/" + match.Groups[0].Value;
            }

            return newLink;
        }
        
    }
}