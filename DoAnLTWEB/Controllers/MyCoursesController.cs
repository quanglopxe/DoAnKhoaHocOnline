using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Models;

namespace DoAnLTWEB.Controllers
{
    
    public class MyCoursesController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;
        public MyCoursesController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }

        public List<MyCourses> GetMyCourses()
        {
            List<MyCourses> listMyCourses = Session["MyCourses"] as List<MyCourses>;
            if (listMyCourses == null)
            {
                listMyCourses = new List<MyCourses>();
                Session["MyCourses"] = listMyCourses;
            }
            return listMyCourses;
        }

        private int TongSoLuong()
        {
            int tsl = 0;
            List<MyCourses> listMyCourses = Session["MyCourses"] as List<MyCourses>;
            if (listMyCourses != null)
            {
                tsl = listMyCourses.Sum(sl => sl.iSoLuong);
            }
            return tsl;
        }

        private decimal TongThanhTien()
        {
            decimal ttt = 0;
            List<MyCourses> listMyCourses = Session["MyCourses"] as List<MyCourses>;
            if (listMyCourses != null)
            {
                ttt += listMyCourses.Sum(c => c.dDonGia);
            }
            return ttt;
        }

        //
        // GET: /MyCourses/
        public ActionResult MyCourses()
        {            
            List<MyCourses> listMyCourses = GetMyCourses();
            ViewBag.TongThanhTien = TongThanhTien();
            return View(listMyCourses);
        }

        public ActionResult MyCoursesPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            return View();
        }

        public ActionResult ThemMyCourses(string cID, string strURL)
        {
            List<MyCourses> listMyCourses = GetMyCourses();
            MyCourses khoahoc = listMyCourses.Find(c => c.sMaKH == cID);
            if (khoahoc == null)
            {
                khoahoc = new MyCourses(cID);
                listMyCourses.Add(khoahoc);
                return Redirect(strURL);
            }
            else
            {
                return RedirectToAction("MyCourses");
            }
        }

        public ActionResult XoaMyCourses(string MaKH)
        {
            List<MyCourses> listMyCourses = GetMyCourses();
            MyCourses kh = listMyCourses.Single(s => s.sMaKH == MaKH);
            if (kh != null)
            {
                listMyCourses.RemoveAll(s => s.sMaKH == MaKH);
                return RedirectToAction("MyCourses", "MyCourses");
            }

            if (listMyCourses.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("MyCourses", "MyCourses");
        }

        public ActionResult XoaMyCoursesAll()
        {
            List<MyCourses> listMyCourses = GetMyCourses();
            listMyCourses.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CapNhatMyCourses(string MaKH, FormCollection f)
        {
            List<MyCourses> listMyCourses = GetMyCourses();
            MyCourses kh = listMyCourses.Single(s => s.sMaKH == MaKH);
            return RedirectToAction("MyCourses", "MyCourses");
        }

        
	}
}