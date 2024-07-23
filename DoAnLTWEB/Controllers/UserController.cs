using DoAnLTWEB.Filter;
using DoAnLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnLTWEB.Controllers
{    
    public class UserController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;
        private readonly CourseController courseController;

        public UserController(QuanLyKhoaHocDataContext _db, CourseController _courseController)
        {
            db = _db;
            courseController = _courseController;
        }
        
        public ActionResult Login()
        {
            Session["Roles"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Login(NguoiDung nguoiDung)
        {
            try
            {
                NguoiDung nd = db.NguoiDungs.FirstOrDefault(t => t.TenND == nguoiDung.TenND);

                if (nd != null && BCrypt.Net.BCrypt.Verify(nguoiDung.MatKhau,nd.MatKhau))
                {
                    Session["User"] = nd;
                    if (nd.Roles == true)
                    {
                        Session["Roles"] = "Admin";
                        return RedirectToAction("AdminCourse", "Admin");
                    }
                    else
                    {
                        Session["Roles"] = "User";
                        var listHD = db.HoaDonDkies.Where(hd => hd.MaND == nd.MaND).ToList();
                        var cthd = db.ChiTietHoaDons.Where(c => listHD.Select(hd => hd.MaHD).Contains(c.MaHD)).ToList();
                        List<string> courseIds = new List<string>();

                        foreach (var hd in listHD)
                        {
                            if (hd.TrangThai == "Đã thanh toán")
                            {
                                foreach (var registeredCourses in cthd)
                                {
                                    var courses = db.KhoaHocs.FirstOrDefault(kh => kh.MaKH == registeredCourses.MaKH);
                                    if (courses != null)
                                    {
                                        courseIds.Add(registeredCourses.MaKH);
                                    }
                                }
                                Session["CourseIds"] = courseIds;
                            }
                        }
                        return RedirectToAction("Index", "Home");
                    }                    
                }
                else
                {
                    ViewBag.Message = "Tên tài khoản hoặc mật khẩu không đúng!";
                    return View(nguoiDung);
                }
                                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }

        public string generateMaND()
        {
            int curYear = DateTime.Now.Year;
            string newMaND = "ND" + curYear;

            if (db.KhoaHocs == null)
            {
                newMaND += "000";
            }
            else
            {
                int count = db.NguoiDungs.Where(k => k.MaND.StartsWith(newMaND)).Count();
                newMaND += (count + 1).ToString("D3");
            }
            return newMaND;
        }

        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Register(NguoiDung nguoiDung)
        {                        
            try
            {
                if(ModelState.IsValid)
                {
                    if (db.NguoiDungs.Any(u => u.TenND == nguoiDung.TenND))
                    {
                        ModelState.AddModelError("TenND", "Tên tài khoản này đã tồn tại.");
                        return View(nguoiDung);
                    }
                    if (db.NguoiDungs.Any(u => u.Email == nguoiDung.Email))
                    {
                        ModelState.AddModelError("Email", "Email này đã tồn tại.");
                        return View(nguoiDung);
                    }
                    if (db.NguoiDungs.Any(u => u.DienThoai == nguoiDung.DienThoai))
                    {
                        ModelState.AddModelError("DienThoai", "Số điện thoại này đã tồn tại.");
                        return View(nguoiDung);
                    }
                    else
                    {
                        nguoiDung.MaND = generateMaND();
                        string hashPassword = BCrypt.Net.BCrypt.HashPassword(nguoiDung.MatKhau);
                        nguoiDung.MatKhau = hashPassword;
                        db.NguoiDungs.InsertOnSubmit(nguoiDung);
                        db.SubmitChanges();
                        return RedirectToAction("Login", "User");
                    }
                }
                else
                {
                    ViewBag.TenND = nguoiDung.TenND;
                    ViewBag.MatKhau = nguoiDung.MatKhau;
                    ViewBag.HoTen = nguoiDung.HoTen;
                    ViewBag.Email = nguoiDung.Email;
                    ViewBag.DienThoai = nguoiDung.DienThoai;
                    return View(nguoiDung);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }
        [CheckLogin]
        public ActionResult EditProfile()
        {
            NguoiDung nguoiDung = (NguoiDung)Session["User"];            
            return View(nguoiDung);
        }
        [CheckLogin]
        [HttpPost]
        public ActionResult EditProfile(NguoiDung nguoiDung)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NguoiDung nd = db.NguoiDungs.FirstOrDefault(t => t.MaND == nguoiDung.MaND);

                    if (nd.Email != nguoiDung.Email)
                    {
                        // Kiểm tra email mới có trùng với email đang tồn tại hay không
                        NguoiDung existingEmail = db.NguoiDungs.FirstOrDefault(u => u.Email == nguoiDung.Email && u.MaND != nguoiDung.MaND);
                        if (existingEmail != null)
                        {
                            ModelState.AddModelError("Email", "Email này đã tồn tại.");
                            return View(nguoiDung);
                        }
                    }

                    nd.HoTen = nguoiDung.HoTen;
                    nd.Email = nguoiDung.Email;
                    nd.DienThoai = nguoiDung.DienThoai;
                    Session["User"] = nd;
                    db.SubmitChanges();
                    return RedirectToAction("Course", "Course");
                }
                else
                    return View(nguoiDung);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }
        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["Roles"] = null;
            Session["CourseIds"] = null;
            Session["MyCourses"] = null;
            return RedirectToAction("Login", "User");
        }
        public ActionResult Error(string error = "")
        {
            ViewBag.ErrorMessage = error;
            return View();
        }
    }
}