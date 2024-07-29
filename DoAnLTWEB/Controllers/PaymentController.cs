using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnLTWEB.Filter;
using DoAnLTWEB.Models;
using DoAnLTWEB.Payment;

namespace DoAnLTWEB.Controllers
{
    public class PaymentController : Controller
    {
        private readonly QuanLyKhoaHocDataContext db;
        public PaymentController(QuanLyKhoaHocDataContext _db)
        {
            db = _db;
        }
        // GET: Payment
        public ActionResult VnPayPayment()
        {

            var user = (NguoiDung)Session["User"];
            var hoadondky = db.HoaDonDkies.OrderByDescending(hd => hd.MaHD).FirstOrDefault();
            var cthd = db.ChiTietHoaDons.Where(c => c.MaHD == hoadondky.MaHD).ToList();
            DateTime expireDate = DateTime.Now.AddDays(1);

            // Tạo đối tượng VnPayLibrary và cấu hình
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", "262XSFHX");
            vnpay.AddRequestData("vnp_Amount", ((int)(cthd.Sum(c => c.SoTien) * 100)).ToString()); // Tổng số tiền thanh toán
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang" + hoadondky.MaHD); // Thông tin đơn hàng
            vnpay.AddRequestData("vnp_OrderType", "other"); // Loại hình thanh toán
            vnpay.AddRequestData("vnp_ReturnUrl", Url.Action("VnPayReturn", "Payment", null, Request.Url.Scheme));
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã giao dịch
            vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "MMZXWISZNUUUNKGOZQPCPASLLTHYGMTB");

            return Redirect(paymentUrl);
        }


        public ActionResult VnPayReturn()
        {
            // Xử lý kết quả thanh toán
            var responseCode = Request.QueryString["vnp_ResponseCode"];
            var hoadondky = db.HoaDonDkies.OrderByDescending(hd => hd.MaHD).FirstOrDefault();            
            

            if (responseCode == "00")
            {
                // Cập nhật trạng thái
                hoadondky.TrangThai = "Đã thanh toán";
                db.SubmitChanges();
                Session["SuccessMessage"] = "Thanh toán thành công!";
                
                //Lưu các khóa học đã đăng ký
                var nd = (NguoiDung)Session["User"];
                var listHD = db.HoaDonDkies.Where(hd => hd.MaND == nd.MaND).ToList();
                var listCTHD = db.ChiTietHoaDons.Where(c => listHD.Select(hd => hd.MaHD).Contains(c.MaHD)).ToList();
                List<string> courseIds = new List<string>();

                foreach (var hd in listHD)
                {
                    if (hd.TrangThai == "Đã thanh toán")
                    {
                        foreach (var registeredCourses in listCTHD)
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

                //Set thời gian khóa học
                var listKH = db.KhoaHocs.Where(c => courseIds.Contains(c.MaKH)).ToList();
                foreach(var kh in listKH)
                {
                    kh.NgayBD = DateTime.Now;
                    kh.NgayKT = DateTime.Now.AddMonths(6);                                        
                }
                db.SubmitChanges();
                //Cập nhật lại giỏ hàng
                return RedirectToAction("XoaMyCoursesAll", "MyCourses");
            }
            else
            {
                hoadondky.TrangThai = "Đã hủy";
                db.SubmitChanges();
                ViewBag.Error = "Respond code != 00 => Thanh toán không thành công! Giao dịch đã bị hủy";
                return RedirectToAction("Course", "Course");                
            }            
        }
        public string generateMaHD()
        {
            int curYear = DateTime.Now.Year;
            string newMaHD = "HD" + curYear;

            if (db.KhoaHocs == null)
            {
                newMaHD += "000";
            }
            else
            {
                int count = db.HoaDonDkies.Where(k => k.MaHD.StartsWith(newMaHD)).Count();
                newMaHD += (count + 1).ToString("D3");
            }
            return newMaHD;
        }
        [CheckLogin]
        public ActionResult ThanhToan()
        {            
            var User = Session["User"] as NguoiDung;
            var listKH = Session["MyCourses"] as List<CourseCart>;            
            var KhachHang = db.NguoiDungs.Where(kh => kh.TenND == User.TenND).FirstOrDefault();                                 

            try
            {
                HoaDonDky dk1 = new HoaDonDky();
                dk1.MaND = KhachHang.MaND;
                dk1.MaHD = generateMaHD();
                dk1.NgayDangKy = DateTime.Now.Date;
                dk1.TrangThai = "Đang xử lý";
                db.HoaDonDkies.InsertOnSubmit(dk1);
                db.SubmitChanges();                
                foreach (var item in listKH)
                {
                    ChiTietHoaDon cttt = new ChiTietHoaDon();
                    cttt.MaHD = dk1.MaHD;
                    cttt.MaKH = item.MaKH;
                    cttt.SoTien = item.GiaKhoaHoc;
                    cttt.PhuongThuc = "Chuyển khoản";
                    db.ChiTietHoaDons.InsertOnSubmit(cttt);                    
                }
                db.SubmitChanges();                
                return RedirectToAction("VnPayPayment");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Error", "User", new { error = ex.Message });
            }
        }
        
    }
}